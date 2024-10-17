using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Reference to the ApplicationUser
        public ApplicationUser User { get; set; }  // Can be either Instructor or Student
    }

}
