using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Question
    {
        
            [Key]
            public int QuestionId { get; set; }

            [Required]
            [MaxLength(100)]
            [MinLength(10)]
            public string Text { get; set; }

            [ForeignKey("Exam")]
            public int ExamId { get; set; }
            public Exam Exam { get; set; }

            public ICollection<Answer> Answers { get; set; }

            [ForeignKey("ApplicationUser")]
            public string UserId { get; set; }  // Reference to the ApplicationUser (Instructor)
            public ApplicationUser User { get; set; }
        }
    }


