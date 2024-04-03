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
using Serilog.Filters;
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

        [HttpPut(Name = "Seed")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Put()
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
                _context.ChangeTracker.Clear();
                using var transaction = _context.Database.BeginTransaction();
                var existingOrders = await _context.Orders.ToDictionaryAsync(o => o.Id);
                var existingBakingGoods = await _context.BakingGoods.ToDictionaryAsync(bg => bg.Id);
                var existingBatches = await _context.Batches.ToDictionaryAsync(b => b.Id);
                var existingStocks = await _context.Stocks.ToDictionaryAsync(s => s.Id);
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
                        var order1 = existingOrders.GetValueOrDefault(record.PacketOrderId);
                        var packet = new Packet()
                        {
                            TrackId = record.TrackId,
                            Order = order1
                        };
                        _context.Packets.Add(packet);
                        _context.Orders.Update(order1);
                        if (packet != null)
                            _context.Entry(packet).State = EntityState.Detached;
                    }
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Order] ON");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Order] OFF");

                    foreach (var record in records2)
                    {
                        var order1 = existingOrders.GetValueOrDefault(record.OrderBakingGoodOrderId);
                        var bg1 = existingBakingGoods.GetValueOrDefault(record.OrderBakingGoodBakingGoodId);
                        var orderBakingGood = new OrderBakingGood()
                        {
                            Order = order1,
                            BakingGood = bg1,
                            Quantity = record.OrderBakingGoodQuantity
                        };
                        _context.OrderBakingGoods.Add(orderBakingGood);
                        if (orderBakingGood != null)
                            _context.Entry(orderBakingGood).State = EntityState.Detached;
                    }

                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BakingGood ON");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BakingGood OFF");

                    foreach (var record in records2)
                    {
                        var bakingGoodBatch = new BakingGoodBatch()
                        {
                            BakingGood = existingBakingGoods.GetValueOrDefault(record.BakingGoodBatchBakingGoodId),
                            Batch = existingBatches.GetValueOrDefault(record.BakingGoodBatchBatchId),
                            Quantity = record.BakingGoodBatchQuantity
                        };
                        _context.BakingGoodBatches.Add(bakingGoodBatch);
                        if (bakingGoodBatch != null)
                            _context.Entry(bakingGoodBatch).State = EntityState.Detached;
                    }

                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Batch] ON");
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Batch] OFF");

                    foreach (var record in records2)
                    {
                        var batchStock = new BatchStock()
                        {
                            Batch = existingBatches.GetValueOrDefault(record.BatchStockBatchId),
                            Stock = existingStocks.GetValueOrDefault(record.BatchStockStockId),
                            Quantity = record.BatchStockQuantity
                        };
                        _context.BatchStocks.Add(batchStock);
                        if (batchStock != null)
                            _context.Entry(batchStock).State = EntityState.Detached;
                    }
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Stock] ON");
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Stock] OFF");
                transaction.Commit();
                _context.ChangeTracker.Clear();

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
                    BatchStocks = _context.BatchStocks.Count()
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