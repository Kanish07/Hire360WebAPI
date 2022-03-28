#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hire360WebAPI.Models;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HumanResourceController : ControllerBase
    {
        private readonly Hire360Context _context;

        public HumanResourceController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/HumanResource
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HumanResource>>> GetHumanResources()
        {
            return await _context.HumanResources.ToListAsync();
        }

        // GET: api/HumanResource/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HumanResource>> GetHumanResource(Guid id)
        {
            try
            {
                var humanResource = await _context.HumanResources.FindAsync(id);

                if (humanResource == null)
                {
                    return Ok(new { status = "Failed", data = humanResource, message = "No HumanResource Id found in the give Id" });
                }

                return humanResource;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "failed", message = "Get HumanResource Failed" });

            }
        }

        // PUT: api/HumanResource/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHumanResource(Guid id, HumanResource humanResource)
        {
            try
            {
                if (id != humanResource.Hrid)
                {
                    return Ok(new { status = "Failed", data = humanResource, message = "Human Resource Id not found" });
                }

                _context.Entry(humanResource).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HumanResourceExists(id))
                {
                    return Ok(new { status = "Failed", data = humanResource, messsage = "Human Resource Id is already available" });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { status = "Failed", data = humanResource, messsage = "Failed to Update the Human Resource" });
        }

        // POST: api/HumanResource
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HumanResource>> PostHumanResource(HumanResource humanResource)
        {
            try
            {
                _context.HumanResources.Add(humanResource);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetHumanResource", new { id = humanResource.Hrid }, Ok(new { data = humanResource }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Human Resource" });
            }
        }

        // DELETE: api/HumanResource/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHumanResource(Guid id)
        {
            try
            {
                var humanResource = await _context.HumanResources.FindAsync(id);
                if (humanResource == null)
                {
                    return Ok(new { status = "Failed", data = humanResource, message = "HumanResource Id not found" });
                }

                _context.HumanResources.Remove(humanResource);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = humanResource, messsage = "HumanResource Id has be deleted..." });
            }
             catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete HumanResource" });
            }
        }

        private bool HumanResourceExists(Guid id)
        {
            return _context.HumanResources.Any(e => e.Hrid == id);
        }
    }
}
