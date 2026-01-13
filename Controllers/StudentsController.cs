using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FIT4016_KiemTra_2026.Models;
using FIT4016_KiemTra_2026.Services;

namespace FIT4016_KiemTra_2026.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var (students, totalPages, totalCount) = _studentService.GetStudents(page, pageSize);
            return View(students);
        }

        public IActionResult Create()
        {
            ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name", student.SchoolId);
                return View(student);
            }

            var result = _studentService.CreateStudent(student);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name", student.SchoolId);
                return View(student);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound();

            ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name", student.SchoolId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name", student.SchoolId);
                return View(student);
            }

            var result = _studentService.UpdateStudent(student);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewBag.Schools = new SelectList(_studentService.GetAllSchools(), "Id", "Name", student.SchoolId);
                return View(student);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var result = _studentService.DeleteStudent(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}