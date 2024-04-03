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
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<OrderController> _logger;

        public OrderController(
            ApplicationDbContext context,
            ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("All", Name = "GetOrdersContentAndPackets")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> Get(
            [FromQuery] RequestDTO<OrderDTO> input)
        {
            _logger.LogInformation(CustomLogEvents.Orders_Get,
                "Get method started.");

            var query = _context.Orders
                .Include(o => o.OrderBakingGoods)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(o => o.DeliveryPlace.Contains(input.FilterQuery)
                                          || o.DeliveryDate.ToString().Contains(input.FilterQuery));
            }

            var recordCount = await query.CountAsync();

            query = query
                .OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);

            var orders = await query.ToListAsync();

            var links = new List<LinkDTO>
    {
        new LinkDTO(
            Url.Action(
                null,
                "Order",
                new { input.PageIndex, input.PageSize },
                Request.Scheme)!,
            "self",
            "GET")
    };

            return Ok(new RestDTO<Order>()
            {
                Data = orders,
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = recordCount,
                Links = links
            });
        }

        [HttpGet("Info/{orderId}", Name = "GetOrderInfo")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetOrderInfo(int orderId)
        {
            _logger.LogInformation(CustomLogEvents.Orders_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var orderInfo = await _context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new OrderDTO
                {
                    DeliveryPlace = o.DeliveryPlace,
                    DeliveryDate = o.DeliveryDate
                }).ToListAsync();

            // Construct the response DTO
            var restDTO = new RestDTO<OrderDTO>
            {
                Data = orderInfo,
                // You may set other properties such as page index, page size, and record count if needed
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action(
                    "GetOrderInfo",
                    "Order",
                    new { orderId },
                    Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }

        [HttpGet("Contents/{orderId}", Name = "GetOrderContents")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetOrderContents(int orderId)
        {
            _logger.LogInformation(CustomLogEvents.Orders_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var orderInfo = await _context.Orders
                .Include(o => o.OrderBakingGoods)
                .ThenInclude(obg => obg.BakingGood)
                .Where(o => o.Id == orderId)
                .Select(o => new BakedGoodsDTO
                {
                    Quantity = o.OrderBakingGoods.Select(obg => obg.Quantity).FirstOrDefault(),
                    BakedGood = o.OrderBakingGoods.Select(obg => obg.BakingGood.Name).FirstOrDefault()
                }).ToListAsync();

            // Construct the response DTO
            var restDTO = new RestDTO<BakedGoodsDTO>
            {
                Data = orderInfo,
                // You may set other properties such as page index, page size, and record count if needed
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action(
                    "GetOrderInfo",
                    "Order",
                    new { orderId },
                    Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }

        [HttpGet("TrackIds/{orderId}", Name = "GetPacketsForOrder")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<IActionResult> GetPacketsForOrder(int orderId)
        {
            _logger.LogInformation(CustomLogEvents.Orders_Get,
                "Get method started.");

            // Retrieve packets associated with the specified order id and send Construct the DTO
            var packets = await _context.Packets
                .Where(p => p.OrderId == orderId)
                .Select(p => new PacketDTO
                {
                    TrackId = p.TrackId
                }).ToListAsync();

            // Construct the response DTO
            var restDTO = new RestDTO<PacketDTO>
            {
                Data = packets,
                // You may set other properties such as page index, page size, and record count if needed
                Links = new List<LinkDTO>
            {
            // Add HATEOAS links as needed
            new LinkDTO(
                Url.Action(
                    "GetPacketsForOrder",
                    "Order",
                    new { orderId },
                    Request.Scheme)!,
                "self",
                "GET")
        }
            };

            return Ok(restDTO);
        }


    }
}

