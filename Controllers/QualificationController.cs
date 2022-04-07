#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hire360WebAPI.Models;
using Hire360WebAPI.Helpers;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[Action]")]
    [ApiController]
    [Authorize]
    public class QualificationController : ControllerBase
    {
        private readonly Hire360Context _context;

        public QualificationController(Hire360Context context)
        {
            _context = context;
        }

        // GET: api/Qualification
        [HttpGet]
        public async Task<IActionResult> GetAllQualifications()
        {
            try
            {
                var qualification = await _context.Qualifications.Include(q => q.Candidate).ToListAsync();
                return Ok(new { status = "success", data = qualification, message = "Get all qualifications successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all qualification data failed" });
            }
        }

        // GET: api/Qualification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Qualification>> GetQualificationById(Guid id)
        {
            try
            {
                var qualification = await _context.Qualifications.Where(q => q.QualificationId == id).Include(c => c.Candidate).FirstOrDefaultAsync();

                if (qualification == null)
                {
                    return NotFound(new { status = "failed", data = qualification, message = "Qualification id not found" });
                }
                return Ok(new { status = "success", data = qualification, message = "Get qualification by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get qualification failed" });
            }
        }

        // PUT: api/Qualification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualificationById(Guid id, Qualification qualification)
        {
            try
            {
                _context.Entry(qualification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!QualificationExists(id))
                {
                    return NotFound(new { status = "failed", data = qualification, messsage = "Qualification id not found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update qualification by id failed" });
                }
            }
            return Ok(new { status = "success", messsage = "Details updated" });
        }

        // POST: api/Qualification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Qualification>> AddNewQualification(Qualification qualification)
        {
            try
            {
                _context.Qualifications.Add(qualification);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetQualificationById", new { id = qualification.QualificationId }, new { status = "success", data = qualification, message = "Qualification applied successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to create new qualification" });
            }
        }

        // DELETE: api/Qualification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualificationById(Guid id)
        {
            try
            {
                var qualification = await _context.Qualifications.FindAsync(id);
                if (qualification == null)
                {
                    return NotFound(new { status = "failed", data = qualification, message = "Qualification id not found" });
                }

                _context.Qualifications.Remove(qualification);
                await _context.SaveChangesAsync();
                return Ok(new { status = "success", data = qualification, messsage = "Qualification deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to delete qualification" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Qualification>> GetQualificationByCandidateId(Guid id)
        {
            try
            {
                var qualification = await _context.Qualifications.Where(q => q.CandidateId == id).ToListAsync();
                return Ok(new { status = "success", data = qualification, messsage = "Qualification by candidate id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Qualification by candidate id failed" });
            }
        }

        private bool QualificationExists(Guid id)
        {
            return _context.Qualifications.Any(e => e.QualificationId == id);
        }
    }
}
