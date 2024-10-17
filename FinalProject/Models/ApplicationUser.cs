using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required(ErrorMessage = "Must enter name")]
        //[MaxLength(50, ErrorMessage = "Name must be less than 50 letters")]
        //[MinLength(3, ErrorMessage = "Name must be greater than 3 letters")]
        //public string Name { get; set; }

        [RegularExpression(@"\.(jpg|png)$", ErrorMessage = "Image must be png or jpg")]
        public string? Image_URL { get; set; }

        public string? Description { get; set; }  // Only for Instructors, can leave null for Students

        // Navigation properties if needed
        public ICollection<InstructorCourse>? InstructorCourses { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<Answer>? Answers { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Cart>? Carts { get; set; }
    }
}

