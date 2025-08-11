namespace CompanySignUpTask.Service_Layer.EmailService
{
    public interface IEmailService
    {
        Task  SendEmailAsync(string to, string subject, string body);
    }
}
