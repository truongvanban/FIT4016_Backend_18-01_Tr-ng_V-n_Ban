using FIT4016_KiemTra_2026.Data;
using FIT4016_KiemTra_2026.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace FIT4016_KiemTra_2026.Services
{
        public class StudentService
        {
            private readonly ApplicationDbContext _context;

            public StudentService(ApplicationDbContext context)
            {
                _context = context;
            }

            // 1. CREATE - Thêm học sinh mới
            public (bool Success, string Message) CreateStudent(Student student)
            {
                try
                {
                    // Validate dữ liệu
                    var validationResult = ValidateStudent(student, isUpdate: false);
                    if (!validationResult.Success)
                        return validationResult;

                    // Kiểm tra Student ID đã tồn tại chưa
                    if (_context.Students.Any(s => s.StudentId == student.StudentId))
                        return (false, "Student ID already exists.");

                    // Kiểm tra Email đã tồn tại chưa
                    if (_context.Students.Any(s => s.Email == student.Email))
                        return (false, "Email already exists.");

                    // Kiểm tra School có tồn tại không
                    if (!_context.Schools.Any(s => s.Id == student.SchoolId))
                        return (false, "Selected school does not exist.");

                    // Thiết lập thời gian
                    student.CreatedAt = DateTime.UtcNow;
                    student.UpdatedAt = DateTime.UtcNow;

                    // Thêm vào database
                    _context.Students.Add(student);
                    _context.SaveChanges();

                    return (true, "Student created successfully!");
                }
                catch (Exception ex)
                {
                    return (false, $"Error: {ex.Message}");
                }
            }

            // 2. READ - Lấy danh sách học sinh (có phân trang)
            public (List<Student> Students, int TotalPages, int TotalCount)
                GetStudents(int page = 1, int pageSize = 10)
            {
                try
                {
                    var query = _context.Students
                        .Include(s => s.School)
                        .OrderBy(s => s.FullName);

                    var totalCount = query.Count();
                    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    var students = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    return (students, totalPages, totalCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return (new List<Student>(), 0, 0);
                }
            }

            // 3. READ - Lấy học sinh theo ID
            public Student? GetStudentById(int id)
            {
                try
                {
                    return _context.Students
                        .Include(s => s.School)
                        .FirstOrDefault(s => s.Id == id);
                }
                catch
                {
                    return null;
                }
            }

            // 4. UPDATE - Cập nhật học sinh
            public (bool Success, string Message) UpdateStudent(Student student)
            {
                try
                {
                    // Validate dữ liệu
                    var validationResult = ValidateStudent(student, isUpdate: true);
                    if (!validationResult.Success)
                        return validationResult;

                    // Tìm học sinh hiện tại
                    var existingStudent = _context.Students.Find(student.Id);
                    if (existingStudent == null)
                        return (false, "Student not found.");

                    // Kiểm tra Email trùng (trừ chính nó)
                    if (_context.Students.Any(s => s.Email == student.Email && s.Id != student.Id))
                        return (false, "Email already exists.");

                    // Kiểm tra School có tồn tại không
                    if (!_context.Schools.Any(s => s.Id == student.SchoolId))
                        return (false, "Selected school does not exist.");

                    // Cập nhật thông tin
                    existingStudent.FullName = student.FullName;
                    existingStudent.Email = student.Email;
                    existingStudent.Phone = student.Phone;
                    existingStudent.SchoolId = student.SchoolId;
                    existingStudent.UpdatedAt = DateTime.UtcNow;

                    _context.SaveChanges();
                    return (true, "Student updated successfully!");
                }
                catch (Exception ex)
                {
                    return (false, $"Error: {ex.Message}");
                }
            }

            // 5. DELETE - Xóa học sinh
            public (bool Success, string Message) DeleteStudent(int id)
            {
                try
                {
                    var student = _context.Students.Find(id);
                    if (student == null)
                        return (false, "Student not found.");

                    _context.Students.Remove(student);
                    _context.SaveChanges();

                    return (true, "Student deleted successfully!");
                }
                catch (Exception ex)
                {
                    return (false, $"Error: {ex.Message}");
                }
            }

            // 6. Lấy tất cả trường học (cho dropdown)
            public List<School> GetAllSchools()
            {
                try
                {
                    return _context.Schools.OrderBy(s => s.Name).ToList();
                }
                catch
                {
                    return new List<School>();
                }
            }

            // 7. Validate dữ liệu học sinh
            private (bool Success, string Message) ValidateStudent(Student student, bool isUpdate)
            {
                // Validate Full Name
                if (string.IsNullOrWhiteSpace(student.FullName))
                    return (false, "Full name is required.");
                if (student.FullName.Length < 2 || student.FullName.Length > 100)
                    return (false, "Full name must be 2-100 characters.");

                // Validate Student ID (chỉ khi tạo mới)
                if (!isUpdate)
                {
                    if (string.IsNullOrWhiteSpace(student.StudentId))
                        return (false, "Student ID is required.");
                    if (student.StudentId.Length < 5 || student.StudentId.Length > 20)
                        return (false, "Student ID must be 5-20 characters.");
                }

                // Validate Email
                if (string.IsNullOrWhiteSpace(student.Email))
                    return (false, "Email is required.");

                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(student.Email))
                    return (false, "Invalid email format.");

                // Validate Phone (optional)
                if (!string.IsNullOrWhiteSpace(student.Phone))
                {
                    var phoneDigits = new string(student.Phone.Where(char.IsDigit).ToArray());
                    if (phoneDigits.Length < 10 || phoneDigits.Length > 11)
                        return (false, "Phone must be 10-11 digits.");
                }

                // Validate School
                if (student.SchoolId <= 0)
                    return (false, "Please select a school.");

                return (true, "Valid");
            }
        }
    }
