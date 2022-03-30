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
    public class SkillController : ControllerBase
    {
        private readonly Hire360Context _context;

        public SkillController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/Skill
        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                var skill = await _context.JobApplieds.ToListAsync();
                return Ok(new { status = "success", data = skill, message = "Get all the skills successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all skills data failed" });
            }
        }

        // GET: api/Skill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkillById(Guid id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(new { status = "failed", data = skill, message = "No skills id found" });
                }
                return Ok(new { status = "success", data = skill, message = "Get skills by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get skills failed" });
            }
        }

        // PUT: api/Skill/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkillById(Guid id, Skill skill)
        {
            try
            {
                _context.Entry(skill).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!SkillExists(id))
                {
                    return NotFound(new { status = "failed", data = skill, messsage = "Skill id not found" }); 
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update skills by id failed" }); ;
                }
            }

            return Ok(new { status = "success", data = skill, messsage = "Details updated" });
        }

        // POST: api/Skill
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Skill>> RegisterSkill(Skill skill)
        {
            try
            {
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSkillById", new { id = skill.SkillId }, new { status = "success", data = skill, message = "Skills applied successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to create new skill" });
            }
        }

        // DELETE: api/Skill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillById(Guid id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);
                if (skill == null)
                {
                    return NotFound(new { status = "failed", data = skill, message = "Skills id not found" });
                }
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
                return Ok(new { status = "success", data = skill, messsage = "Skills deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to delete skills" });
            }
        }

        private bool SkillExists(Guid id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}
