using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Reference to the ApplicationUser
        public ApplicationUser User { get; set; }  // Will be a Student in this case
        public ICollection<CartItem> CartItems { get; set; }
    }

}
