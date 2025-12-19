using System.Collections.Generic;

namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface ICompanyService
    {
        Domain.CompanyLister GetAll(Domain.CompanyLister mLister);

        List<Domain.Company> GetActiveCompanies();

        Domain.Company Upsert(Domain.Company mCompany);

        Domain.Company Get(int id);

        void Delete(int id);
    }
}
