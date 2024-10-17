using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;


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






        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Courses
                .Where(c => c.InstructorCourse.Any(ic => ic.UserId == user.Id))
                .ToListAsync();

            return View(courses);
        }

       [HttpGet]
        public IActionResult AddCourse()
        {
            ViewBag.Categories = _context.Categories.ToList(); // جلب الفئات من قاعدة البيانات
            return View(new Course());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddCourse(Course model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // الحصول على معرف المستخدم
                model.UserId = userId; // إضافة معرف المستخدم إلى الكورس

                // حفظ الكورس في قاعدة البيانات
                _context.Courses.Add(model);
                await _context.SaveChangesAsync();

                // إضافة الدرس المرتبط بالكورس
                var newLesson = new Lesson
                {
                    Title = model.LessonTitle, // من الحقل الموجود داخل الـ Course
                    Content = model.LessonContent, // من الحقل الموجود داخل الـ Course
                    CourseId = model.Id // استخدم معرف الكورس الجديد
                };
                _context.Lessons.Add(newLesson);

                await _context.SaveChangesAsync(); // حفظ الدرس

                return RedirectToAction("Index", "Instructor"); // إعادة التوجيه بعد الحفظ
            }

            // في حالة وجود أخطاء
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage); // طباعة الأخطاء إن وجدت
            }

            // إعادة تحميل الفئات إذا فشل النموذج
            model.Category = _context.Categories.FirstOrDefault(c => c.Id == model.CategoryId);
            return View("AddCourse", model); // إعادة عرض النموذج مع البيانات المدخلة
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
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
                await _context.SaveChangesAsync();
            }
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
