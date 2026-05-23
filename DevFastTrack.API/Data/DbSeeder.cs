using DevFastTrack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if data already exists
        if (await context.Courses.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed Courses
        var courses = new List<Course>
        {
            new Course
            {
                Title = "🔥 Coding Confidence Workshop",
                Description = "Perfect for Non-IT Students (ECE, Mechanical, Civil & Others)! Transform your coding journey in just 2 hours. Learn Python basics and problem-solving framework. Live interactive session - No prior coding experience needed!",
                Price = 99,
                OriginalPrice = 999,
                Duration = "2 hours",
                Level = "Beginner",
                ThumbnailUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=800",
                Syllabus = @"⏰ Session 1 (30 mins): Python Basics for Absolute Beginners
- Why Python is perfect for non-IT students
- Setting up your first Python program
- Variables, data types, and basic operations
- Live coding demonstration

⏰ Session 2 (45 mins): Problem-Solving Framework
- The 3-step approach used by professional developers
- Hands-on: Solve 3 simple problems together
- Learn to think logically and break down problems
- Common mistakes and how to avoid them

⏰ Session 3 (30 mins): Your Personalized Roadmap
- Custom 30-day learning plan for non-IT students
- Free resources and practice platforms
- How to build projects for your resume
- Q&A with mentor

⏰ Bonus (15 mins): Career Opportunities
- Tech jobs for non-IT graduates
- How to transition into software development
- Building your coding portfolio from scratch",
                Prerequisites = @"No coding experience required - Perfect for absolute beginners!
Just a laptop with internet connection
Willingness to learn and participate actively
Ideal for ECE, Mechanical, Civil, and other non-IT students",
                Outcomes = @"✅ Learn Python basics from scratch
✅ Gain confidence to write your first programs
✅ Understand problem-solving like a developer
✅ Get your personalized 30-day learning roadmap
✅ Solve 3 real coding problems with guidance
✅ Learn how non-IT students can enter tech industry
✅ Certificate of completion
✅ Lifetime access to workshop recording
✅ Free Python learning resources worth ₹2000",
                MentorName = "Rahul Kumar",
                MentorBio = "Ex-Amazon SDE with 8+ years of experience. Helped 500+ non-IT students transition into tech careers. Known for making coding simple and fun!",
                IsActive = true
            },
            new Course
            {
                Title = "Maari Training | Professional DSA Course",
                Description = "Master Data Structures & Algorithms with live classes, personal mentorship, and guaranteed interview preparation. Small batches of max 15 students.",
                Price = 4999,
                OriginalPrice = 9999,
                Duration = "8 weeks",
                Level = "Beginner",
                ThumbnailUrl = "https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=800",
                Syllabus = @"Week 1: Arrays & Strings
Week 2: Linked Lists & Stacks
Week 3: Trees & Graphs
Week 4: Dynamic Programming
Week 5: Sorting & Searching
Week 6: Advanced Topics
Week 7: System Design Basics
Week 8: Mock Interviews",
                Prerequisites = @"Basic programming knowledge in any language
Willingness to practice 2-3 hours daily
A computer with stable internet connection
No prior DSA knowledge required",
                Outcomes = @"Master fundamental data structures
Solve complex algorithmic problems
Understand time and space complexity
Learn proven problem-solving patterns
Practice 100+ coding interview questions
Crack FAANG-style technical interviews",
                MentorName = "Rahul Kumar",
                MentorBio = "Ex-Amazon SDE with 8+ years of experience. Helped 500+ students crack top tech companies.",
                IsActive = true
            },
            new Course
            {
                Title = "Full Stack Web Development Bootcamp",
                Description = "Learn MERN stack from scratch. Build real-world projects and deploy them. Get job-ready in 12 weeks.",
                Price = 7999,
                OriginalPrice = 14999,
                Duration = "12 weeks",
                Level = "Intermediate",
                ThumbnailUrl = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=800",
                Syllabus = @"Week 1-2: HTML, CSS, JavaScript
Week 3-4: React.js Fundamentals
Week 5-6: Node.js & Express
Week 7-8: MongoDB & Database Design
Week 9-10: Authentication & Security
Week 11: Deployment & DevOps
Week 12: Final Project",
                Prerequisites = @"Basic computer knowledge
Passion for web development
No coding experience required
Commitment to learn 3-4 hours daily",
                Outcomes = @"Build full-stack web applications
Master React.js and Node.js
Deploy applications to cloud
Create RESTful APIs
Work with databases
Build portfolio projects",
                MentorName = "Priya Sharma",
                MentorBio = "Full Stack Developer at Google. 6 years of experience building scalable web applications.",
                IsActive = true
            },
            new Course
            {
                Title = "System Design Masterclass",
                Description = "Learn to design scalable systems like Netflix, Uber, and WhatsApp. Perfect for senior engineer interviews.",
                Price = 6999,
                OriginalPrice = 12999,
                Duration = "6 weeks",
                Level = "Advanced",
                ThumbnailUrl = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800",
                Syllabus = @"Week 1: System Design Fundamentals
Week 2: Scalability & Load Balancing
Week 3: Database Design & Caching
Week 4: Microservices Architecture
Week 5: Real-world System Designs
Week 6: Mock Interviews",
                Prerequisites = @"2+ years of software development experience
Understanding of basic data structures
Knowledge of databases
Familiarity with web technologies",
                Outcomes = @"Design scalable distributed systems
Understand trade-offs in system design
Learn caching strategies
Master database sharding
Prepare for senior engineer interviews
Build system design portfolio",
                MentorName = "Arjun Mehta",
                MentorBio = "Principal Engineer at Microsoft. Designed systems handling millions of users.",
                IsActive = true
            }
        };

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        // Seed Batches for each course
        var batches = new List<Batch>();
        
        // Special batches for workshop (first course)
        var workshop = courses[0];
        batches.Add(new Batch
        {
            CourseId = workshop.Id,
            BatchName = "🔥 This Saturday - 10 AM Batch",
            StartDate = new DateTime(2026, 5, 17),
            EndDate = new DateTime(2026, 5, 17),
            Timing = "Saturday 10:00 AM - 12:00 PM IST",
            MeetingLink = "https://meet.google.com/workshop-sat-morning",
            SeatsTotal = 20,
            IsActive = true
        });

        batches.Add(new Batch
        {
            CourseId = workshop.Id,
            BatchName = "🔥 This Sunday - 4 PM Batch",
            StartDate = new DateTime(2026, 5, 18),
            EndDate = new DateTime(2026, 5, 18),
            Timing = "Sunday 4:00 PM - 6:00 PM IST",
            MeetingLink = "https://meet.google.com/workshop-sun-evening",
            SeatsTotal = 20,
            IsActive = true
        });

        batches.Add(new Batch
        {
            CourseId = workshop.Id,
            BatchName = "Next Saturday - 10 AM Batch",
            StartDate = new DateTime(2026, 5, 24),
            EndDate = new DateTime(2026, 5, 24),
            Timing = "Saturday 10:00 AM - 12:00 PM IST",
            MeetingLink = "https://meet.google.com/workshop-next-sat",
            SeatsTotal = 20,
            IsActive = true
        });
        
        // Regular batches for other courses
        for (int i = 1; i < courses.Count; i++)
        {
            var course = courses[i];
            batches.Add(new Batch
            {
                CourseId = course.Id,
                BatchName = "May 2026 - Morning Batch",
                StartDate = new DateTime(2026, 5, 20),
                EndDate = new DateTime(2026, 7, 20),
                Timing = "Mon-Fri 6:00 AM - 8:00 AM IST",
                MeetingLink = "https://meet.google.com/abc-defg-hij",
                SeatsTotal = 15,
                IsActive = true
            });

            batches.Add(new Batch
            {
                CourseId = course.Id,
                BatchName = "May 2026 - Evening Batch",
                StartDate = new DateTime(2026, 5, 20),
                EndDate = new DateTime(2026, 7, 20),
                Timing = "Mon-Fri 8:00 PM - 10:00 PM IST",
                MeetingLink = "https://meet.google.com/xyz-uvwx-rst",
                SeatsTotal = 15,
                IsActive = true
            });

            batches.Add(new Batch
            {
                CourseId = course.Id,
                BatchName = "June 2026 - Weekend Batch",
                StartDate = new DateTime(2026, 6, 1),
                EndDate = new DateTime(2026, 8, 1),
                Timing = "Sat-Sun 10:00 AM - 2:00 PM IST",
                MeetingLink = "https://meet.google.com/klm-nopq-rst",
                SeatsTotal = 15,
                IsActive = true
            });
        }

        context.Batches.AddRange(batches);
        await context.SaveChangesAsync();

        // Seed Admin User
        var adminUser = new User
        {
            Name = "Admin User",
            Email = "admin@devfasttrack.com",
            Phone = "9999999999",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        // Seed Sample Announcements
        var announcements = new List<Announcement>
        {
            new Announcement
            {
                Title = "🎓 Perfect for Non-IT Students!",
                Description = "ECE, Mechanical, Civil students - Learn Python & coding in just 2 hours! Our Coding Confidence Workshop is designed specifically for absolute beginners. 90% OFF - Was ₹999, Now just ₹99!",
                CourseId = null
            },
            new Announcement
            {
                Title = "🐍 Python Workshop This Weekend!",
                Description = "No coding experience? No problem! Join our beginner-friendly workshop and write your first Python program. Perfect for non-IT students looking to enter tech. Limited seats!",
                CourseId = courses[0].Id
            },
            new Announcement
            {
                Title = "Welcome to Maari Training!",
                Description = "Start your tech journey today! Whether you're from IT or non-IT background, we have courses designed for everyone. Check out our beginner-friendly workshops!",
                CourseId = null
            }
        };

        context.Announcements.AddRange(announcements);
        await context.SaveChangesAsync();
    }
}
