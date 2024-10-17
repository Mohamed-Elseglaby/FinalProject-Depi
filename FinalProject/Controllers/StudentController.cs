using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;
        private readonly ApplicationContext _context;

        // Constructor to inject dependencies
        public StudentController(UserManager<ApplicationUser> userManager, IProfileService profileService, ApplicationContext context)
        {
            _userManager = userManager;
            _profileService = profileService;
            _context = context;
        }

        // GET: Student/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user); // Passes the user model to the view
        }

        // POST: Student/UpdateProfileImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileImage(IFormFile ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                try
                {
                    // Upload the new profile image and update the user's profile
                    user.Image_URL = await _profileService.UploadProfileImage(ProfileImage, user.Id, user.Image_URL);
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Profile image updated successfully!";
                        return RedirectToAction("Profile");
                    }

                    // Add validation errors to the model state
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ProfileImage", ex.Message);
                    return View("Profile", user);
                }
            }
            else
            {
                ModelState.AddModelError("ProfileImage", "Please select an image to upload.");
            }

            return View("Profile", user); // Return the profile view with errors if upload failed
        }




        // GET: Student/Index
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

        // GET: Student/ShowCourse
        //public async Task<IActionResult> ShowCourse(int courseId)
        //{
        //    // Find the course by ID and include the instructor data
        //    var course = await _context.Courses
        //        .Include(c => c.Category)
        //        .Include(c => c.InstructorCourse)
        //        .ThenInclude(ic => ic.User)
        //        .FirstOrDefaultAsync(c => c.CourseId == courseId);

        //    if (course == null)
        //    {
        //        return NotFound("Course not found.");
        //    }

        //    var instructorCourse = course.InstructorCourse.FirstOrDefault();
        //    if (instructorCourse == null)
        //    {
        //        return NotFound("No instructor found for this course.");
        //    }

        //    var instructor = await _userManager.FindByIdAsync(instructorCourse.UserId);
        //    if (instructor == null)
        //    {
        //        return NotFound("Instructor not found.");
        //    }
        //    ViewBag.CourseTitle = course.Title;
        //    ViewBag.CourseDescription = course.Description;
        //    ViewBag.InstructorName = instructor.UserName;
        //    ViewBag.CategoryName=course.Category.Name;
        //    ViewBag.CourseId = courseId;
        //    ViewBag.Price=course.Price;
        //    return View();
        //}


        public IActionResult ShowCourse(int courseId)
        {
            Course selectedcourse = _context.Courses.Include(c=>c.Feedbacks).Include(c => c.Category).FirstOrDefault(c => c.Id == courseId);
            if (selectedcourse != null)
            {
                return View("ShowCourse", selectedcourse);
            }
            return RedirectToAction("Index");
        }


        //Enroll
        public IActionResult Enroll(int courseId)
        {
            // Retrieve the course based on the given courseId
            Course selectedCourse = _context.Courses
                                           .FirstOrDefault(c => c.Id == courseId);

            // Check if the course exists
            if (selectedCourse == null)
            {
                return NotFound("Course not found.");
            }

            // Return the view with the selected course as the model
            return View(selectedCourse);
        }






        // Add feedback
        [HttpPost]
        public IActionResult AddFeedback(int courseId, string comment, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("", "Rating must be between 1 and 5.");
                // Reload course and feedbacks to return to the same view
                var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
                if (course == null)
                {
                    return NotFound();
                }

                var feedbacks = _context.Feedbacks
                                        .Where(f => f.CourseId == courseId)
                                        .Include(f => f.User)
                                        .ToList();
                ViewBag.Feedbacks = feedbacks;
                return View("Add", course);
            }

            var feedback = new Feedback
            {
                CourseId = courseId,
                Comment = comment,
                Rating = rating,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Assumes user is logged in
            };

            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();

            // Reload the course and feedbacks to return to the same page
            var courseWithFeedbacks = _context.Courses
                                              .Include(c => c.Feedbacks)
                                              .FirstOrDefault(c => c.Id == courseId);

            if (courseWithFeedbacks == null)
            {
                return NotFound();
            }

            var feedbackList = _context.Feedbacks
                                       .Where(f => f.CourseId == courseId)
                                       .Include(f => f.User)
                                       .ToList();
            ViewBag.Feedbacks = feedbackList;

            // Return to the Add view with course details and feedbacks
            return View("ShowCourse", courseWithFeedbacks);
        }









    }
}
