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
    public class HobbiesController : ControllerBase
    {
        private readonly CoffeeContext _context;

        public HobbiesController(CoffeeContext context)
        {
            _context = context;
        }

        // GET: api/Hobbies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hobby>>> GetHobbies()
        {
            return await _context.Hobbies.ToListAsync();
        }

        // GET: api/Hobbies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hobby>> GetHobby(int id)
        {
            var hobby = await _context.Hobbies.FindAsync(id);

            if (hobby == null)
            {
                return NotFound();
            }

            return hobby;
        }
        // GET: api/Hobbies/5
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Hobby>>> GetUserHobbies(int id)
        {
            var hobby = await _context.Hobbies.Where(h => h.UserId == id).ToListAsync(); 

            if (hobby == null)
            {
                return NotFound();
            }

            return hobby;
        }

        // PUT: api/Hobbies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHobby(int id, Hobby hobby)
        {
            if (id != hobby.HobbyId)
            {
                return BadRequest();
            }

            _context.Entry(hobby).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HobbyExists(id))
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

        // POST: api/Hobbies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hobby>> PostHobby(Hobby hobby)
        {
            _context.Hobbies.Add(hobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHobby", new { id = hobby.HobbyId }, hobby);
        }

        // DELETE: api/Hobbies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHobby(int id)
        {
            var hobby = await _context.Hobbies.FindAsync(id);
            if (hobby == null)
            {
                return NotFound();
            }

            _context.Hobbies.Remove(hobby);
            await _context.SaveChangesAsync();
            

            return NoContent();
        }

        private bool HobbyExists(int id)
        {
            return _context.Hobbies.Any(e => e.HobbyId == id);
        }
    }
}
