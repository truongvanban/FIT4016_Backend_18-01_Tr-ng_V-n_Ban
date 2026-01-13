using Bogus;
using FIT4016_KiemTra_2026.Data;
using FIT4016_KiemTra_2026.Models;
namespace FIT4016_KiemTra_2026.Services
{
    public class DatabaseSeeder
    {
        public static void SeedData()
        {
            using var context = new ApplicationDbContext();

            Console.WriteLine("Checking database...");

            // Đảm bảo database được tạo
            context.Database.EnsureCreated();

            // Kiểm tra nếu đã có dữ liệu
            if (context.Schools.Any())
            {
                Console.WriteLine("Database already has data.");
                return;
            }

            Console.WriteLine("Seeding data...");

            // Tạo dữ liệu giả cho School
            var schoolFaker = new Faker<School>()
                .RuleFor(s => s.Name, f => f.Company.CompanyName() + " High School")
                .RuleFor(s => s.Principal, f => f.Name.FullName())
                .RuleFor(s => s.Address, f => f.Address.FullAddress())
                .RuleFor(s => s.CreatedAt, f => f.Date.Past(2))
                .RuleFor(s => s.UpdatedAt, (f, s) => s.CreatedAt);

            var schools = schoolFaker.Generate(10);
            context.Schools.AddRange(schools);
            context.SaveChanges();

            // Tạo dữ liệu giả cho Student
            var studentFaker = new Faker<Student>()
                .RuleFor(s => s.FullName, f => f.Name.FullName())
                .RuleFor(s => s.StudentId, f => $"STU{f.Random.Number(1000, 9999):0000}")
                .RuleFor(s => s.Email, (f, s) => f.Internet.Email(s.FullName))
                .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(s => s.SchoolId, f => f.PickRandom(schools).Id)
                .RuleFor(s => s.CreatedAt, f => f.Date.Past(1))
                .RuleFor(s => s.UpdatedAt, (f, s) => s.CreatedAt);

            var students = studentFaker.Generate(20);
            context.Students.AddRange(students);
            context.SaveChanges();

            Console.WriteLine($"✅ Seeding completed!");
            Console.WriteLine($"   Schools created: {context.Schools.Count()}");
            Console.WriteLine($"   Students created: {context.Students.Count()}");
            Console.WriteLine();
        }
    }
}
