using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Domain.Enums;
using PlatformManagementSystem.Infrastructure.Persistence;

namespace PlatformManagementSystem.Infrastructure.Identity
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Already seeded?
            if (await context.Categories.AnyAsync())
                return;

            // Roles 
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // Fix admin user to have the Admin role
            var adminUser = await userManager.FindByEmailAsync("admin@test.com");
            if (adminUser != null)
            {
                if (await userManager.IsInRoleAsync(adminUser, "Instructor"))
                    await userManager.RemoveFromRoleAsync(adminUser, "Instructor");

                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            await CreateUserAsync(userManager,
             "admin@test.com",
             "System Admin",
             "Admin@123",
             "Admin");
            // ─── Instructors ──────────────────────────────────────────────────────
            var instructor1 = await CreateUserAsync(userManager, "ahmed.hassan@edu.com", "Ahmed Hassan", "Instructor@123", "Instructor");
            var instructor2 = await CreateUserAsync(userManager, "sara.mohamed@edu.com", "Sara Mohamed", "Instructor@123", "Instructor");

            if (instructor1 == null || instructor2 == null)
                return;

            // ─── Students ────────────────────────────────────────────────────────
            var studentData = new[]
            {
            ("ali.mahmoud@student.com",   "Ali Mahmoud"),
            ("mona.hassan@student.com",   "Mona Hassan"),
            ("omar.ahmed@student.com",    "Omar Ahmed"),
            ("layla.ibrahim@student.com", "Layla Ibrahim"),
            ("hassan.youssef@student.com","Hassan Youssef")
        };

            var students = new List<ApplicationUser>();
            foreach (var (email, name) in studentData)
            {
                var s = await CreateUserAsync(userManager, email, name, "Student@123", "Student");
                if (s != null) students.Add(s);
            }

            // ─── Categories ──────────────────────────────────────────────────────
            var categories = new List<Category>
        {
            new() { Name = "Programming" },
            new() { Name = "Web Design" },
            new() { Name = "Data Science" },
            new() { Name = "Business" },
            new() { Name = "Languages" }
        };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Courses 
            var courses = new List<Course>
        {
            new()
            {
                Title       = "C# & .NET Fundamentals",
                Description = "Master the fundamentals of C# programming and the .NET ecosystem. This course covers variables, OOP, LINQ, async/await, and building real-world applications from scratch. Perfect for beginners and developers switching from other languages.",
                Price       = 299,
                ThumbnailUrl = "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?w=600&auto=format&fit=crop",
                InstructorId = instructor1.Id,
                CategoryId   = categories[0].Id,
                Status       = CourseStatus.Approved
            },
            new()
            {
                Title       = "Web Design with HTML, CSS & Bootstrap",
                Description = "Learn modern web design from the ground up. Build beautiful, responsive websites using HTML5, CSS3, Flexbox, Grid, and Bootstrap 5. You will create real-world layouts and learn design best practices along the way.",
                Price       = 199,
                ThumbnailUrl = "https://images.unsplash.com/photo-1547658719-da2b51169166?w=600&auto=format&fit=crop",
                InstructorId = instructor2.Id,
                CategoryId   = categories[1].Id,
                Status       = CourseStatus.Approved
            },
            new()
            {
                Title       = "Python for Data Science",
                Description = "Dive into Python programming with a focus on data analysis and visualization. Learn NumPy, Pandas, Matplotlib, and Seaborn to transform raw data into actionable insights that drive decisions.",
                Price       = 349,
                ThumbnailUrl = "https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=600&auto=format&fit=crop",
                InstructorId = instructor1.Id,
                CategoryId   = categories[2].Id,
                Status       = CourseStatus.Approved
            },
            new()
            {
                Title       = "Excel for Business Analytics",
                Description = "Unlock the full power of Microsoft Excel for business decision-making. Learn pivot tables, advanced formulas, data visualization, dashboards, and Power Query to become the Excel expert in your workplace.",
                Price       = 149,
                ThumbnailUrl = "https://images.unsplash.com/photo-1543286386-713bdd548da4?w=600&auto=format&fit=crop",
                InstructorId = instructor2.Id,
                CategoryId   = categories[3].Id,
                Status       = CourseStatus.Approved
            }
        };
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            //
            // ─── Lessons ─────────────────────────────────────────────────────────
            var lessons = new List<Lesson>
        {
            // Course 1: C# .NET
            new() { CourseId = courses[0].Id, Title = "Introduction to C# and .NET", Content = "Welcome to the course! In this lesson we explore the history of C# and the .NET ecosystem, understand how the CLR works, and set up Visual Studio 2022 with your first Hello World application." },
            new() { CourseId = courses[0].Id, Title = "Variables, Data Types & Operators", Content = "Learn the building blocks of C#: value types (int, double, bool, struct), reference types (string, object, class), operators, and type conversions. Practice with short coding exercises." },
            new() { CourseId = courses[0].Id, Title = "Object-Oriented Programming", Content = "Deep dive into classes, objects, constructors, inheritance, polymorphism, encapsulation, and interfaces. OOP is the heart of C# — master it here with real-world analogies." },
            new() { CourseId = courses[0].Id, Title = "LINQ & Collections", Content = "Master Language Integrated Query (LINQ) to query and transform data collections. Learn List<T>, Dictionary<K,V>, IEnumerable, and lambda expressions with hands-on examples." },
            new() { CourseId = courses[0].Id, Title = "Async & Await in C#", Content = "Understand asynchronous programming with async/await, Task, and CancellationToken. Write non-blocking, high-performance .NET applications ready for production." },

            // Course 2: Web Design
            new() { CourseId = courses[1].Id, Title = "HTML5 Structure & Semantics", Content = "Learn the semantic elements of HTML5: header, nav, main, section, article, and footer. Build well-structured, accessible web pages that search engines love." },
            new() { CourseId = courses[1].Id, Title = "CSS3 Styling & Selectors", Content = "Master CSS3 selectors, the box model, colors, typography, shadows, and transitions. Style your HTML pages with confidence and precision." },
            new() { CourseId = courses[1].Id, Title = "Flexbox & CSS Grid", Content = "Build modern, responsive layouts using CSS Flexbox and Grid. Understand when to use each system and create designs that work perfectly on all screen sizes." },
            new() { CourseId = courses[1].Id, Title = "Bootstrap 5 Components", Content = "Speed up your development with Bootstrap 5. Use the 12-column grid, ready-made components like cards, modals, navbars, and utility classes to ship faster." },

            // Course 3: Python DS
            new() { CourseId = courses[2].Id, Title = "Python Basics & Setup", Content = "Install Python 3 and set up Jupyter Notebooks. Learn Python syntax, variables, data types, lists, dictionaries, and control flow with interactive examples." },
            new() { CourseId = courses[2].Id, Title = "NumPy for Numerical Computing", Content = "Learn NumPy arrays, array operations, indexing, slicing, broadcasting, and the mathematical functions you'll need for every data science project." },
            new() { CourseId = courses[2].Id, Title = "Pandas for Data Analysis", Content = "Master DataFrames and Series. Load datasets from CSV and Excel, clean messy data, filter rows, group-by aggregations, and merge tables efficiently." },
            new() { CourseId = courses[2].Id, Title = "Data Visualization with Matplotlib & Seaborn", Content = "Create compelling visualizations: line plots, bar charts, histograms, scatter plots, box plots, and heatmaps. Communicate insights clearly to any audience." },

            // Course 4: Excel
            new() { CourseId = courses[3].Id, Title = "Excel Interface & Navigation", Content = "Get familiar with Excel's ribbon, worksheets, cells, and keyboard shortcuts. Learn how to navigate large spreadsheets and manage workbooks like a professional." },
            new() { CourseId = courses[3].Id, Title = "Formulas & Functions", Content = "Master SUM, AVERAGE, IF, VLOOKUP, INDEX/MATCH, and COUNTIF. Build dynamic spreadsheets that update automatically and handle real business scenarios." },
            new() { CourseId = courses[3].Id, Title = "Pivot Tables & Business Charts", Content = "Summarize thousands of rows instantly with Pivot Tables. Create professional charts and dashboards that turn raw numbers into executive-ready presentations." }
        };
            context.Lessons.AddRange(lessons);
            await context.SaveChangesAsync();

            // ─── Exams ───────────────────────────────────────────────────────────
            var exams = new List<Exam>
        {
            new() { Title = "C# Basics Quiz",                  CourseId = courses[0].Id, DurationMinutes = 30 },
            new() { Title = "Web Design Fundamentals Quiz",    CourseId = courses[1].Id, DurationMinutes = 20 },
            new() { Title = "Python Data Science Quiz",        CourseId = courses[2].Id, DurationMinutes = 25 },
            new() { Title = "Excel Analytics Quiz",            CourseId = courses[3].Id, DurationMinutes = 20 }
        };
            context.Exams.AddRange(exams);
            await context.SaveChangesAsync();

            // ─── Questions ───────────────────────────────────────────────────────
            var questions = new List<Question>
        {
            // Exam 1 – C#
            new() { ExamId = exams[0].Id, Text = "What keyword is used to define a class in C#?" },
            new() { ExamId = exams[0].Id, Text = "Which of the following is a value type in C#?" },
            new() { ExamId = exams[0].Id, Text = "What does the 'async' keyword enable in C#?" },
            new() { ExamId = exams[0].Id, Text = "Which LINQ method returns the first matching element or null?" },
            new() { ExamId = exams[0].Id, Text = "What is the base class of all classes in C#?" },

            // Exam 2 – Web Design
            new() { ExamId = exams[1].Id, Text = "Which HTML tag defines the main content of a document?" },
            new() { ExamId = exams[1].Id, Text = "What CSS property controls the text color?" },
            new() { ExamId = exams[1].Id, Text = "Which Flexbox property aligns items along the main axis?" },
            new() { ExamId = exams[1].Id, Text = "What Bootstrap class makes a column span 6 of 12 grid columns?" },

            // Exam 3 – Python
            new() { ExamId = exams[2].Id, Text = "Which Python library provides the DataFrame data structure?" },
            new() { ExamId = exams[2].Id, Text = "What Pandas function reads a CSV file into a DataFrame?" },
            new() { ExamId = exams[2].Id, Text = "Which NumPy function creates an array filled with zeros?" },
            new() { ExamId = exams[2].Id, Text = "What is the output of len([1, 2, 3]) in Python?" },

            // Exam 4 – Excel
            new() { ExamId = exams[3].Id, Text = "Which Excel function looks up a value in the first column of a range?" },
            new() { ExamId = exams[3].Id, Text = "What Excel feature summarizes large datasets without formulas?" },
            new() { ExamId = exams[3].Id, Text = "Which formula counts only cells that meet a given condition?" }
        };
            context.Questions.AddRange(questions);
            await context.SaveChangesAsync();

            // ─── Answers ─────────────────────────────────────────────────────────
            var answers = new List<Answer>
        {
            // Q0 – class keyword
            new() { QuestionId = questions[0].Id, Text = "class",   IsCorrect = true  },
            new() { QuestionId = questions[0].Id, Text = "struct",  IsCorrect = false },
            new() { QuestionId = questions[0].Id, Text = "object",  IsCorrect = false },
            new() { QuestionId = questions[0].Id, Text = "type",    IsCorrect = false },

            // Q1 – value type
            new() { QuestionId = questions[1].Id, Text = "int",     IsCorrect = true  },
            new() { QuestionId = questions[1].Id, Text = "string",  IsCorrect = false },
            new() { QuestionId = questions[1].Id, Text = "List<T>", IsCorrect = false },
            new() { QuestionId = questions[1].Id, Text = "object",  IsCorrect = false },

            // Q2 – async
            new() { QuestionId = questions[2].Id, Text = "Asynchronous programming",  IsCorrect = true  },
            new() { QuestionId = questions[2].Id, Text = "Parallel thread creation",  IsCorrect = false },
            new() { QuestionId = questions[2].Id, Text = "Synchronous locking",       IsCorrect = false },
            new() { QuestionId = questions[2].Id, Text = "Memory management",         IsCorrect = false },

            // Q3 – FirstOrDefault
            new() { QuestionId = questions[3].Id, Text = "FirstOrDefault()", IsCorrect = true  },
            new() { QuestionId = questions[3].Id, Text = "Single()",         IsCorrect = false },
            new() { QuestionId = questions[3].Id, Text = "Find()",           IsCorrect = false },
            new() { QuestionId = questions[3].Id, Text = "Select()",         IsCorrect = false },

            // Q4 – base class
            new() { QuestionId = questions[4].Id, Text = "object", IsCorrect = true  },
            new() { QuestionId = questions[4].Id, Text = "Base",   IsCorrect = false },
            new() { QuestionId = questions[4].Id, Text = "Entity", IsCorrect = false },
            new() { QuestionId = questions[4].Id, Text = "System", IsCorrect = false },

            // Q5 – <main>
            new() { QuestionId = questions[5].Id, Text = "<main>",    IsCorrect = true  },
            new() { QuestionId = questions[5].Id, Text = "<body>",    IsCorrect = false },
            new() { QuestionId = questions[5].Id, Text = "<div>",     IsCorrect = false },
            new() { QuestionId = questions[5].Id, Text = "<section>", IsCorrect = false },

            // Q6 – color
            new() { QuestionId = questions[6].Id, Text = "color",       IsCorrect = true  },
            new() { QuestionId = questions[6].Id, Text = "text-color",  IsCorrect = false },
            new() { QuestionId = questions[6].Id, Text = "font-color",  IsCorrect = false },
            new() { QuestionId = questions[6].Id, Text = "foreground",  IsCorrect = false },

            // Q7 – justify-content
            new() { QuestionId = questions[7].Id, Text = "justify-content",  IsCorrect = true  },
            new() { QuestionId = questions[7].Id, Text = "align-items",      IsCorrect = false },
            new() { QuestionId = questions[7].Id, Text = "flex-direction",   IsCorrect = false },
            new() { QuestionId = questions[7].Id, Text = "align-content",    IsCorrect = false },

            // Q8 – col-6
            new() { QuestionId = questions[8].Id, Text = "col-6",      IsCorrect = true  },
            new() { QuestionId = questions[8].Id, Text = "col-half",   IsCorrect = false },
            new() { QuestionId = questions[8].Id, Text = "col-50",     IsCorrect = false },
            new() { QuestionId = questions[8].Id, Text = "span-6",     IsCorrect = false },

            // Q9 – Pandas
            new() { QuestionId = questions[9].Id, Text = "Pandas",      IsCorrect = true  },
            new() { QuestionId = questions[9].Id, Text = "NumPy",       IsCorrect = false },
            new() { QuestionId = questions[9].Id, Text = "Matplotlib",  IsCorrect = false },
            new() { QuestionId = questions[9].Id, Text = "Scikit-learn",IsCorrect = false },

            // Q10 – read_csv
            new() { QuestionId = questions[10].Id, Text = "pd.read_csv()",    IsCorrect = true  },
            new() { QuestionId = questions[10].Id, Text = "pd.load_csv()",    IsCorrect = false },
            new() { QuestionId = questions[10].Id, Text = "pd.open_csv()",    IsCorrect = false },
            new() { QuestionId = questions[10].Id, Text = "pd.import_csv()",  IsCorrect = false },

            // Q11 – np.zeros
            new() { QuestionId = questions[11].Id, Text = "np.zeros()", IsCorrect = true  },
            new() { QuestionId = questions[11].Id, Text = "np.empty()", IsCorrect = false },
            new() { QuestionId = questions[11].Id, Text = "np.null()",  IsCorrect = false },
            new() { QuestionId = questions[11].Id, Text = "np.clear()", IsCorrect = false },

            // Q12 – len([1,2,3]) = 3
            new() { QuestionId = questions[12].Id, Text = "3",     IsCorrect = true  },
            new() { QuestionId = questions[12].Id, Text = "2",     IsCorrect = false },
            new() { QuestionId = questions[12].Id, Text = "4",     IsCorrect = false },
            new() { QuestionId = questions[12].Id, Text = "Error", IsCorrect = false },

            // Q13 – VLOOKUP
            new() { QuestionId = questions[13].Id, Text = "VLOOKUP", IsCorrect = true  },
            new() { QuestionId = questions[13].Id, Text = "SUMIF",   IsCorrect = false },
            new() { QuestionId = questions[13].Id, Text = "MATCH",   IsCorrect = false },
            new() { QuestionId = questions[13].Id, Text = "FIND",    IsCorrect = false },

            // Q14 – Pivot Table
            new() { QuestionId = questions[14].Id, Text = "Pivot Table", IsCorrect = true  },
            new() { QuestionId = questions[14].Id, Text = "Chart",       IsCorrect = false },
            new() { QuestionId = questions[14].Id, Text = "Filter",      IsCorrect = false },
            new() { QuestionId = questions[14].Id, Text = "Sort",        IsCorrect = false },

            // Q15 – COUNTIF
            new() { QuestionId = questions[15].Id, Text = "COUNTIF",   IsCorrect = true  },
            new() { QuestionId = questions[15].Id, Text = "COUNT",      IsCorrect = false },
            new() { QuestionId = questions[15].Id, Text = "SUMIF",      IsCorrect = false },
            new() { QuestionId = questions[15].Id, Text = "AVERAGEIF",  IsCorrect = false }
        };
            context.Answers.AddRange(answers);
            await context.SaveChangesAsync();

            // ─── Enrollments ─────────────────────────────────────────────────────
            if (students.Count >= 5)
            {
                var enrollments = new List<Enrollment>
            {
                new() { StudentId = students[0].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-30), ProgressPercentage = 80,  IsCompleted = false },
                new() { StudentId = students[0].Id, CourseId = courses[2].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-15), ProgressPercentage = 40,  IsCompleted = false },
                new() { StudentId = students[1].Id, CourseId = courses[1].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-20), ProgressPercentage = 100, IsCompleted = true  },
                new() { StudentId = students[1].Id, CourseId = courses[3].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-10), ProgressPercentage = 60,  IsCompleted = false },
                new() { StudentId = students[2].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-25), ProgressPercentage = 100, IsCompleted = true  },
                new() { StudentId = students[2].Id, CourseId = courses[1].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-12), ProgressPercentage = 50,  IsCompleted = false },
                new() { StudentId = students[3].Id, CourseId = courses[2].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-8),  ProgressPercentage = 20,  IsCompleted = false },
                new() { StudentId = students[4].Id, CourseId = courses[3].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-5),  ProgressPercentage = 10,  IsCompleted = false }
            };
                context.Enrollments.AddRange(enrollments);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser?> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email, string fullName, string password, string role)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                if (!await userManager.IsInRoleAsync(existing, role))
                    await userManager.AddToRoleAsync(existing, role);
                return existing;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) return null;

            await userManager.AddToRoleAsync(user, role);
            return user;
        }
    }
}