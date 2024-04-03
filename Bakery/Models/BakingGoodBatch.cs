using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakery.Models
{
    [Table("BakingGoodBatch")]
    public class BakingGoodBatch
    {
        [Key]
        [Required]
        public int BakingGoodId { get; set; }
        [Key]
        [Required]
        public int BatchId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public Batch? Batch { get; set; }
        public BakingGood? BakingGood { get; set; }
    }
}
