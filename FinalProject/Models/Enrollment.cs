using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Foreign key reference to ApplicationUser
        public ApplicationUser User { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; }

    }

}
