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
                return Ok(new { status = "Success", data = jobApplied, message = "Get All the Job Applied Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Get All Job Applied Data Failed" });
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
                    return Ok(new { status = "Failed", data = jobApplied, message = "No job Applied Id found" });
                }

                return Ok(new { status = "success", data = jobApplied, message = "Get job Applied By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "failed", message = "Get job Applied by Id Failed" });

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
                    return Ok(new { status = "Failed", data = jobApplied, messsage = "Job Applied Id not available" });
                }
                else
                {
                    Console.WriteLine(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update Job Applied By Id Failed" });
                }
            }
            return Ok(new { status = "Success", messsage = "Details Updated" });
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

                return CreatedAtAction("GetJobApplied", new { id = jobApplied.JobAppliedId }, new { status = "Success", data = jobApplied, message = "Job Applied Successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Job Applied" });
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
                    return Ok(new { status = "Failed", data = jobApplied, message = "JobApplieds Id not found" });
                }

                _context.JobApplieds.Remove(jobApplied);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = jobApplied, messsage = "JobApplieds deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Job Applieds" });
            }
        }

        private bool JobAppliedExists(Guid id)
        {
            return _context.JobApplieds.Any(e => e.JobAppliedId == id);
        }
    }
}
