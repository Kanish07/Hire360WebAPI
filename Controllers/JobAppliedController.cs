#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hire360WebAPI.Models;
using Hire360WebAPI.Services;
using Hire360WebAPI.Helpers;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[Action]")]
    [ApiController]
    [Authorize]
    public class JobAppliedController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly IMailService _mailService;


        public JobAppliedController(Hire360Context context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        // GET: api/JobApplied
        [HttpGet]
        public async Task<IActionResult> GetAllJobsApplied()
        {
            try
            {
                var jobApplied = await _context.JobApplieds.Include(j => j.Candidate).Include(j => j.Job).ToListAsync();
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
                var jobApplied = await _context.JobApplieds.Where(j => j.JobAppliedId == id).Include(j => j.Candidate).Include(j => j.Job).FirstOrDefaultAsync();

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
            try
            {
                _context.Entry(jobApplied).State = EntityState.Modified;
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
        public async Task<ActionResult<JobApplied>> AddNewJobApplied(JobApplied jobApplied)
        {
            try
            {
                var jobAppliedExist = await _context.JobApplieds.Where((j) => j.JobId == jobApplied.JobId && j.CandidateId == jobApplied.CandidateId).FirstOrDefaultAsync();

                if (jobAppliedExist == null)
                {
                    var candidate = await _context.Candidates.Where(j => j.CandidateId == jobApplied.CandidateId).FirstOrDefaultAsync();
                    var job = await _context.Jobs.Where(j => j.JobId == jobApplied.JobId).Include(h => h.Hr).FirstOrDefaultAsync();
                    var body = $"<h3>Hi {candidate.CandidateName},</h3><br><h3> Greetings from Hire360, You have successfully submitted your application for {job.JobTitle}.<br>The recruiter may try to reach you at {candidate.CandidatePhoneNumber} (or) {candidate.CandidateEmail}<br></h3><h3>Regards,<br>Team Hire360</h3><br>";
                    var subject = $"Job applied at {job.Hr.CompanyName} for {job.JobTitle}";
                    _context.JobApplieds.Add(jobApplied);
                    await _context.SaveChangesAsync();
                    // SEND CUSTOM EMAIL
                    await _mailService.SendEmailAsync(candidate.CandidateEmail, subject, body);
                    return CreatedAtAction("GetJobAppliedById", new { id = jobApplied.JobAppliedId }, new { status = "success", data = jobApplied, message = "Job applied successfully" });
                }
                else
                {
                    return BadRequest(new { status = "failed", message = "Job already applied" });
                }

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
                    return NotFound(new { status = "failed", data = jobApplied, message = "JobApplied id not found" });
                }

                _context.JobApplieds.Remove(jobApplied);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", data = jobApplied, messsage = "JobApplied deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to delete job applied" });
            }
        }

        //GET: api/JobApplied/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobAppliedByCandidateId(Guid id)
        {
            try
            {
                var jobAppliedByCandidate = await _context.JobApplieds.Where((j) => j.CandidateId == id).Include(j => j.Job).ToListAsync();
                return Ok(new { status = "success", data = jobAppliedByCandidate, message = "Get all job applied by candidate successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all job applied by candidate failed" });
            }
        }

        // Get the jobs applied by jobId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobAppliedByJobId(Guid id)
        {
            try
            {
                var jobExists = await _context.Jobs.FindAsync(id);
                if (jobExists == null)
                {
                    return NotFound(new { status = "failed", message = "Job not found"});
                }
                var jobApplied = await _context.JobApplieds.Where((j) => j.JobId == id).Include((c) => c.Candidate).ToListAsync();
                return Ok(new { status = "success", data = jobApplied, message = "Get all job applied by job id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all job applied by job id failed" });
            }
        }

        private bool JobAppliedExists(Guid id)
        {
            return _context.JobApplieds.Any(e => e.JobAppliedId == id);
        }
    }
}
