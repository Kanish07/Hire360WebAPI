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
    public class SkillController : ControllerBase
    {
        private readonly Hire360Context _context;

        public SkillController(Hire360Context context)
        {
            _context = context;
        }

        // Get all the skills
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

        // Get skill using skill id
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

        // Update the skill using skill id
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

        // Add new Skill
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Skill>> AddNewSkill(Skill skill)
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

        // Delete skill using skill id
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

        // Get skills by using candidate id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillsByCandidateId(Guid id)
        {
            try
            {
                var skill = await _context.Skills.Where(s => s.CandidateId == id).Include(s => s.SkillSet).ToListAsync();
                return Ok(new { status = "success", data = skill, messsage = "Get skills by candidate id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Skills by candidate id failed" });
            }
        }

        private bool SkillExists(Guid id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}
