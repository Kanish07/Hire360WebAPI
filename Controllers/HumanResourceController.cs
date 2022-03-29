#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hire360WebAPI.Models;
using Hire360WebAPI.Services;
using Hire360WebAPI.Helpers.HRJWT;
using Hire360WebAPI.Entities;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HumanResourceController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly IHumanResourceServices _humanResourceServices;


        public HumanResourceController(Hire360Context context, IHumanResourceServices humanResourceServices)
        {
            _context = context;
            _humanResourceServices = humanResourceServices;
        }

        // POST: api/candidate/login
        [HttpPost("login")]
        public IActionResult Login(AuthRequest model)
        {
            var hr = _context.HumanResources.FirstOrDefault(x => x.Hremail == model.Email);
            if (hr == null)
                return BadRequest(new { message = "Email not found!" });
            var verify = BCrypt.Net.BCrypt.Verify(model.Password, hr!.Hrpassword);
            if (!verify)
                return BadRequest(new { message = "Incorrect password!" });
            var response = _humanResourceServices.Authenticate(hr);
            return Ok(response);
        }

        // GET: api/HumanResource
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<HumanResource>>> GetHumanResources()
        {
            return await _context.HumanResources.ToListAsync();
        }

        // GET: api/HumanResource/5
        [HttpGet("{id}")]
        [Authorize(Role.HR)]
        public async Task<ActionResult<HumanResource>> GetHumanResource(Guid id)
        {
            var humanResource = await _context.HumanResources.FindAsync(id);

            if (humanResource == null)
            {
                return NotFound();
            }

            return humanResource;
        }

        // PUT: api/HumanResource/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHumanResource(Guid id, HumanResource humanResource)
        {
            if (id != humanResource.Hrid)
            {
                return BadRequest();
            }

            humanResource.Hrpassword = BCrypt.Net.BCrypt.HashPassword(humanResource.Hrpassword);
            _context.Entry(humanResource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HumanResourceExists(id))
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

        // POST: api/HumanResource
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HumanResource>> PostHumanResource(HumanResource humanResource)
        {
            humanResource.Hrpassword = BCrypt.Net.BCrypt.HashPassword(humanResource.Hrpassword);
            _context.HumanResources.Add(humanResource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHumanResource", new { id = humanResource.Hrid }, humanResource);
        }

        // DELETE: api/HumanResource/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHumanResource(Guid id)
        {
            var humanResource = await _context.HumanResources.FindAsync(id);
            if (humanResource == null)
            {
                return NotFound();
            }

            _context.HumanResources.Remove(humanResource);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HumanResourceExists(Guid id)
        {
            return _context.HumanResources.Any(e => e.Hrid == id);
        }
    }
}
