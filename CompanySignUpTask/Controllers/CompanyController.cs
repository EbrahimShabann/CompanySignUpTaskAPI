using CompanySignUpTask.Data_Layer;
using CompanySignUpTask.Data_Layer.DTOs;
using CompanySignUpTask.Repository_Layer.IRepository;
using CompanySignUpTask.Service_Layer.CompanyServices;
using CompanySignUpTask.Service_Layer.EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace CompanySignUpTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICredintailsService credintailsService;
        private readonly IEmailService emailService;
        private readonly AppDbContext db;
        private readonly ICompanyRepo repo;

        public CompanyController(ICredintailsService credintailsService, IEmailService emailService, AppDbContext db,ICompanyRepo repo)
        {
            this.credintailsService = credintailsService;
            this.emailService = emailService;
            this.db = db;
            this.repo = repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] registerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var result = await credintailsService.register(dto);

                if (!result.success)
                    return BadRequest(result.message);

                await emailService.SendEmailAsync(
                    dto.Email,
                    "Your OTP Verification Code",
                    $"Your OTP Verification code is: <b>{result.otp}</b>"
                );

                await transaction.CommitAsync();
                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Registration failed. " + ex.Message);
            }

        }
        [HttpPost("validateOTP")]
        public IActionResult validateOTP([FromBody]validateOtpDto otpDto)
        {
            if (string.IsNullOrEmpty(otpDto.email) || string.IsNullOrEmpty(otpDto.otp))
                             return BadRequest("Email and OTP are required.");
          
            var isValid = credintailsService.ValidateOTP(otpDto.email, otpDto.otp);

            //return isValid ? Ok(new { msg="OTP is valid." }) : BadRequest(new { msg = "Invalid OTP." });
            if (isValid) 
            {
                return Ok(new { msg = "OTP is valid." });
            }
            else 
            { 
                 return BadRequest(new { msg = "Invalid OTP." });
            }
                


        }
  
        [HttpPost("setPassword")]
        public IActionResult setPassword(passwordDto passwordDto)
        {
            if (string.IsNullOrEmpty(passwordDto.email) || string.IsNullOrEmpty(passwordDto.password) 
                || string.IsNullOrEmpty(passwordDto.confirmPassword))
            {
                return BadRequest("Email, Password, and Confirm Password are required.");
            }
            if (passwordDto.password != passwordDto.confirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            bool res= credintailsService.setPassword(passwordDto);
            return res == true ? Ok(new {msg= "Password set succefully" }) : NotFound(new { msg = "Company not found" });
        }

        [HttpPost("login")]
        public IActionResult login(passwordDto loginDto)
        { 
            if(loginDto.email == null || loginDto.password == null)
            {
                return BadRequest("Email and Password are required.");
            }
            var result = credintailsService.login(loginDto);
            return result;
        }

        [HttpGet("getCompany")]
        public IActionResult getCompany(string email) 
        { 
            if(string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            var company = repo.GetByEmail(email);
            if(company == null)  return NotFound("Company not found.");
            return Ok(company);

        }


    }
}
