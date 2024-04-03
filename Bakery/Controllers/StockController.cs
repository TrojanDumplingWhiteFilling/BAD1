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


namespace Bakery.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public StockController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Stock

        [HttpGet(Name = "GetStock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetStock()
        {
            _logger.LogInformation(CustomLogEvents.Stock_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var StockInfo = await _context.BatchStocks
            .Select(bs => new StockDTO
            {
                Ingredient = bs.Stock.Name,
                Quantity = bs.Quantity
            })
            .ToListAsync();

            // Construct the response DTO
            var restDTO = new RestDTO<StockDTO>
            {
                Data = StockInfo,
                // You may set other properties such as page index, page size, and record count if needed
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action("GetStock", "Stock", new {}, Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }

    }
}