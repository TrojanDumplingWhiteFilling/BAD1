using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("BatchStock")]
    public class BatchStock
    {
        [Key]
        [Required]
        public int BatchId { get; set; }

        [Key]
        [Required]
        public int StockId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public Batch? Batch { get; set; }
        public Stock? Stock { get; set; }
    }
}
