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
using Hire360WebAPI.Helpers.CandidateJWT;
using Hire360WebAPI.Entities;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly ICandidateServices _candidateServices;

        public CandidateController(Hire360Context context, ICandidateServices candidateServices)
        {
            _context = context;
            _candidateServices = candidateServices;
        }

        // POST: api/candidate/login
        [HttpPost("login")]
        public IActionResult Login(AuthRequest model)
        {
            var candidate = _context.Candidates.FirstOrDefault(x => x.CandidateEmail == model.Email);
            if (candidate == null)
                return BadRequest(new { message = "Email not found!" });
            var verify = BCrypt.Net.BCrypt.Verify(model.Password, candidate!.CandidatePassword);
            if (!verify)
                return BadRequest(new { message = "Incorrect password!" });
            var response = _candidateServices.Authenticate(candidate);
            return Ok(response);
        }

        // GET: api/Candidate
        [HttpGet]
        [Authorize(Role.Candidate)]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            return await _context.Candidates.ToListAsync();
        }

        // GET: api/Candidate/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidate(Guid id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return candidate;
        }

        // PUT: api/Candidate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidate(Guid id, Candidate candidate)
        {
            if (id != candidate.CandidateId)
            {
                return BadRequest();
            }
            
            candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
            _context.Entry(candidate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(id))
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

        // POST: api/Candidate
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Candidate>> PostCandidate(Candidate candidate)
        {
            candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandidate", new { id = candidate.CandidateId }, candidate);
        }

        // DELETE: api/Candidate/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidate(Guid id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CandidateExists(Guid id)
        {
            return _context.Candidates.Any(e => e.CandidateId == id);
        }
    }
}
