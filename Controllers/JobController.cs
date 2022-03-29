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
                return Ok(new { status = "Success", data = job, message = "Get All the Job Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Get All Job Data Failed" });
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
                    return Ok(new { status = "Failed", data = job, message = "No Job Id found " });
                }

                return Ok(new { status = "success", data = job, message = "Get job Applied By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get job Applied by Id Failed" });
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
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return Ok(new { status = "Failed", data = job, messsage = "Job Id is already available" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update Job By Id Failed" });
                }
            }
            return Ok(new { status = "Failed", data = job, messsage = "Failed to Update the Job" });
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

                return CreatedAtAction("GetJobById", new { id = job.JobId }, new { status = "Success", data = job, message = "Job registered Successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Job" });
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
                    return Ok(new { status = "Failed", data = job, message = "Job Id not found" });
                }
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                return Ok(new { status = "Success", data = job, messsage = "Job deleted..." });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Job" });
            }
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
