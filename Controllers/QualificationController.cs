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
                var qualification = await _context.JobApplieds.ToListAsync();
                return Ok(new { status = "Success", data = qualification, message = "Get All the qualification Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Get All qualification Data Failed" });
            }
        }

        // GET: api/Qualification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Qualification>> GetQualificationById(Guid id)
        {
            try
            {
                var qualification = await _context.Qualifications.FindAsync(id);

                if (qualification == null)
                {
                    return Ok(new { status = "Failed", data = qualification, message = "No Qualifications Id found" });
                }

                return Ok(new { status = "success", data = qualification, message = "Get Qualifications By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "failed", message = "Get Qualifications Failed" });

            }
        }

        // PUT: api/Qualification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualificationById(Guid id, Qualification qualification)
        {
            _context.Entry(qualification).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!QualificationExists(id))
                {
                    Console.WriteLine(ex);
                    return Ok(new { status = "Failed", data = qualification, messsage = "Qualification Id not available" });
                }
                else
                {
                    Console.WriteLine(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update qualification By Id Failed" });
                }
            }
            return Ok(new { status = "Success", messsage = "Details updated" });
        }

        // POST: api/Qualification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Qualification>> RegisterQualification(Qualification qualification)
        {
            try
            {
                _context.Qualifications.Add(qualification);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetQualification", new { id = qualification.QualificationId }, new { status = "Success", data = qualification, message = "qualification Applied Successfully" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Qualification" });
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
                    return Ok(new { status = "Failed", data = qualification, message = "Qualification Id not found" });
                }

                _context.Qualifications.Remove(qualification);
                await _context.SaveChangesAsync();
                return Ok(new { status = "Success", data = qualification, messsage = "Qualification deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Qualification" });
            }
        }

        private bool QualificationExists(Guid id)
        {
            return _context.Qualifications.Any(e => e.QualificationId == id);
        }
    }
}
