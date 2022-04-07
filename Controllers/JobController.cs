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
                var count = _context.Jobs.Count();
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

                return Ok(new { status = "success", data = new { job = job, isApplied = jobAlreadyApplied != null }, message = "Get job applied by id successful" });
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
        public async Task<IActionResult> GetJobAddedByHrId(Guid id)
        {
            try
            {
                var HrExist = await _context.HumanResources.FindAsync(id);
                if (HrExist == null)
                {
                    return NotFound(new { status = "failed", message = "Hr not found" });
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

        //Filter Job API
        [HttpGet("{candidateId}/{page}")]
        public async Task<IActionResult> GetAllJobsBasedOnFilter(Guid candidateId, int page, [FromQuery] string city, [FromQuery] string role, [FromQuery] string salarylow, [FromQuery] string salaryhigh)
        {
            try
            {
                var jobAlreadyApplied = await _context.Candidates.Where(c => c.CandidateId.Equals(candidateId)).Include(c => c.JobApplieds).FirstOrDefaultAsync();
                var applied = new List<Guid>();
                var appliedAsString = new List<string>();
                foreach (var item in jobAlreadyApplied.JobApplieds)
                {
                    applied.Add(item.JobId);
                    appliedAsString.Add(item.JobId.ToString());
                }


                var cityArr = city == null || city == "" ? new string[100] : city.Split(',');
                var roleArr = role == null || role == "" ? new string[100] : role.Split(',');
                var low = salarylow == null || salarylow == "" ? 0 : int.Parse(salarylow);
                var high = salaryhigh == null || salaryhigh == "" ? 999999999 : int.Parse(salaryhigh);
                var pageNumber = (page - 1) * 10;
                var job = new List<Job>();
                var jobCount = 1;

                if (city != null && city != "" || role != null && role != "")
                {
                    if (city != null && city != "" && role != null && role != "")
                    {
                        job = await _context.Jobs.Where(j => cityArr.Contains(j.JobCity) && roleArr.Contains(j.JobTitle)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Skip(pageNumber).Take(10).ToListAsync();
                        jobCount = _context.Jobs.Where(j => cityArr.Contains(j.JobCity) && roleArr.Contains(j.JobTitle)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Count();
                    }
                    else if (city != null && city != "")
                    {
                        job = await _context.Jobs.Where(j => cityArr.Contains(j.JobCity)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Skip(pageNumber).Take(10).ToListAsync();
                        jobCount = _context.Jobs.Where(j => cityArr.Contains(j.JobCity)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Count();
                    }
                    else
                    {
                        job = await _context.Jobs.Where(j => roleArr.Contains(j.JobTitle)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Skip(pageNumber).Take(10).ToListAsync();
                        jobCount = _context.Jobs.Where(j => roleArr.Contains(j.JobTitle)).Where(j => j.Package >= low && j.Package <= high).Include(j => j.Hr).Count();
                    }
                }
                else
                {
                    job = await _context.Jobs.Include(j => j.Hr).Where(j => j.Package >= low && j.Package <= high).Skip(pageNumber).Take(10).ToListAsync();
                    jobCount = _context.Jobs.Include(j => j.Hr).Where(j => j.Package >= low && j.Package <= high).Count();
                }

                var jobList = new List<Job>();
                foreach (var item in job)
                {
                    if (!appliedAsString.Contains(item.JobId.ToString()))
                    {
                        jobList.Add(item);
                    }
                }
                return Ok(new { status = "success", data = jobList, count = jobCount, message = "Get all jobs successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all jobs failed" });
            }
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
