using CompanySignUpTask.Data_Layer.DTOs;
using CompanySignUpTask.Data_Layer.Models;
using CompanySignUpTask.Repository_Layer.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompanySignUpTask.Service_Layer.CompanyServices
{
    public class CredintailsService : ICredintailsService
    {
        private readonly ICompanyRepo companyRepo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CredintailsService(ICompanyRepo companyRepo,IWebHostEnvironment _webHostEnvironment)
        {
            this.companyRepo = companyRepo;
            webHostEnvironment = _webHostEnvironment;
        }
        public async Task<(bool success, string message, string otp)> register(registerDto dto)
        {
            var isValidEmail = companyRepo.IsValidEmail(dto.Email);
            string uniqueFileName="";
            if (!isValidEmail)
            {
                return (false, "Email already exists", "");
            }
            if (dto.logo != null && dto.logo.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "logos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.logo.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.logo.CopyToAsync(stream);
                }

              
            }
            string Otp = GenerateOTP();

            var newCompany = new Company
            {
                NameArabic = dto.NameArabic,
                NameEnglish = dto.NameEnglish,
                Email = dto.Email,
                Phone = dto.Phone,
                WebsiteUrl = dto.WebsiteUrl,
                LogoUrl = "/logos/" + uniqueFileName, // this will be the public URL,
                OTPCode = Otp,
                OTPGeneratedAt = DateTime.UtcNow
            };

            companyRepo.Add(newCompany);
            companyRepo.save();

              return (true, "Registration successful", Otp);
        }



        private string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999); // Generate a 6-digit OTP
            return otp.ToString();
        }

        public bool ValidateOTP(string email, string otp)
        {
            Company company = companyRepo.GetByEmail(email);
            if (company == null)
            {
                return false; // Email not found
            }
            // Check if the OTP matches and is within the valid time frame (e.g., 5 minutes)
            bool otpIsValid = company.OTPCode == otp &&
                             (DateTime.UtcNow - company.OTPGeneratedAt).TotalMinutes <= 5;
            if (otpIsValid)
            {
                // Clear OTP after successful validation
                company.OTPCode = null;
                company.OTPGeneratedAt = DateTime.MinValue;
                companyRepo.Update(company);
                companyRepo.save();
            }
            return otpIsValid;
        }

        public bool setPassword(passwordDto dto)
        {
            var company = companyRepo.GetByEmail(dto.email);
            if (company == null)
            {
                return false;
            }
            var hasher = new PasswordHasher<string>();
            company.PasswordHash = hasher.HashPassword("", dto.password);
            companyRepo.Update(company);
            companyRepo.save();
            return true;
        }

        public ActionResult login(passwordDto loginDto)
        {
            var company=companyRepo.GetByEmail(loginDto.email);
            if (company == null)
                return new NotFoundObjectResult(new { msg = "Company not found" });
            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword("", company.PasswordHash, loginDto.password);
            if(loginDto.email == company.Email && result == PasswordVerificationResult.Success)
            {
                return new OkObjectResult(company);
            }
            else
            {
                return new BadRequestObjectResult(new { msg = "Invalid email or password" });
            }

        }
    }
}
