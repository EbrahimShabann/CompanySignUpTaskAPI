using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CompanySignUpTask.Data_Layer.Models
{
    public class Company
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(255)]
        public string NameEnglish { get; set; }

        [Required]
        [MaxLength(300)]
        public string NameArabic { get; set; }


        [Required]
        [EmailAddress]
        [Remote("IsValidEmail", "CustomValidation")]
        public string Email { get; set; }

        [RegularExpression(@"^(?:\+20|0020|0)?1[0125]\d{8}$",
          ErrorMessage = "Phone Number must be in one of these formats: +20xxxxxxxxxx, 0020xxxxxxxxxx, or 0xxxxxxxxxx")]
        public string Phone { get; set; }

        public string WebsiteUrl{ get; set; }
        public string LogoUrl{ get; set; }
        public string PasswordHash{ get; set; }
        public string OTPCode{ get; set; }
        public DateTime OTPGeneratedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
