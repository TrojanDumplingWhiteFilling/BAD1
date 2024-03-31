using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("Batch")]
    public class Batch
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        public DateTime ActualEndTime { get; set; }
        public ICollection<BakingGoodBatch>? BakingGoodBatches { get; set; }
        public ICollection<BatchStock>? BatchStocks { get; set; }
    }
}
