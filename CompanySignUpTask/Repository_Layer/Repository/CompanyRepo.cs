using CompanySignUpTask.Data_Layer;
using CompanySignUpTask.Data_Layer.Models;
using CompanySignUpTask.Repository_Layer.IRepository;

namespace CompanySignUpTask.Repository_Layer.Repository
{
    public class CompanyRepo : ICompanyRepo
    {
        private readonly AppDbContext db;

        public CompanyRepo(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(Company entity)
        {
            db.Companies.Add(entity);
        }

      
        public List<Company> GetAll()
        {
           return  db.Companies.ToList();
        }

        public Company GetByEmail(string email)
        {
           return GetAll().SingleOrDefault(c=>c.Email==email);
        }

        public Company GetById(string id)
        {
            return GetAll().SingleOrDefault(c=>c.Id==id);
        }

        public bool IsValidEmail(string email)
        {
            var companyFromDb=db.Companies.FirstOrDefault(c=>c.Email==email);
            return companyFromDb == null;
        }

        public void save()
        {
            db.SaveChanges();
        }

        public void Update(Company entity)
        {
            db.Entry(entity).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
