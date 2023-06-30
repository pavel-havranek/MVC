using MagistriMVC.Models;
using MagistriMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagistriMVC.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class SubjectsController : Controller
    {
        public SubjectService service;
        public SubjectsController(SubjectService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var allSubjects = await service.GetAllAsync();
            return View(allSubjects);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Subject newSubject)
        {
            if (ModelState.IsValid)
            {
                await service.CreateAsync(newSubject);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var SubjectToEdit = await service.GetByIdAsync(id);
            if (SubjectToEdit == null)
            {
                return View("NotFound");
            }
            return View(SubjectToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name")] Subject subject)
        {
            await service.UpdateAsync(id, subject);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var subjectToDelete = await service.GetByIdAsync(id);
            if (subjectToDelete == null)
            {
                return View("NotFound");
            }
            await service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
