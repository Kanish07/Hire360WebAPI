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
    public class JobController : ControllerBase
    {
        private readonly Hire360Context _context;

        public JobController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/Job
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var job = await _context.JobApplieds.ToListAsync();
                return Ok(new { status = "success", data = job, message = "Get all the job successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all job data failed" });
            }
        }

        // GET: api/Job/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJobById(Guid id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);

                if (job == null)
                {
                    return NotFound(new { status = "failed", data = job, message = "No job id found " });
                }

                return Ok(new { status = "success", data = job, message = "Get job applied by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get job applied by id Failed" });
            }
        }

        // PUT: api/Job/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobById(Guid id, Job job)
        {
            _context.Entry(job).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!JobExists(id))
                {
                    return BadRequest(new { status = "failed", data = job, messsage = "Job id not found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update job by id failed" });
                }
            }
            return Ok(new { status = "success", data = job, messsage = "Details updated" });
        }

        // POST: api/Job
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Job>> RegisterJob(Job job)
        {
            try
            {
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetJobById", new { id = job.JobId }, new { status = "success", data = job, message = "Job registered successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to create new job" });
            }
        }

        // DELETE: api/Job/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobById(Guid id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    return NotFound(new { status = "failed", data = job, message = "Job id not found" });
                }
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                return Ok(new { status = "success", data = job, messsage = "Job deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Faild to delete job" });
            }
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
