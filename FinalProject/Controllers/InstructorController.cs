using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;


namespace FinalProject.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public InstructorController(UserManager<ApplicationUser> userManager, IProfileService profileService, ApplicationContext context, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _profileService = profileService;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: InstructorProfile
        public async Task<IActionResult> InstructorProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = await _context.Courses
                .Where(c => c.InstructorCourse.Any(ic => ic.UserId == user.Id))
                .ToListAsync();

            ViewBag.Courses = courses;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("Description")] ApplicationUser model, IFormFile ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Update the description only if it's not empty
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                user.Description = model.Description;
            }

            // Handle Profile Image Upload
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                try
                {
                    user.Image_URL = await _profileService.UploadProfileImage(ProfileImage, user.Id, user.Image_URL);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ProfileImage", ex.Message);
                    return View("InstructorProfile", user);
                }
            }

            // Update the user in the database
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("InstructorProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("InstructorProfile", user);
        }






        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var courses = await _context.Courses
        //        .Where(c => c.InstructorCourse.Any(ic => ic.UserId == user.Id))
        //        .ToListAsync();

        //    return View(courses);
        //}
        [HttpPost]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Log if user is null
                Console.WriteLine("User is null");
                return Json(new { success = false });
            }

            // Remove the image URL from the user profile
            user.Image_URL = null;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Log success
                Console.WriteLine("Profile image deleted successfully");

            }

            // Log failure
            Console.WriteLine("Failed to delete profile image");
            return RedirectToAction("EditProfile", "Student");

        }
        public async Task<IActionResult> Index()
        {
            var co = _context.Courses;
            return View(await co.ToListAsync());
        }


        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DurationTime,VideoURL,ImageURL,CategoryId")] Course course)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InstructorProfile));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DurationTime,VideoURL,ImageURL,CategoryId")] Course course)
        {
            if (id != course.Id)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                  
                }
                return RedirectToAction(nameof(Create));
            }
            return View(course);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
               .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
    public class AddCourseViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<IFormFile> Files { get; set; }

        public List<Category> Categories { get; set; }

        // إضافة حقول الدرس
        public string LessonTitle { get; set; }
        public string LessonContent { get; set; }
    }


}
