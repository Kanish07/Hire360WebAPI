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
        public async Task<IActionResult> GetAllJobApplieds()
        {
            try
            {
                var jobApplied = await _context.JobApplieds.ToListAsync();
                return Ok(new { status = "success", data = jobApplied, message = "Get all the job applied successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all job applied data failed" });
            }
        }

        // GET: api/JobApplied/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplied>> GetJobAppliedById(Guid id)
        {
            try
            {
                var jobApplied = await _context.JobApplieds.FindAsync(id);

                if (jobApplied == null)
                {
                    return NotFound(new { status = "failed", data = jobApplied, message = "No job applied id found" });
                }

                return Ok(new { status = "success", data = jobApplied, message = "Get job applied by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get job applied by id failed" });
            }
        }

        // PUT: api/JobApplied/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobAppliedById(Guid id, JobApplied jobApplied)
        {
            _context.Entry(jobApplied).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!JobAppliedExists(id))
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return NotFound(new { status = "failed", data = jobApplied, messsage = "Job applied id not found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update job applied by id failed" });
                }
            }
            return Ok(new { status = "success", messsage = "Details updated" });
        }

        // POST: api/JobApplied
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobApplied>> RegisterJobApplied(JobApplied jobApplied)
        {
            try
            {
                _context.JobApplieds.Add(jobApplied);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAllJobApplieds", new { id = jobApplied.JobAppliedId }, new { status = "success", data = jobApplied, message = "Job applied successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to create new job applied" });
            }
        }

        // DELETE: api/JobApplied/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobAppliedById(Guid id)
        {
            try
            {
                var jobApplied = await _context.JobApplieds.FindAsync(id);
                if (jobApplied == null)
                {
                    return NotFound(new { status = "failed", data = jobApplied, message = "JobApplieds id not found" });
                }

                _context.JobApplieds.Remove(jobApplied);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", data = jobApplied, messsage = "JobApplieds deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Faild to delete job applieds" });
            }
        }

        private bool JobAppliedExists(Guid id)
        {
            return _context.JobApplieds.Any(e => e.JobAppliedId == id);
        }
    }
}
