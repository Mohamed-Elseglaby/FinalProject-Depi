using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }  // Rating out of 5

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        
        public Course Course { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Reference to the ApplicationUser
        public ApplicationUser User { get; set; }  // Will be a Student in this case
    }

}
