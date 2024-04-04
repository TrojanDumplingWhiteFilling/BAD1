using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Bakery.Models;
using Bakery.Models.Csv;
using MyBGList_Chap6.Data;
using Serilog.Filters;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Bakery.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SeedController> _logger;

        public SeedController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<SeedController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        [HttpPut(Name = "Seed")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Put()
        {
            try
            {
                // sample data #1
                var order1 = new Order()
                {
                    DeliveryPlace = "New Aarhus Gade 1",
                    //DeliveryDate = DateTime.Parse("2022-01-01 12:00")
                    DeliveryDate = "01012022 1200",
                    GPSCoordinates = "56.1234, 10.1234"
                };
                _context.Orders.Add(order1);

                var order2 = new Order()
                {
                    DeliveryPlace = "Old Aarhus Gade 2",
                    //DeliveryDate = DateTime.Parse("2022-01-02 08:00")
                    DeliveryDate = "02012022 0800",
                    GPSCoordinates = "52.1234, 10.1454"
                };
                _context.Orders.Add(order2);

                var bg1 = new BakingGood()
                {
                    Name = "Bread",
                    DateProduced = DateTime.Parse("2022-01-01")
                };
                var bg2 = new BakingGood()
                {
                    Name = "Cake",
                    DateProduced = DateTime.Parse("2022-01-02")
                };
                var bg3 = new BakingGood()
                {
                    Name = "Cookie",
                    DateProduced = DateTime.Parse("2022-01-03")
                };

                _context.BakingGoods.Add(bg1);
                _context.BakingGoods.Add(bg2);
                _context.BakingGoods.Add(bg3);

                var batch1 = new Batch()
                {
                    StartDateTime = DateTime.Parse("2022-01-01T08:00:00"),
                    EndDateTime = DateTime.Parse("2022-01-01T12:00:00"),
                    ActualEndTime = DateTime.Parse("2022-01-01T14:00:00")
                };
                var batch2 = new Batch()
                {
                    StartDateTime = DateTime.Parse("2022-01-02T08:00:00"),
                    EndDateTime = DateTime.Parse("2022-01-02T12:00:00"),
                    ActualEndTime = DateTime.Parse("2022-01-02T13:00:00")
                };

                _context.Batches.Add(batch1);

                var stock1 = new Stock()
                {
                    Name = "Flour",
                    Quantity = 100
                };
                var stock2 = new Stock()
                {
                    Name = "Sugar",
                    Quantity = 200
                };
                var stock3 = new Stock()
                {
                    Name = "Butter",
                    Quantity = 300
                };

                _context.Stocks.Add(stock1);
                _context.Stocks.Add(stock2);
                _context.Stocks.Add(stock3);

                // 3x packets
                var packet1 = new Packet()
                {
                    TrackId = "12345",
                    Order = order1
                };
                var packet2 = new Packet()
                {
                    TrackId = "67890",
                    Order = order1
                };
                var packet3 = new Packet()
                {
                    TrackId = "54321",
                    Order = order2
                };
                _context.Packets.Add(packet1);
                _context.Packets.Add(packet2);
                _context.Packets.Add(packet3);

                var orderBakingGood1 = new OrderBakingGood()
                {
                    Order = order1,
                    BakingGood = bg1,
                    Quantity = 40
                };

                var orderBakingGood2 = new OrderBakingGood()
                {
                    Order = order2,
                    BakingGood = bg2,
                    Quantity = 10
                };
                var orderBakingGood3 = new OrderBakingGood()
                {
                    Order = order2,
                    BakingGood = bg3,
                    Quantity = 20
                };
                _context.OrderBakingGoods.Add(orderBakingGood1);
                _context.OrderBakingGoods.Add(orderBakingGood2);
                _context.OrderBakingGoods.Add(orderBakingGood3);

                var bakingGoodBatch1 = new BakingGoodBatch()
                {
                    BakingGood = bg1,
                    Batch = batch1,
                    Quantity = 100
                };
                var bakingGoodBatch2 = new BakingGoodBatch()
                {
                    BakingGood = bg2,
                    Batch = batch2,
                    Quantity = 100
                };
                var bakingGoodBatch3 = new BakingGoodBatch()
                {
                    BakingGood = bg3,
                    Batch = batch2,
                    Quantity = 100
                };
                _context.BakingGoodBatches.Add(bakingGoodBatch1);
                _context.BakingGoodBatches.Add(bakingGoodBatch2);
                _context.BakingGoodBatches.Add(bakingGoodBatch3);

                var batchStock1 = new BatchStock()
                {
                    Batch = batch1,
                    Stock = stock1,
                    Quantity = 200
                };

                var batchStock2 = new BatchStock()
                {
                    Batch = batch2,
                    Stock = stock2,
                    Quantity = 200
                };

                var batchStock3 = new BatchStock()
                {
                    Batch = batch2,
                    Stock = stock3,
                    Quantity = 200
                };

                _context.BatchStocks.Add(batchStock1);
                _context.BatchStocks.Add(batchStock2);
                _context.BatchStocks.Add(batchStock3);

                // add allergen data
                var allergen1 = new Allergen()
                {
                    AllergenName = "Gluten"
                };
                _context.Allergens.Add(allergen1);

                var allergen2 = new Allergen()
                {
                    AllergenName = "Dairy"
                };
                _context.Allergens.Add(allergen2);

                var allergen3 = new Allergen()
                {
                    AllergenName = "Nuts"
                };
                _context.Allergens.Add(allergen3);

                var stockAllergen1 = new StockAllergen()
                {
                    Stock = stock1,
                    StockId = stock1.Id,
                    Allergen = allergen1,
                    AllergenId = allergen1.AllergenId
                };

                var stockAllergen2 = new StockAllergen()
                {
                    Stock = stock3,
                    StockId = stock3.Id,
                    Allergen = allergen2,
                    AllergenId = allergen2.AllergenId
                };
                _context.StockAllergens.Add(stockAllergen1);
                _context.StockAllergens.Add(stockAllergen2);

                await _context.SaveChangesAsync();

                return new JsonResult(new
                {
                    Message = "Database has been seeded",
                    Orders = _context.Orders.Count(),
                    BakingGoods = _context.BakingGoods.Count(),
                    Batches = _context.Batches.Count(),
                    Stocks = _context.Stocks.Count(),
                    Packets = _context.Packets.Count(),
                    OrderBakingGoods = _context.OrderBakingGoods.Count(),
                    BakingGoodBatches = _context.BakingGoodBatches.Count(),
                    BatchStocks = _context.BatchStocks.Count(),
                    Allergens = _context.Allergens.Count(),
                    StockAllergens = _context.StockAllergens.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                return StatusCode(500, new { Message = "An error occurred while seeding the database" });
            }
        }
    }
}