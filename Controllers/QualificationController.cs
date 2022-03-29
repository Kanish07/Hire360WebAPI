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
        public async Task<ActionResult<IEnumerable<Qualification>>> GetQualifications()
        {
            return await _context.Qualifications.ToListAsync();
        }

        // GET: api/Qualification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Qualification>> GetQualification(Guid id)
        {
            try
            {
                var qualification = await _context.Qualifications.FindAsync(id);

                if (qualification == null)
                {
                    return Ok(new { status = "Failed", data = qualification, message = "No Qualifications Id found in the give Id" });
                }

                return qualification;
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
        public async Task<IActionResult> PutQualification(Guid id, Qualification qualification)
        {
            try
            {
                if (id != qualification.QualificationId)
                {
                    return Ok(new { status = "Failed", data = qualification, message = "Qualification Id not found" });
                }

                _context.Entry(qualification).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QualificationExists(id))
                {
                    return Ok(new { status = "Failed", data = qualification, messsage = "Qualification Id is already available" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { status = "Failed", data = qualification, messsage = "Failed to Update the Qualification" });
        }

        // POST: api/Qualification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Qualification>> PostQualification(Qualification qualification)
        {
            try
            {
                _context.Qualifications.Add(qualification);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetQualification", new { id = qualification.QualificationId }, Ok(new { data = qualification }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { status = "Failed", message = "Failed to create new Qualification" });
            }
        }

        // DELETE: api/Qualification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualification(Guid id)
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
                return Ok(new { status = "Success", data = qualification, messsage = "Qualification Id has be deleted..." });
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
