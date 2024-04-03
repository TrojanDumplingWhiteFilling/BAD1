using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakery.Models
{
    // Allergen class for Allergen table with id and name
    [Table("Allergen")]
    public class Allergen
    {
        [Key]
        [Required]
        public int AllergenId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AllergenName { get; set; } = null!;

        // Navigation properties
        public ICollection<StockAllergen>? StockAllergens { get; set; }
    }
}