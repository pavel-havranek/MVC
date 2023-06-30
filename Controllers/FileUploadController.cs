using MagistriMVC.Models;
using MagistriMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml;

namespace MagistriMVC.Controllers {
    public class FileUploadController : Controller {
        StudentService studentService;

        public FileUploadController(StudentService studentService) {
            this.studentService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file) {
            string filePath = "";
            if (file.Length > 0) {
                filePath = Path.GetFullPath(file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                    stream.Close();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    XmlElement koren = xmlDoc.DocumentElement;
                    foreach (XmlNode node in koren.SelectNodes("/Students/Student")) {
                        Student s = new Student {
                            FirstName = node.ChildNodes[0].InnerText,
                            LastName = node.ChildNodes[1].InnerText,
                            DateOfBirth = DateTime.Parse(node.ChildNodes[2].InnerText, CultureInfo.CreateSpecificCulture("cs-CZ"))
                        };
                        await studentService.CreateAsync(s);
                    }
                }
                return RedirectToAction("Index", "Students");
            }
            else return View("NotFound");

        }
    }
}
