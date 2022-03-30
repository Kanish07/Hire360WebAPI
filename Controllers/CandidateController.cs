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
using Hire360WebAPI.Entities;
using MimeKit;

namespace Hire360WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly ICandidateServices _candidateServices;
        private readonly IMailService mailService;

        public CandidateController(Hire360Context context, ICandidateServices candidateServices, IMailService mailService)
        {
            _context = context;
            _candidateServices = candidateServices;
            this.mailService = mailService;
        }

        // POST: api/candidate/login
        [HttpPost("login")]
        public IActionResult CandidateLogin(AuthRequest model)
        {
            try
            {
                var candidate = _context.Candidates.FirstOrDefault(x => x.CandidateEmail == model.Email);
                if (candidate == null)
                    return BadRequest(new { message = "Account does not exist" });
                var verify = BCrypt.Net.BCrypt.Verify(model.Password, candidate!.CandidatePassword);
                if (!verify)
                    return BadRequest(new { message = "Please Enter a correct Email and Password." });
                var response = _candidateServices.Authenticate(candidate);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Login Failed" });
            }
        }

        // GET: api/Candidate
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCandidates()
        {
            try
            {
                var candidate = await _context.Candidates.ToListAsync();
                return Ok(new { status = "success", data = candidate, message = "Get All Candidate Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Get All Candidate Data Failed" });
            }
        }

        // GET: api/Candidate/5
        [HttpGet("{id}")]
        [Authorize(Role.Candidate)]
        public async Task<ActionResult<Candidate>> GetCandidateById(Guid id)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(id);

                if (candidate == null)
                {
                    return Ok(new { status = "Failed", data = candidate, message = "No candidate Id found" });
                }

                // SEND CUSTOM EMAIL
                // var body = $"<h1>Hi {candidate.CandidateName}</h1><br><h3> Greetings from Hire360, Do check out the latest job postings on our site. Have a great day!</h3><br><br>";
                // var subject = "New Job Feeds Available";
                // await mailService.SendEmailAsync(candidate.CandidateEmail, subject, body);

                return Ok(new { status = "success", data = candidate, message = "Get candidate By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get candidate by Id Failed" });
            }
        }

        // PUT: api/Candidate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidateById(Guid id, Candidate candidate)
        {
            candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
            _context.Entry(candidate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!CandidateExists(id))
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return NotFound(new { status = "failed", message = "No Candidate Found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update Candidate By Id Failed" });
                }
            }
            return Ok(new { status = "success", message = "Details Updated" });
        }

        // POST: api/Candidate
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Candidate>> RegisterCandidate(Candidate candidate)
        {
            try
            {
                candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
                await mailService.SendWelcomeEmailAsync(candidate.CandidateEmail, candidate.CandidateName);

                return CreatedAtAction("GetCandidateById", new { id = candidate.CandidateId }, new { status = "success", data = candidate, message = "Candidate registration Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "User registration Failed" });
            }

        }

        // DELETE: api/Candidate/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidateById(Guid id)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(id);
                if (candidate == null)
                {
                    return Ok(new { status = "Failed", message = "Candidate Id not found" });
                }

                _context.Candidates.Remove(candidate);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", messsage = "Candidate Deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Candidate" });
            }
        }

        private bool CandidateExists(Guid id)
        {
            return _context.Candidates.Any(e => e.CandidateId == id);
        }
    }
}
