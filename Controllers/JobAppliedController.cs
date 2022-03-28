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
            try
            {
                var jobApplied = await _context.JobApplieds.FindAsync(id);

                if (jobApplied == null)
                {
                    return Ok(new { status = "Failed", data = jobApplied, message = "No jobApplied Id found in the give Id" });
                }

                return jobApplied;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "failed", message = "Get jobApplied Failed" });

            }
        }

        // PUT: api/JobApplied/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobApplied(Guid id, JobApplied jobApplied)
        {
            try
            {
                if (id != jobApplied.JobAppliedId)
                {
                    return Ok(new { status = "Failed", data = jobApplied, message = "Job Applied Id not found" });
                }

                _context.Entry(jobApplied).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobAppliedExists(id))
                {
                    return Ok(new { status = "Failed", data = jobApplied, messsage = "Job Applied Id is already available" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { status = "Failed", data = jobApplied, messsage = "Failed to Update the Job Applied" });
        }

        // POST: api/JobApplied
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobApplied>> PostJobApplied(JobApplied jobApplied)
        {
            try
            {
                _context.JobApplieds.Add(jobApplied);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetJobApplied", new { id = jobApplied.JobAppliedId }, Ok(new { data = jobApplied }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Job Applied" });
            }
        }

        // DELETE: api/JobApplied/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplied(Guid id)
        {
            try
            {
                var jobApplied = await _context.JobApplieds.FindAsync(id);
                if (jobApplied == null)
                {
                    return Ok(new { status = "Failed", data = jobApplied, message = "JobApplieds Id not found" });
                }

                _context.JobApplieds.Remove(jobApplied);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = jobApplied, messsage = "JobApplieds Id has be deleted..." });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete JobApplieds" });
            }
        }

        private bool JobAppliedExists(Guid id)
        {
            return _context.JobApplieds.Any(e => e.JobAppliedId == id);
        }
    }
}
