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
        public async Task<IActionResult> GetAllSkillSets()
        {
            try
            {
                var skillSet = await _context.JobApplieds.ToListAsync();
                return Ok(new { status = "Success", data = skillSet, message = "Get All the skillSet Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Get All skillSet Data Failed" });
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
                    return Ok(new { status = "Failed", data = skillSet, message = "No SkillSets Id found" });
                }

                return Ok(new { status = "success", data = skillSet, message = "Get SkillSets By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get SkillSets Failed" });
            }
        }

        // PUT: api/SkillSet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkillSetById(Guid id, SkillSet skillSet)
        {
            _context.Entry(skillSet).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!SkillSetExists(id))
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return Ok(new { status = "Failed", data = skillSet, messsage = "SkillSet Id not available" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update SkillSet By Id Failed" }); ;
                }
            }
            return Ok(new { status = "Success", data = skillSet, messsage = "Details Updated" });
        }

        // POST: api/SkillSet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SkillSet>> RegisterSkillSet(SkillSet skillSet)
        {
            try
            {
                _context.SkillSets.Add(skillSet);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSkillSetById", new { id = skillSet.SkillSetId }, new { status = "Success", data = skillSet, message = "SkillSet Successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new SkillSet" });
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
                    return Ok(new { status = "Failed", data = skillSet, message = "SkillSets Id not found" });
                }

                _context.SkillSets.Remove(skillSet);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = skillSet, messsage = "SkillSets deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete SkillSets" });
            }
        }

        private bool SkillSetExists(Guid id)
        {
            return _context.SkillSets.Any(e => e.SkillSetId == id);
        }
    }
}
