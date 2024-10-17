
using FinalProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Course
    {
        [Key]


        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int DurationTime { get; set; }

        public string VideoURL { get; set; }
        public string ImageURL { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<InstructorCourse> InstructorCourse { get; set; }
    }
}
