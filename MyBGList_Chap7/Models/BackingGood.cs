using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("BakingGood")]
    public class BakingGood
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        public DateTime DateProduced { get; set; }

        public ICollection<OrderBakingGood>? OrderBakingGoods { get; set; }
        public ICollection<BakingGoodBatch>? BakingGoodBatches { get; set; }
    }
}
