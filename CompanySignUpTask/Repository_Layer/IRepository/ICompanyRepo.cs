using CompanySignUpTask.Data_Layer.Models;

namespace CompanySignUpTask.Repository_Layer.IRepository
{
    public interface ICompanyRepo
    {
        List<Company> GetAll();
        Company GetById(string id);
        Company GetByEmail(string email);
        void Add(Company entity);
        void Update(Company entity);
        bool IsValidEmail(string email);
        void save();
    }
}
