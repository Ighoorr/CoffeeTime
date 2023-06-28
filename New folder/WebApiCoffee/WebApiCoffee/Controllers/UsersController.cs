using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCoffee.Models;
using WebApiCoffee.Services;

namespace WebApiCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CoffeeContext _context;
        private readonly SMTPService _emailService;
        private readonly IPhotoService _photoService;

        public UsersController(CoffeeContext context, SMTPService emailService,IPhotoService photoService)
        {
            _context = context;
            _emailService = emailService;
            _photoService = photoService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u=>u.Photo).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        // GET: api/Users/5
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.SecondName = user.SecondName;
            existingUser.Age = user.Age;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            if (user.Pass != null)
            {
                existingUser.Pass = user.Pass;
            }
            if (user.Photo != null)
            {
                existingUser.Photo = user.Photo;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var t = _context.Users;
            return NoContent();
        }


        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (userExists)
            {
                return Conflict("User with the same email already exists.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Hobbies.RemoveRange(_context.Hobbies.Where(h => h.UserId == id));
            _context.Photos.RemoveRange(_context.Photos.Where(p => p.UserId == id));
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("suggestions/{userId}")]
        public IActionResult GetUserHobbies(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(); // Повернути 404, якщо користувача не знайдено
            }

            var userHobbyNames = _context.Hobbies
                .Where(h => h.UserId == userId)
                .Select(h => h.Name.ToLower())
                .ToList();

            var result = _context.Users
                .Include(u => u.Photo)
                .Where(u => u.UserId != userId)
                .Where(u => _context.Hobbies
                    .Where(h => h.UserId == u.UserId)
                    .Any(h => userHobbyNames.Contains(h.Name.ToLower())))
                .Select(u => new
                {   u.Photo.Url,
                    u.UserId,
                    u.FirstName,
                    u.Email,
                    Hobbies = string.Join(", ", _context.Hobbies
                        .Where(h => h.UserId == u.UserId && userHobbyNames.Contains(h.Name.ToLower()))
                        .Select(h => h.Name))
                })
                .ToList();

            return Ok(result);
            /*var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(); 
            }

            var userHobbies = _context.Hobbies.Where(h => h.UserId == userId).ToList();

            var commonHobbies = _context.Hobbies
                .Where(h => h.UserId != userId)
                .AsEnumerable() // Виклик AsEnumerable для перемикання на клієнтську оцінку
                .Where(h => userHobbies.Any(uh => uh.Name == h.Name))
                .Select(h => h.Name)
                .ToList();

            var result = new
            {
                user.UserId,
                user.FirstName,
                user.Email,
                Hobbies = string.Join(", ", commonHobbies)
            };

            return Ok(result);*/
        }
        [HttpPost ("request")]
        public IActionResult SendEmail(EmailDTO request)
        {
            _emailService.SendEmail2(request);
            return Ok();
        }
        [HttpPost ("addphoto/{id}")]
        public async Task<ActionResult<Photo>> AddPhoto(int id,IFormFile file)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.UserId == id);
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            if (user.Photo != null)
            {
                //var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Url == user.Photo.Url);
                // photo.Url = result.SecureUrl.AbsoluteUri;
                //  photo.PublicId = result.PublicId;
                await _photoService.DeletePhotoAsync(user.Photo.PublicId);
                user.Photo.Url = result.SecureUrl.AbsoluteUri;
                user.Photo.PublicId = result.PublicId;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return user.Photo;
                }
            }
            else
            {
                var photo = new Photo
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,
                    UserId = id
                };
                _context.Photos.Add(photo);
                user.Photo = photo;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return user.Photo;
                }
            }

           

            // }

            return BadRequest("Problem adding photo");
        }
        [HttpDelete("deletephoto/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var photo = user.Photo;
            if (photo == null)
            {
                return NotFound("Photo not found");
            }

            // Видалити фото з бази даних
            _context.Photos.Remove(photo);

            // Видалити фото з файлової системи або хмарного сховища (якщо ви його використовуєте)
            // Використайте ваш сервіс фото для виконання цієї операції
            var deleteResult = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (deleteResult.Error != null)
            {
                return BadRequest(deleteResult.Error.Message);
            }

            // Зберегти зміни у базі даних
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
