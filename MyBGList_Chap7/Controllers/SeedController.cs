using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBGList.Models;
using MyBGList.Models.Csv;
using MyBGList_Chap6.Data;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace MyBGList.Controllers
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

        [HttpPost(Name = "Seed")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post()
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
                {
                    HasHeaderRecord = true,
                    Delimiter = ";"
                };

                // Read and process the first CSV file (BakingRecordBase)
                var baseCsvFilePath = Path.Combine(_env.ContentRootPath, "Data/sample_data.csv");
                using (var reader = new StreamReader(baseCsvFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var bakingRecordBaseRecords = csv.GetRecords<BakingRecordBase>();
                    foreach (var record in bakingRecordBaseRecords)
                    {
                        // Insert records into respective tables
                        var order = new Order()
                        {
                            DeliveryPlace = record.DeliveryPlace,
                            DeliveryDate = record.DeliveryDate
                        };
                        _context.Orders.Add(order);

                        var bakingGood = new BakingGood()
                        {
                            Name = record.BgName,
                            DateProduced = record.DateProduced
                        };
                        _context.BakingGoods.Add(bakingGood);

                        var batch = new Batch()
                        {
                            StartDateTime = record.BatchStartDateTime,
                            EndDateTime = record.BatchEndDateTime,
                            ActualEndTime = record.BatchActualEndTime
                        };
                        _context.Batches.Add(batch);

                        var stock = new Stock()
                        {
                            Name = record.SkName,
                            Quantity = record.StockQuantity
                        };
                        _context.Stocks.Add(stock);
                    }
                }

                await _context.SaveChangesAsync();

                // Read and process the second CSV file (BakingRecordMap)
                var mapCsvFilePath = Path.Combine(_env.ContentRootPath, "Data/sample_data_junction.csv");
                using (var reader = new StreamReader(mapCsvFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var records2 = csv.GetRecords<BakingRecordMap>();
                    foreach (var record in records2)
                    {
                        // Check if the Order with the specified ID exists in the context
                        // var existingOrder = _context.Orders
                        // .AsNoTrackingWithIdentityResolution()
                        // .FirstOrDefault(o => o.Id == record.PacketOrderId);
                        var existingOrder = _context.Orders
                            .AsNoTrackingWithIdentityResolution()
                            .ToDictionary(o => o.Id)[record.PacketOrderId];
                        var packet = new Packet()
                        {
                            TrackId = record.TrackId,
                            Order = existingOrder
                        };
                        _context.Packets.Add(packet);
                        await _context.SaveChangesAsync();
                        // Check if the Order and BakingGood with the specified IDs exist in the context
                        // existingOrder = _context.Orders
                        // .AsNoTrackingWithIdentityResolution()
                        // .FirstOrDefault(o => o.Id == record.OrderBakingGoodOrderId);

                        // Add the OrderBakingGood
                        _context.OrderBakingGoods.Add(new OrderBakingGood()
                        {
                            Order = existingOrder,
                            BakingGood = _context.BakingGoods
                                .AsNoTrackingWithIdentityResolution()
                                .ToDictionary(bg => bg.Id)[record.OrderBakingGoodBakingGoodId],
                            Quantity = record.OrderBakingGoodQuantity
                        });
                        // var packet = new Packet()
                        // {
                        //     TrackId = record.TrackId,
                        //     Order = _context.Orders.Where(o => o.Id == record.PacketOrderId).FirstOrDefault()
                        // };
                        // _context.Packets.Add(packet);

                        // _context.OrderBakingGoods.Add(new OrderBakingGood()
                        // {
                        //     Order = _context.Orders.Where(o => o.Id == record.OrderBakingGoodOrderId).FirstOrDefault(),
                        //     BakingGood = _context.BakingGoods.Where(bg => bg.Id == record.OrderBakingGoodBakingGoodId).FirstOrDefault(),
                        //     Quantity = record.OrderBakingGoodQuantity
                        // });

                        // _context.BakingGoodBatches.Add(new BakingGoodBatch()
                        // {
                        //     BakingGoodId = record.BakingGoodBatchBakingGoodId,
                        //     BatchId = record.BakingGoodBatchBatchId,
                        //     Quantity = record.BakingGoodBatchQuantity
                        // });

                        // _context.BatchStocks.Add(new BatchStock()
                        // {
                        //     BatchId = record.BatchStockBatchId,
                        //     StockId = record.BatchStockStockId,
                        //     Quantity = record.BatchStockQuantity
                        // });
                    }

                    await _context.SaveChangesAsync();

                    return new JsonResult(new
                    {
                        Message = "Database has been seeded",
                        Orders = _context.Orders.Count(),
                        Packets = _context.Packets.Count(),
                        BakingGoods = _context.BakingGoods.Count(),
                        Batches = _context.Batches.Count(),
                        Stocks = _context.Stocks.Count(),
                        OrderBakingGoods = _context.OrderBakingGoods.Count(),
                        BakingGoodBatches = _context.BakingGoodBatches.Count(),
                        BatchStocks = _context.BatchStocks.Count()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                return StatusCode(500, new { Message = "An error occurred while seeding the database" });
            }
        }
    }
}
