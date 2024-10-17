using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; }
    }

}
