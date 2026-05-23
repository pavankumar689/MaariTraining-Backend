using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DevFastTrack.API.Data;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/payment")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentController(AppDbContext db, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var keyId = _config["Razorpay:KeyId"];
            var keySecret = _config["Razorpay:KeySecret"];

            Console.WriteLine($"KeyId: {(keyId != null && keyId.Length > 10 ? keyId.Substring(0, 10) : keyId)}...");
            Console.WriteLine($"KeySecret length: {keySecret?.Length ?? 0}");

            if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(keySecret) || 
                keyId == "YOUR_RAZORPAY_KEY_ID" || keySecret == "YOUR_RAZORPAY_KEY_SECRET")
            {
                return BadRequest(new { error = "Razorpay credentials not configured. Please update appsettings.json with real credentials." });
            }

            var course = await _db.Courses.FindAsync(request.CourseId);
            if (course == null) return NotFound(new { error = "Course not found" });

            var batch = await _db.Batches.Include(b => b.Enrollments).FirstOrDefaultAsync(b => b.Id == request.BatchId);
            if (batch == null) return NotFound(new { error = "Batch not found" });

            if (batch.Enrollments.Count >= batch.SeatsTotal)
            {
                return BadRequest(new { error = "This batch is full." });
            }

            // Create order using Razorpay REST API directly
            var httpClient = _httpClientFactory.CreateClient();
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{keySecret}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

            var orderData = new
            {
                amount = (int)(course.Price * 100), // Amount in paise from DB
                currency = "INR",
                receipt = $"rcpt_{DateTime.UtcNow.Ticks}",
                notes = new Dictionary<string, string>
                {
                    { "courseId", request.CourseId.ToString() },
                    { "batchId", request.BatchId.ToString() },
                    { "userId", User.FindFirstValue(ClaimTypes.NameIdentifier)! },
                    { "amount", course.Price.ToString() }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(orderData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new { error = $"Razorpay API error: {responseBody}" });
            }

            var order = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return Ok(new
            {
                orderId = order.GetProperty("id").GetString(),
                amount = course.Price,
                currency = "INR",
                keyId = keyId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Failed to create order: {ex.Message}" });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        try
        {
            var keyId = _config["Razorpay:KeyId"];
            var keySecret = _config["Razorpay:KeySecret"];

            if (string.IsNullOrEmpty(keySecret) || string.IsNullOrEmpty(keyId))
            {
                return BadRequest(new { error = "Razorpay credentials not configured" });
            }

            // Verify signature using HMAC SHA256
            var signature = request.RazorpayOrderId + "|" + request.RazorpayPaymentId;
            var expectedSignature = GenerateSignature(signature, keySecret);

            if (expectedSignature != request.RazorpaySignature)
            {
                return BadRequest(new { error = "Invalid payment signature" });
            }

            // Fetch order from Razorpay to get notes securely
            var httpClient = _httpClientFactory.CreateClient();
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{keySecret}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

            var response = await httpClient.GetAsync($"https://api.razorpay.com/v1/orders/{request.RazorpayOrderId}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new { error = "Failed to verify order details with Razorpay" });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<JsonElement>(responseBody);
            
            var notes = order.GetProperty("notes");
            var courseId = int.Parse(notes.GetProperty("courseId").GetString()!);
            var batchId = int.Parse(notes.GetProperty("batchId").GetString()!);
            var userId = int.Parse(notes.GetProperty("userId").GetString()!);
            var amount = decimal.Parse(notes.GetProperty("amount").GetString()!);

            // Ensure current user is the one who created the order
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (userId != currentUserId)
            {
                return Forbid();
            }

            // Check if already enrolled
            var alreadyEnrolled = await _db.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
            if (!alreadyEnrolled)
            {
                // Create Enrollment
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId,
                    BatchId = batchId,
                    PaymentId = request.RazorpayPaymentId,
                    AmountPaid = amount,
                    Status = "Active"
                };

                _db.Enrollments.Add(enrollment);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                success = true,
                paymentId = request.RazorpayPaymentId,
                orderId = request.RazorpayOrderId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Payment verification failed: {ex.Message}" });
        }
    }

    private string GenerateSignature(string data, string secret)
    {
        var encoding = new System.Text.UTF8Encoding();
        var keyBytes = encoding.GetBytes(secret);
        var dataBytes = encoding.GetBytes(data);

        using var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}

public class CreateOrderRequest
{
    public int CourseId { get; set; }
    public int BatchId { get; set; }
}

public class VerifyPaymentRequest
{
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public string RazorpaySignature { get; set; } = string.Empty;
}
