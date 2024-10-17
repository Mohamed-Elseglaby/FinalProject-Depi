using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
 
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Foreign key reference to ApplicationUser
        public ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<Payment> Payments { get; set; } // Link payments to orders
    }

}
