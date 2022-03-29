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
using Hire360WebAPI.Helpers.HRJWT;
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
                    return BadRequest(new { message = "Account does not exist" });
                var verify = BCrypt.Net.BCrypt.Verify(model.Password, hr!.Hrpassword);
                if (!verify)
                    return BadRequest(new { message = "Please Enter a correct Email and Password." });
                var response = _humanResourceServices.Authenticate(hr);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Login Failed" });
            }
        }

        // GET: api/HumanResource
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllHumanResources()
        {
            try
            {
                var humanResource = await _context.HumanResources.ToListAsync();
                return Ok(new { status = "Success", data = humanResource, message = "Get All Human Resources" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return Ok(new { status = "Failed", message = "Get all Human Resources data failed" });
            }
        }

        // GET: api/HumanResource/5
        [HttpGet("{id}")]
        [Authorize(Role.HR)]
        public async Task<ActionResult<HumanResource>> GetHumanResourceById(Guid id)
        {
            try
            {
                var humanResource = await _context.HumanResources.FindAsync(id);

                if (humanResource == null)
                {
                    return Ok(new { status = "Failed", data = humanResource, message = "No Human Resource Id found" });
                }

                return Ok(new { status = "success", data = humanResource, message = "Get Human Resource By Id Successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get Human Resource by Id Failed" });

            }
        }

        // PUT: api/HumanResource/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHumanResourceById(Guid id, HumanResource humanResource)
        {
            humanResource.Hrpassword = BCrypt.Net.BCrypt.HashPassword(humanResource.Hrpassword);
            _context.Entry(humanResource).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                if (!HumanResourceExists(id))
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return Ok(new { status = "Failed", data = humanResource, messsage = "NO Human Resource Found" });
                }
                else
                {
                    Console.WriteLine(ex);
                    Sentry.SentrySdk.CaptureException(ex);
                    return Ok(new { status = "Failed", data = humanResource, serverMessage = ex.Message, messsage = "Update Human Resource by Id failed" });
                }
            }
            return Ok(new { status = "Success", messsage = "Details Updated" });
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

                return CreatedAtAction("GetAllHumanResources", new { id = humanResource.Hrid }, new { status = "Success", data = humanResource, message = "Human registration successful" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", serverMessage = ex.Message, message = "Human Resource registration Failed" });
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
                    return Ok(new { status = "Failed", message = "Human Resource Id not found" });
                }

                _context.HumanResources.Remove(humanResource);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", messsage = "Human Resource deleted" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Faild to delete Human Resource" });
            }
        }

        private bool HumanResourceExists(Guid id)
        {
            return _context.HumanResources.Any(e => e.Hrid == id);
        }
    }
}
