using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bakery.Attributes;
using System.Text.Json.Serialization;
using Bakery.Models;

namespace Bakery.DTO
{
    public class StockDTO
    {
        public string? Ingredient { get; set; }
        public int Quantity { get; set; }
        public List<string>? Allergens { get; set; }
    }
}


