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

namespace Hire360WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HumanResourceController : ControllerBase
    {
        private readonly Hire360Context _context;
        private readonly IHumanResourceServices _humanResourceServices;

        public HumanResourceController(Hire360Context context, IHumanResourceServices humanResourceServices)
        {
            _context = context;
            _humanResourceServices = humanResourceServices;
        }

        // POST: api/candidate/login
        [HttpPost("login")]
        public IActionResult HrLogin(AuthRequest model)
        {
            try
            {
                var hr = _context.HumanResources.FirstOrDefault(x => x.Hremail == model.Email);
                if (hr == null)
                    return NotFound(new { message = "Account does not exist" });
                var verify = BCrypt.Net.BCrypt.Verify(model.Password, hr!.Hrpassword);
                if (!verify)
                    return BadRequest(new { message = "Please enter a correct email and password." });
                var response = _humanResourceServices.Authenticate(hr);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Login failed" });
            }
        }

        // GET: api/HumanResource
        [HttpGet]
        // [Authorize]
        public async Task<IActionResult> GetAllHumanResources()
        {
            try
            {
                var humanResource = await _context.HumanResources.ToListAsync();
                return Ok(new { status = "success", data = humanResource, message = "Get all human resources" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get all human resources data failed" });
            }
        }

        // GET: api/HumanResource/5
        [HttpGet("{id}")]
        // [Authorize(Role.HR)]
        public async Task<ActionResult<HumanResource>> GetHumanResourceById(Guid id)
        {
            try
            {
                var humanResource = await _context.HumanResources.FindAsync(id);

                if (humanResource == null)
                {
                    return NotFound(new { status = "failed", data = humanResource, message = "No human resource id found" });
                }

                return Ok(new { status = "success", data = humanResource, message = "Get human resource by id successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get human resource by id failed" });

            }
        }

        // PUT: api/HumanResource/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHumanResourceById(Guid id, HumanResource humanResource)
        {
            humanResource.Hrpassword = BCrypt.Net.BCrypt.HashPassword(humanResource.Hrpassword);
            try
            {
                _context.Entry(humanResource).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!HumanResourceExists(id))
                {
                    return NotFound(new { status = "failed", data = humanResource, messsage = "No human resource found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return BadRequest(new { status = "failed", data = humanResource, serverMessage = ex.Message, messsage = "Update human resource by id failed" });
                }
            }
            return Ok(new { status = "success", messsage = "Details updated" });
        }

        // POST: api/HumanResource
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HumanResource>> RegisterHumanResource(HumanResource humanResource)
        {
            try
            {
                humanResource.Hrpassword = BCrypt.Net.BCrypt.HashPassword(humanResource.Hrpassword);
                _context.HumanResources.Add(humanResource);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAllHumanResources", new { id = humanResource.Hrid }, new { status = "success", data = humanResource, message = "Human resource registration successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", serverMessage = ex.Message, message = "Human resource registration failed" });
            }
        }

        // DELETE: api/HumanResource/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHumanResourceById(Guid id)
        {
            try
            {
                var humanResource = await _context.HumanResources.FindAsync(id);
                if (humanResource == null)
                {
                    return NotFound(new { status = "failed", message = "Human resource id not found" });
                }

                _context.HumanResources.Remove(humanResource);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", messsage = "Human resource deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Faild to delete Human Resource" });
            }
        }

        private bool HumanResourceExists(Guid id)
        {
            return _context.HumanResources.Any(e => e.Hrid == id);
        }
    }
}
