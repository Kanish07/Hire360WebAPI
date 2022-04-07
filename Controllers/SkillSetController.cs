#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hire360WebAPI.Models;
using Hire360WebAPI.Helpers;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[Action]")]
    [ApiController]
    [Authorize]
    public class SkillSetController : ControllerBase
    {
        private readonly Hire360Context _context;

        public SkillSetController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/SkillSet
        [HttpGet]
        public async Task<IActionResult> GetAllSkillSets()
        {
            try
            {
                var skillSet = await _context.SkillSets.ToListAsync();
                return Ok(new { status = "success", data = skillSet, message = "Get all skillset successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all skillset data failed" });
            }
        }

        // GET: api/SkillSet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillSet>> GetSkillSetById(Guid id)
        {
            try
            {
                var skillSet = await _context.SkillSets.FindAsync(id);

                if (skillSet == null)
                {
                    return NotFound(new { status = "failed", data = skillSet, message = "No skillsets id found" });
                }
                return Ok(new { status = "success", data = skillSet, message = "Get skillsets by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get skillsets failed" });
            }
        }

        // PUT: api/SkillSet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkillSetById(Guid id, SkillSet skillSet)
        {
            try
            {
                _context.Entry(skillSet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!SkillSetExists(id))
                {
                    return NotFound(new { status = "failed", data = skillSet, messsage = "Skillset id not found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update skillSet by id failed" }); ;
                }
            }
            return Ok(new { status = "success", data = skillSet, messsage = "Details updated" });
        }

        // POST: api/SkillSet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SkillSet>> AddNewSkillSet(SkillSet skillSet)
        {
            try
            {
                _context.SkillSets.Add(skillSet);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSkillSetById", new { id = skillSet.SkillSetId }, new { status = "success", data = skillSet, message = "Skillset registered successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to create new skillset" });
            }
        }

        // DELETE: api/SkillSet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillSetById(Guid id)
        {
            try
            {
                var skillSet = await _context.SkillSets.FindAsync(id);
                if (skillSet == null)
                {
                    return NotFound(new { status = "failed", data = skillSet, message = "Skillsets id not found" });
                }

                _context.SkillSets.Remove(skillSet);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", data = skillSet, messsage = "Skillsets deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to delete skillsets" });
            }
        }

        private bool SkillSetExists(Guid id)
        {
            return _context.SkillSets.Any(e => e.SkillSetId == id);
        }
    }
}
