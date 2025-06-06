using Microsoft.AspNetCore.Mvc;
using ruhanBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ruhanBack.Dtos;
using ruhanBack.Data;
using ruhanBack.models;
using Microsoft.EntityFrameworkCore;
namespace ruhanBack.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("BuyItem")]
        public async Task<ActionResult> BuyItem([FromBody] BuytItemDto buytItemDto)
        {
            var buyItem = new items
            {
                Name = buytItemDto.Name,
                Skin = buytItemDto.Skin,
                Type = buytItemDto.Type,
                Rarity = buytItemDto.Rarity,
                CSFloatPrice = buytItemDto.CSFloatPrice,
                CSMoneyPrice = buytItemDto.CSMoneyPrice,
                Revenue = buytItemDto.Revenue,
                BoughtAtPrice = buytItemDto.BoughtAtPrice,
                BoughtAtDate = DateTime.Now
            };
            _context.Items.Add(buyItem);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("NotSoldItems")]
        public async Task<ActionResult> NotSoldItems()
        {

            var items = _context.Items.Where(u => u.SoldAtPrice == null).ToList();
            return Ok(items);
        }

        [HttpPost("SellItem")]
        public async Task<ActionResult> SellItem(SelltItemDto selltItemDto)
        {

            var item = await _context.Items.FirstAsync(u => u.Id == selltItemDto.Id);
            item.Revenue = selltItemDto.Revenue;
            item.SoldAtPrice = selltItemDto.SoldAtPrice;
            item.SoldAtDate = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("Summaries")]
        public async Task<ActionResult> GetSummaries()
        {
            var summary = await _context.Items
                .Where(i => i.SoldAtPrice != null)
                .GroupBy(i => 1) // Group by a constant to aggregate over all items
                .Select(g => new
                {
                    TotalRevenue = g.Sum(i => i.Revenue),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            return Ok(summary);
        }


    }
}

