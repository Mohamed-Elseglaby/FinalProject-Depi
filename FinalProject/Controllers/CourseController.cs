using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    public class CourseController : Controller
    {

        private readonly ApplicationContext _context;
        public CourseController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 6; // عدد الكورسات لكل صفحة
            var courses = _context.Courses.Include(u => u.InstructorCourse)
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList();

            int totalCourses = _context.Courses.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCourses / pageSize);
            ViewBag.CurrentPage = page;
            return View("Index", courses);
        }



        public IActionResult ShowCourse(int courseId)
        {
            Course selectedcourse = _context.Courses.Include(c => c.Category).FirstOrDefault(c => c.Id == courseId);
            if (selectedcourse != null)
            {
                return View("ShowCourse", selectedcourse);
            }
            return RedirectToAction("Index");
        }







    }
    

}

