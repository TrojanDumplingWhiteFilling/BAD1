using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakery.Models
{
    // StockAllergen class for join table of Stock and Allergen
    [Table("StockAllergen")]
    public class StockAllergen
    {
        [Key]
        [Required]
        public int StockId { get; set; }

        [Key]
        [Required]
        public int AllergenId { get; set; }
        public Stock? Stock { get; set; }
        public Allergen? Allergen { get; set; }
    }
}