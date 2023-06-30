using MagistriMVC.Services;
using MagistriMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagistriMVC.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class GradesController : Controller
    {
        GradeService service;
        public GradesController(GradeService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            var allGrades = await service.GetAllAsync();
            return View(allGrades);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            var gradesDropdownsData = await service.GetGradesDropdownsValues();
            ViewBag.Students = new SelectList(gradesDropdownsData.Students, "Id", "LastName");
            ViewBag.Subjects = new SelectList(gradesDropdownsData.Subjects, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(GradesViewModel newGrade)
        {
            await service.CreateAsync(newGrade);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var gradeToEdit = await service.GetByIdAsync(id);
            if (gradeToEdit == null)
            {
                return View("NotFound");
            }
            var gradesDropdownsData = await service.GetGradesDropdownsValues();
            ViewBag.Students = new SelectList(gradesDropdownsData.Students, "Id", "LastName");
            ViewBag.Subjects = new SelectList(gradesDropdownsData.Subjects, "Id", "Name");
            var gradeVMtoEdit = new GradesViewModel()
            {
                Id = gradeToEdit.Id,
                Date = gradeToEdit.Date,
                Mark = gradeToEdit.Mark,
                StudentId = gradeToEdit.Student.Id,
                SubjectId = gradeToEdit.Subject.Id,
                What = gradeToEdit.What
            };
            return View(gradeVMtoEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, GradesViewModel updatedGrade)
        {
            await service.UpdateAsync(id, updatedGrade);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
