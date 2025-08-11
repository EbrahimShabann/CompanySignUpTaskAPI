using CompanySignUpTask.Repository_Layer.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace CompanySignUpTask.Controllers
{
    public class CustomValidationController : Controller
    {
        private readonly ICompanyRepo companyRepo;

        public CustomValidationController(ICompanyRepo companyRepo)
        {
            this.companyRepo = companyRepo;
        }
        public IActionResult IsValidEmail(string Email)
        {
            return Json(companyRepo.IsValidEmail(Email));
        }
    }
}
