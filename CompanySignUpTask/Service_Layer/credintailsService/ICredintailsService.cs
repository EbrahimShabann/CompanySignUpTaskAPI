using CompanySignUpTask.Data_Layer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CompanySignUpTask.Service_Layer.CompanyServices
{
    public interface ICredintailsService
    {
        Task<(bool success, string message, string otp)> register(registerDto dto); 
        bool ValidateOTP(string email, string otp);
        bool setPassword(passwordDto dto);
        ActionResult login(passwordDto dto);
    }
}
