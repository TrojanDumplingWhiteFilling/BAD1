using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bakery.Attributes;
using System.Text.Json.Serialization;
using Bakery.Models;

namespace Bakery.DTO
{
    public class BakedGoodQuantityDTO
    {
        public string? BakedGood { get; set; }
        public int Quantity { get; set; }
    }
}

