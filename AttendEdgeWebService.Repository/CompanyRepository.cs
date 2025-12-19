using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        public List<Domain.Company> GetActiveCompanies()
        {
            var mCompany = new List<Domain.Company>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efCompanies = from company in context.Companies
                                      where company.DeletedDate == null && company.DeletedBy == null && company.IsActive == true
                                      select new
                                      {
                                          Id = company.Id,
                                          Name = company.Name,
                                          Code = company.Code,
                                      };

                    mCompany = JsonSerializer.Deserialize<List<Domain.Company>>(JsonSerializer.Serialize(efCompanies.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mCompany;
        }

        public Domain.CompanyLister GetAll(Domain.CompanyLister mLister)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var query = from comp in context.Companies
                                where comp.DeletedDate == null && comp.DeletedBy == null
                                select new
                                {
                                    Id = comp.Id,
                                    Code = comp.Code,
                                    Name = comp.Name,
                                    ContactPerson = comp.ContactPerson,
                                    Line1 = comp.Line1,
                                    Line2 = comp.Line2,
                                    City = comp.City,
                                    State = comp.State,
                                    EmailAddress = comp.EmailAddress,
                                    MobileNumber = comp.MobileNumber,
                                    IsActive = comp.IsActive
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.Code.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Code) && x.Code.ToUpper().Contains(criteria.Code.ToUpper()));

                        if (criteria.Line1.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Line1) && x.Line1.ToUpper().Contains(criteria.Line1.ToUpper()));

                        if (criteria.City.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.City) && x.City.ToUpper().Contains(criteria.City.ToUpper()));

                        if (criteria.State.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.State) && x.State.ToUpper().Contains(criteria.State.ToUpper()));

                        if (criteria.ContactPerson.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.ContactPerson) && x.ContactPerson.ToUpper().Contains(criteria.ContactPerson.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }

                    // Switch to in-memory for complex string filters
                    var efCompanies = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = efCompanies.Count();

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efCompanies = efCompanies.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Company>>(JsonSerializer.Serialize(efCompanies.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.Company Upsert(Domain.Company mCompany)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efCompany = context.Companies.Where(x => x.Id == mCompany.Id && x.DeletedBy == null && x.DeletedDate == null).FirstOrDefault();
                    if (efCompany == null || efCompany.Id <= default(int))
                    {
                        efCompany = new Persistence.Company();
                        efCompany.CreatedBy = mCompany.CreatedBy;
                        efCompany.CreatedDate = DateTime.Now;
                        context.Companies.Add(efCompany);
                    }

                    efCompany.Code = mCompany.Code;
                    efCompany.Name = mCompany.Name;
                    efCompany.ContactPerson = mCompany.ContactPerson;
                    efCompany.Line1 = mCompany.Line1;
                    efCompany.Line2 = mCompany.Line2;
                    efCompany.City = mCompany.City;
                    efCompany.State = mCompany.State;
                    efCompany.EmailAddress = mCompany.EmailAddress;
                    efCompany.MobileNumber = mCompany.MobileNumber;
                    efCompany.Logo = mCompany.Logo;
                    efCompany.IsActive = mCompany.IsActive;
                    efCompany.LastModifiedBy = mCompany.LastModifiedBy;
                    efCompany.LastModifiedDate = DateTime.Now;

                    context.SaveChanges();

                    mCompany.Id = efCompany.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCompany;
        }

        public Domain.Company Get(int id)
        {
            var mCompany = new Domain.Company();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efCompany = (from c in context.Companies
                                     where c.DeletedDate == null && c.DeletedBy == null && c.Id == id
                                     select new
                                     {
                                         Id = c.Id,
                                         Code = c.Code,
                                         Name = c.Name,
                                         ContactPerson = c.ContactPerson,
                                         Line1 = c.Line1,
                                         Line2 = c.Line2,
                                         City = c.City,
                                         State = c.State,
                                         EmailAddress = c.EmailAddress,
                                         MobileNumber = c.MobileNumber,
                                         Logo = c.Logo,
                                         IsActive = c.IsActive
                                     }).AsEnumerable();

                    mCompany = JsonSerializer.Deserialize<Domain.Company>(JsonSerializer.Serialize(efCompany.FirstOrDefault()));
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return mCompany;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efCompany = context.Companies.Where(x => x.Id == id).FirstOrDefault();
                    if (efCompany != null && efCompany.Id > default(int))
                    {
                        efCompany.DeletedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
