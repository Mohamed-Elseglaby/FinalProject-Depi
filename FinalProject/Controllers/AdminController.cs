using FinalProject.Models;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationContext context;

        public AdminController(UserManager<ApplicationUser> userManager,ApplicationContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

      
        public async Task<IActionResult> AdminDashboard()
        {

            var user =await userManager.FindByIdAsync(User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier).Value);
            var userViewModel = new AdminVm
            {
                Phone=user.PhoneNumber,
                Name = user.UserName,
                Email = user.Email
            };
            var students = await userManager.GetUsersInRoleAsync("Student");

            
            var studentViewModels = students.Select(s => new StudentViewModel
            {
                Id = s.Id,
                Name = s.UserName,
                Email = s.Email,
                phone=s.PhoneNumber

            }).ToList();
            ViewBag.student=  studentViewModels;


            return View(userViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var deleteSudent = await userManager.FindByIdAsync(id);
            if (deleteSudent == null)
            {
                return NotFound("Student not found");

            }
            else
            {
                var result = await userManager.DeleteAsync(deleteSudent);
                if (result.Succeeded)
                {
                    return RedirectToAction("AdminDashboard");
                }
            }
            return View("AdminDashboard");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var Sudent = await userManager.FindByIdAsync(id);
            if (Sudent == null)
            {
                return NotFound("Student not found");

            }
            else
            {
                var studentViewModels = new StudentViewModel
                {
                    Id = Sudent.Id,
                    Name = Sudent.UserName,
                    Email = Sudent.Email,
                    phone = Sudent.PhoneNumber

                };
                return View(studentViewModels);
            }
            

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(string Id, StudentViewModel Student)
        {

            if (!ModelState.IsValid)
            {
                return View("Edit", Student);  
            }

            
            var student = await userManager.FindByIdAsync(Student.Id);
            if (student == null)
            {
                return Content("Student not found");
            }
            var newStudent = new StudentViewModel
            {
                Id = student.Id,
                Name = student.UserName,
                Email = student.Email,
                phone = student.PhoneNumber

            }; 
            
            student.UserName = Student.Name;
            student.Email = Student.Email;
            student.PhoneNumber = Student.phone;

            
            var result = await userManager.UpdateAsync(student);
            if (result.Succeeded)
            {
                
                return RedirectToAction("AdminDashboard");  // Redirect to the admin dashboard after a successful update
            }

            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Edit",Student);
        }
   //     public IActionResult Courses()
   //     {
   //         var course = context.Courses.ToList();

   //         return View(course);
   //     }
   //     [HttpGet]
   //     public IActionResult EditCourse(int Id)
   //     {
   //         var course = context.Courses.FirstOrDefault(c => c.Id == Id);
   //         return View(course);
   //     }
   //     [HttpPost]
   //     [ValidateAntiForgeryToken]
   //     public IActionResult SaveEditCourse(int Id, Course NewCourse)
   //     {
   //         var oldCourse = context.Courses.FirstOrDefault(c => c.Id == Id);
   //         if (oldCourse!=null)
   //         {
   //             oldCourse.Title = NewCourse.Title;
   //             oldCourse.Description = NewCourse.Description;
   //             oldCourse.Price = NewCourse.Price;
   //             context.SaveChanges();         
   //                 return RedirectToAction("Courses");
   //         }
   //         return View("EditCourse", oldCourse);
   //     }
   //     public IActionResult DeleteCourse(int Id)
   //     {
   //         var Course = context.Courses.FirstOrDefault(c => c.Id == Id);
   //         context.Remove(Course);
   //         context.SaveChanges();
   //         return RedirectToAction("Courses");
   //     }
   //     public IActionResult CreateCourse(Course course)
   //     {

   //         ViewData["courslist"] = context.Categories.ToList();
   //         return View(new Course());
   //     }
   //     public IActionResult SaveCreateCourse(Course course)
   //     {
   //         if (course != null)
   //         {
   //             context.Courses.Add(course);
   //             context.SaveChanges();
   //             return RedirectToAction("Courses");

   //         }
			//ViewData["courslist"] = context.Categories.ToList();
			//return View("CreateCourse" , course);
   //     }
        public IActionResult AssignRole() => View();
        [HttpPost]
        public async Task<IActionResult> AssignRole(string RoleName, string UserName)
		{
			var user = await userManager.FindByNameAsync(UserName);
            var ui = await userManager.RemoveFromRoleAsync(user,"Student");
			var result = await userManager.AddToRoleAsync(user, RoleName);
			return RedirectToAction("AdminDashboard");
		}

	}
}
