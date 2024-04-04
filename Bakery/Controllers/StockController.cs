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
            var StockInfo = await _context.Stocks
            .Select(bs => new StockDTO
            {
                Ingredient = bs.Name,
                Quantity = bs.Quantity,
                Allergens = bs.StockAllergens
                    .Select(sa => sa.Allergen.AllergenName)
                    .ToList()
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

        //POST
        [HttpPost(Name = "AddStock")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> AddStock(StockDTO stockDto)
        {
            try
            {
                if (stockDto.Quantity < 0)
                {
                    return BadRequest("Quantity cannot be negative.");
                }

                var stock = new Stock
                {
                    Name = stockDto.Ingredient,
                    Quantity = stockDto.Quantity
                };

                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();

                var ListOfStocks = await _context.Stocks
                    .Where(s => s.Name == stock.Name)
                    .ToListAsync();

                return Ok(new RestDTO<Stock>()
                {
                    Data = ListOfStocks,
                    Links = new List<LinkDTO>
                {
                    new LinkDTO(
                            Url.Action(
                                null,
                                "Stock",
                                new {stock.Name},
                                Request.Scheme)!,
                            "self",
                            "POST"),
                }
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(CustomLogEvents.Stock_Post, ex, "Error in Post method.");
                return StatusCode(500, "Internal server error.");
            }

        }



        // PUT
        [HttpPut(Name = "UpdateStock")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> UpdateStock(StockDTO stockDto)
        {
            try
            {
                if (stockDto.Quantity < 0)
                {
                    return BadRequest("Quantity cannot be negative.");
                }

                var stock = await _context.Stocks
                    .Where(s => s.Name == stockDto.Ingredient)
                    .FirstOrDefaultAsync();

                if (stock == null)
                {
                    return NotFound("Stock not found.");
                }
                stock.Quantity = stockDto.Quantity;

                _context.Stocks.Update(stock);
                await _context.SaveChangesAsync();

                return Ok(new RestDTO<Stock>()
                {
                    Data = new List<Stock> { stock },
                    Links = new List<LinkDTO>
                {
                    new LinkDTO(
                            Url.Action(
                                null,
                                "Stock",
                                new {stock.Name},
                                Request.Scheme)!,
                            "self",
                            "PUT"),
                }
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(CustomLogEvents.Stock_Put, ex, "Error in Put method.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // DELETE
        [HttpDelete(Name = "DeleteStock")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> DeleteStock(string ingredient)
        {
            try
            {
                var stock = await _context.Stocks
                    .Where(s => s.Name == ingredient)
                    .FirstOrDefaultAsync();

                if (stock == null)
                {
                    return NotFound("Stock not found.");
                }

                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();

                return Ok(new RestDTO<Stock>()
                {
                    Data = new List<Stock> { stock },
                    Links = new List<LinkDTO>
                {
                    new LinkDTO(
                            Url.Action(
                                null,
                                "Stock",
                                new {stock},
                                Request.Scheme)!,
                            "self",
                            "DELETE"),
                }
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(CustomLogEvents.Stock_Delete, ex, "Error in Delete method.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}