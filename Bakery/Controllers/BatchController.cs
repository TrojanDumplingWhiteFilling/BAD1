using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bakery.DTO;
using MyBGList_Chap6.Data;
using Bakery.Models;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bakery.Constants;
using System.Linq.Dynamic.Core.Exceptions;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;

namespace Bakery.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<OrderController> _logger;

        public BatchController(
            ApplicationDbContext context,
            ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Ingridients/{batchId}", Name = "GetBatchStock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetBatchStock(int batchId)
        {
            _logger.LogInformation(CustomLogEvents.Batch_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var Ingridients = await _context.BatchStocks
                .Where(o => o.BatchId == batchId)
                .Select(o => new BakedGoodQuantityDTO
                {
                    BakedGood = o.Stock.Name,
                    Quantity = o.Quantity
                }).ToListAsync();

            // Construct the response DTO
            var restDTO = new RestDTO<BakedGoodQuantityDTO>
            {
                Data = Ingridients,
                // You may set other properties such as page index, page size, and record count if needed
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action(
                    "GetBatchStock",
                    "Batch",
                    new { batchId },
                    Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }

        [HttpGet("AvgDelay/", Name = "GetAverageDelay")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetAverageDelay()
        {
            _logger.LogInformation(CustomLogEvents.Batch_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var delays = await _context.Batches
                .Select(b => EF.Functions.DateDiffMinute(b.EndDateTime, b.ActualEndTime))
                .ToListAsync();

            var averageDelay = delays.Any() ? delays.Average() : 0;

            List<double> doubles = new List<double>();
            doubles.Add(averageDelay);

            // Construct the response DTO
            var restDTO = new RestDTO<double>
            {
                Data = doubles,
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action(
                    "GetAverageDelay",
                    "Batch",
                    new { },
                    Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }

    }
}

