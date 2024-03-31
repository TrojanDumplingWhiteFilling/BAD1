﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("OrderBakingGood")]
    public class OrderBakingGood
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Key]
        [Required]
        public int BakingGoodId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        public BakingGood? BakingGood { get; set; }
        public Order? Order { get; set; }
    }
}
