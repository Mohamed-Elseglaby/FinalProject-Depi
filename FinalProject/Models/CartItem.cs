using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public int Quantity { get; set; }
        public Course Course { get; set; }
        public Cart Cart { get; set; }

    }

}
