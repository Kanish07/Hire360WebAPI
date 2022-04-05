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
    [Route("api/[Action]")]
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
                var job = await _context.Jobs.Include(j => j.Hr).ToListAsync();
                return Ok(new { status = "success", data = job, message = "Get all jobs successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all jobs failed" });
            }
        }

        // GET: api/Job/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            try
            {
                var job = await _context.Jobs.Where(j => j.JobId == id).Include(j => j.Hr).FirstOrDefaultAsync();

                if (job == null)
                {
                    return NotFound(new { status = "failed", data = job, message = "No job id found" });
                }

                return Ok(new { status = "success", data = job, message = "Get job applied by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get job applied by id failed" });
            }
        }

        // GET: api/Job/5
        [HttpGet("{id}/{candidateId}")]
        public async Task<IActionResult> GetJobByIdCheckIfAlreadyApplied(Guid id, Guid candidateId)
        {
            try
            {
                var jobAlreadyApplied = await _context.JobApplieds.Where(j => j.JobId == id && j.CandidateId == candidateId).FirstOrDefaultAsync();
                var job = await _context.Jobs.Where(j => j.JobId == id).Include(j => j.Hr).FirstOrDefaultAsync();

                if (job == null)
                {
                    return NotFound(new { status = "failed", data = job, message = "No job id found" });
                }

                return Ok(new { status = "success", data = new {job = job, isApplied = jobAlreadyApplied != null}, message = "Get job applied by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get job applied by id failed" });
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
                    return NotFound(new { status = "failed", data = job, messsage = "Job id not found" });
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
        public async Task<ActionResult<Job>> AddNewJob(Job job)
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
                return BadRequest(new { status = "failed", message = "Failed to delete job" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobAddedByHrId(Guid id){
            try
            {
                var HrExist = await _context.HumanResources.FindAsync(id);
                if (HrExist == null)
                {
                    return NotFound(new { status = "failed", message = "Hr not found"});
                }
                var job = await _context.Jobs.Where((j) => j.Hrid == id).Include(h => h.Hr).ToListAsync();
                return Ok(new { status = "success", data = job, message = "Get all the job added by Hr successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all the job added by Hr failed" });
            }
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
