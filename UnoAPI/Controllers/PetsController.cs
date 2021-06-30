using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnoAPI.Data;
using UnoAPI.Data.Models;
using UnoAPI.Inteface;

namespace UnoAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly UnoContext _context;

        public PetsController(UnoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Customer/{CustomerId}")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(string CustomerId)
        {
            return await _context.Pets.Where(p => p.Owner.Id == CustomerId).Include(p => p.Owner).Include(p => p.Kind).ToListAsync();
        }

        // PUT: api/Pets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, Pet pet)
        {
            if (id != pet.Id)
            {
                return BadRequest();
            }

            _context.Entry(pet.Kind).State = EntityState.Unchanged;
            _context.Entry(pet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
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

        // POST: api/Pets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pet>> PostPet(Pet pet)
        {
            /*User user = await _context.Users.SingleAsync(u => u.Id == ownerId);
            Specie kind = await _context.Species.SingleAsync(s => s.Id == specieId);
            pet.Owner = user;
            pet.Kind = kind;*/
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            pet.Owner = await _context.Users.SingleAsync(u => u.Email == email);
            _context.Attach(pet.Kind);
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPet", new { id = pet.Id }, pet);
        }

        // DELETE: api/Pets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("Species")]
        public async Task<List<Specie>> GetSpecies()
        {
            return await _context.Species.ToListAsync();
        }
    }
}
