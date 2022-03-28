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
    public class JobAppliedController : ControllerBase
    {
        private readonly Hire360Context _context;

        public JobAppliedController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/JobApplied
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplied>>> GetJobApplieds()
        {
            return await _context.JobApplieds.ToListAsync();
        }

        // GET: api/JobApplied/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplied>> GetJobApplied(Guid id)
        {
            var jobApplied = await _context.JobApplieds.FindAsync(id);

            if (jobApplied == null)
            {
                return NotFound();
            }

            return jobApplied;
        }

        // PUT: api/JobApplied/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobApplied(Guid id, JobApplied jobApplied)
        {
            if (id != jobApplied.JobAppliedId)
            {
                return BadRequest();
            }

            _context.Entry(jobApplied).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobAppliedExists(id))
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

        // POST: api/JobApplied
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobApplied>> PostJobApplied(JobApplied jobApplied)
        {
            _context.JobApplieds.Add(jobApplied);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobApplied", new { id = jobApplied.JobAppliedId }, jobApplied);
        }

        // DELETE: api/JobApplied/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplied(Guid id)
        {
            var jobApplied = await _context.JobApplieds.FindAsync(id);
            if (jobApplied == null)
            {
                return NotFound();
            }

            _context.JobApplieds.Remove(jobApplied);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobAppliedExists(Guid id)
        {
            return _context.JobApplieds.Any(e => e.JobAppliedId == id);
        }
    }
}
