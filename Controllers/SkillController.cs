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
                return Ok(new { status = "Success", data = skill, message = "Get All the Skills Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Get All Skills Data Failed" });
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
                    return Ok(new { status = "Failed", data = skill, message = "No Skills Id found" });
                }
                return Ok(new { status = "success", data = skill, message = "Get Skills By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get Skills Failed" });
            }
        }

        // PUT: api/Skill/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkillById(Guid id, Skill skill)
        {
            _context.Entry(skill).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!SkillExists(id))
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return Ok(new { status = "Failed", data = skill, messsage = "Skill Id not available" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update Skills By Id Failed" }); ;
                }
            }

            return Ok(new { status = "Failed", data = skill, messsage = "Failed to Update the Skill" });
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

                return CreatedAtAction("GetSkillById", new { id = skill.SkillId }, new { status = "Success", data = skill, message = "Skills Applied Successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Skill" });
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
                    return Ok(new { status = "Failed", data = skill, message = "Skills Id not found" });
                }
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
                return Ok(new { status = "Success", data = skill, messsage = "Skills deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Skills" });
            }
        }

        private bool SkillExists(Guid id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}
