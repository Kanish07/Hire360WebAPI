#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
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
    [Route("api/[Action]")]
    [ApiController]
    // [Authorize]
    public class CandidateController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly ICandidateServices _candidateServices;
        private readonly IMailService _mailService;
        private readonly IAzureStorage _azurestorage;

        public CandidateController(Hire360Context context, ICandidateServices candidateServices, IMailService mailService, IAzureStorage azureStorage)
        {
            _context = context;
            _azurestorage = azureStorage;
            _candidateServices = candidateServices;
            _mailService = mailService;
        }

        // POST: api/candidate/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult CandidateLogin(AuthRequest model)
        {
            try
            {
                var candidate = _context.Candidates.FirstOrDefault(x => x.CandidateEmail == model.Email);
                if (candidate == null)
                    return NotFound(new { message = "Account does not exist" });
                var verify = BCrypt.Net.BCrypt.Verify(model.Password, candidate!.CandidatePassword);
                if (!verify)
                    return BadRequest(new { message = "Please enter correct email and password." });
                var response = _candidateServices.Authenticate(candidate);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Login failed" });
            }
        }

        // GET: api/Candidate
        [HttpGet]
        public async Task<IActionResult> GetAllCandidates()
        {
            try
            {
                var candidate = await _context.Candidates.ToListAsync();
                return Ok(new { status = "success", data = candidate, message = "Get all candidate successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all candidate data failed" });
            }
        }

        // GET: api/Candidate/5
        [HttpGet("{candidateId}")]
        public async Task<ActionResult<Candidate>> GetCandidateById(Guid candidateId)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(candidateId);

                if (candidate == null)
                {
                    return NotFound(new { status = "failed", data = candidate, message = "No candidate found" });
                }

                return Ok(new { status = "success", data = candidate, message = "Get candidate by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get candidate by id failed" });
            }
        }

        // PUT: api/Candidate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{candidateId}")]
        public async Task<IActionResult> UpdateCandidateById(Guid candidateId, Candidate candidate)
        {
            try
            {
                candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
                _context.Entry(candidate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!CandidateExists(candidateId))
                {
                    return NotFound(new { status = "failed", message = "No candidate found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update candidate by id failed" });
                }
            }
            return Ok(new { status = "success", message = "Details updated" });
        }

        // POST: api/Candidate
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Candidate>> AddNewCandidate(Candidate candidate)
        {
            try
            {
                candidate.CandidatePassword = BCrypt.Net.BCrypt.HashPassword(candidate.CandidatePassword);
                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
                var subject = $"Welcome to Hire360";
                var body = $"<h3>Hi {candidate.CandidateName},</h3><br><h3>Greetings from Hire360, Thank you for signing up to Hire360!</h3><br><h3>Regards,<br>Team Hire360</h3><br>";
                // await _mailService.SendWelcomeEmailAsync(candidate.CandidateEmail, candidate.CandidateName);
                await _mailService.SendEmailAsync(candidate.CandidateEmail, subject, body);
                return CreatedAtAction("GetCandidateById", new { id = candidate.CandidateId }, new { status = "success", data = candidate, message = "Candidate registration Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "User registration failed" });
            }

        }

        // DELETE: api/Candidate/5
        [HttpDelete("{candidateId}")]
        public async Task<IActionResult> DeleteCandidateById(Guid candidateId)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(candidateId);
                if (candidate == null)
                {
                    return NotFound(new { status = "failed", message = "Candidate id not found" });
                }

                _context.Candidates.Remove(candidate);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", messsage = "Candidate deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Failed to delete candidate" });
            }
        }

        // File Upload
        [HttpPost("{candidateId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadResume(Guid candidateId)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileURL = await _azurestorage.UploadAsync(file.OpenReadStream(), fileName, file.ContentType);
                    if (fileURL != null)
                    {
                        var candidate = await _context.Candidates.FindAsync(candidateId);
                        candidate.CandidateResume = fileURL;
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new { fileURL });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // File Upload
        [HttpPost("{candidateId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadProfilePicture(Guid candidateId)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileURL = await _azurestorage.UploadAsync(file.OpenReadStream(), fileName, file.ContentType);
                    if (fileURL != null)
                    {
                        var candidate = await _context.Candidates.FindAsync(candidateId);
                        candidate.CandidatePhotoUrl = fileURL;
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new { fileURL });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //Update Description
        [HttpGet("{candidateId}")]
        [Authorize(Role.Candidate)]
        public async Task<IActionResult> UpdateCandidateDescriptionById(Guid candidateId, string description)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(candidateId);
                candidate.CandidateDescription = description;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!CandidateExists(candidateId))
                {
                    return NotFound(new { status = "failed", message = "No candidate found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Update candidate description by id failed" });
                }
            }
            return Ok(new { status = "success", message = "Description updated" });
        }

        private bool CandidateExists(Guid candidateId)
        {
            return _context.Candidates.Any(e => e.CandidateId == candidateId);
        }
    }
}
