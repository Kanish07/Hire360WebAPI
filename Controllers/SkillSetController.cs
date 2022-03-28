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
            var skillSet = await _context.SkillSets.FindAsync(id);

            if (skillSet == null)
            {
                return NotFound();
            }

            return skillSet;
        }

        // PUT: api/SkillSet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkillSet(Guid id, SkillSet skillSet)
        {
            if (id != skillSet.SkillSetId)
            {
                return BadRequest();
            }

            _context.Entry(skillSet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillSetExists(id))
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

        // POST: api/SkillSet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SkillSet>> PostSkillSet(SkillSet skillSet)
        {
            _context.SkillSets.Add(skillSet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkillSet", new { id = skillSet.SkillSetId }, skillSet);
        }

        // DELETE: api/SkillSet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillSet(Guid id)
        {
            var skillSet = await _context.SkillSets.FindAsync(id);
            if (skillSet == null)
            {
                return NotFound();
            }

            _context.SkillSets.Remove(skillSet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SkillSetExists(Guid id)
        {
            return _context.SkillSets.Any(e => e.SkillSetId == id);
        }
    }
}
