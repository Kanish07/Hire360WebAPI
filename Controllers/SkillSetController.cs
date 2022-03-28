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
    public class SkillSetController : ControllerBase
    {
        private readonly Hire360Context _context;

        public SkillSetController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/SkillSet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillSet>>> GetSkillSets()
        {
            return await _context.SkillSets.ToListAsync();
        }

        // GET: api/SkillSet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillSet>> GetSkillSet(Guid id)
        {
            try
            {
                var skillSet = await _context.SkillSets.FindAsync(id);

                if (skillSet == null)
                {
                    return Ok(new { status = "Failed", data = skillSet, message = "No SkillSets Id found in the give Id" });
                }

                return skillSet;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "failed", message = "Get SkillSets Failed" });
            }
        }

        // PUT: api/SkillSet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkillSet(Guid id, SkillSet skillSet)
        {
            try
            {
                if (id != skillSet.SkillSetId)
                {
                    return Ok(new { status = "Failed", data = skillSet, message = "SkillSet Id not found" });
                }

                _context.Entry(skillSet).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillSetExists(id))
                {
                    return Ok(new { status = "Failed", data = skillSet, messsage = "SkillSet Id is already available" });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { status = "Failed", data = skillSet, messsage = "Failed to Update the SkillSet" });
        }

        // POST: api/SkillSet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SkillSet>> PostSkillSet(SkillSet skillSet)
        {
            try
            {
                _context.SkillSets.Add(skillSet);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSkillSet", new { id = skillSet.SkillSetId }, Ok(new { data = skillSet }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new SkillSet" });
            }
        }

        // DELETE: api/SkillSet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillSet(Guid id)
        {
            try
            {
                var skillSet = await _context.SkillSets.FindAsync(id);
                if (skillSet == null)
                {
                    return Ok(new { status = "Failed", data = skillSet, message = "SkillSets Id not found" });
                }

                _context.SkillSets.Remove(skillSet);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = skillSet, messsage = "SkillSets Id has be deleted..." });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete SkillSets" });
            }
        }

        private bool SkillSetExists(Guid id)
        {
            return _context.SkillSets.Any(e => e.SkillSetId == id);
        }
    }
}
