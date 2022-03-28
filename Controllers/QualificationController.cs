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
            var qualification = await _context.Qualifications.FindAsync(id);

            if (qualification == null)
            {
                return NotFound();
            }

            return qualification;
        }

        // PUT: api/Qualification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQualification(Guid id, Qualification qualification)
        {
            if (id != qualification.QualificationId)
            {
                return BadRequest();
            }

            _context.Entry(qualification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QualificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Qualification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Qualification>> PostQualification(Qualification qualification)
        {
            _context.Qualifications.Add(qualification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQualification", new { id = qualification.QualificationId }, qualification);
        }

        // DELETE: api/Qualification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualification(Guid id)
        {
            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return NotFound();
            }

            _context.Qualifications.Remove(qualification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QualificationExists(Guid id)
        {
            return _context.Qualifications.Any(e => e.QualificationId == id);
        }
    }
}
