namespace CompanySignUpTask.Data_Layer.DTOs
{
    public class registerDto
    {
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WebsiteUrl { get; set; }
        public IFormFile logo { get; set; }
    }
}
