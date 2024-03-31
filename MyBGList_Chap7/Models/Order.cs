using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string DeliveryPlace { get; set; } = null!;

        [Required]
        public DateTime DeliveryDate { get; set; }

        // Navigation properties
        public ICollection<Packet>? Packets { get; set; }
        public ICollection<OrderBakingGood>? OrderBakingGoods { get; set; }
    }
}
