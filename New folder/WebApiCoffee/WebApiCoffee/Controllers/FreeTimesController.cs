using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCoffee.Models;

namespace WebApiCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreeTimesController : ControllerBase
    {
        private readonly CoffeeContext _context;

        public FreeTimesController(CoffeeContext context)
        {
            _context = context;
        }

        // GET: api/FreeTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FreeTime>>> GetFreeTimes()
        {
            return await _context.FreeTimes.ToListAsync();
        }

        // GET: api/FreeTimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FreeTime>> GetFreeTime(int id)
        {
            var freeTime = await _context.FreeTimes.FindAsync(id);

            if (freeTime == null)
            {
                return NotFound();
            }

            return freeTime;
        }

        // PUT: api/FreeTimes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFreeTime(int id, FreeTime freeTime)
        {
            if (id != freeTime.TimeId)
            {
                return BadRequest();
            }

            _context.Entry(freeTime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FreeTimeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FreeTimes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FreeTime>> PostFreeTime(FreeTime freeTime)
        {
            _context.FreeTimes.Add(freeTime);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFreeTime", new { id = freeTime.TimeId }, freeTime);
        }

        // DELETE: api/FreeTimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFreeTime(int id)
        {
            var freeTime = await _context.FreeTimes.FindAsync(id);
            if (freeTime == null)
            {
                return NotFound();
            }

            _context.FreeTimes.Remove(freeTime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FreeTimeExists(int id)
        {
            return _context.FreeTimes.Any(e => e.TimeId == id);
        }
    }
}
