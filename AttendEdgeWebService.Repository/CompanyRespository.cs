using AttendEdgeWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class CompanyRespository : ICompanyRepository
    {
        public Domain.CompanyLister GetAll(Domain.CompanyLister mLister)
        {
            try
            {
                using (var context = new AttendEdgeWebService.Persistence.AttendEdgeDBEntities())
                {
                    var efCompanies = (from comp in context.Companies
                                       where comp.DeletedDate == null && comp.DeletedBy == null
                                       select new Domain.Company()
                                       {
                                           Id = comp.Id,
                                           Name = comp.Name,
                                           Line1 = comp.Line1,
                                           Line2 = comp.Line2,
                                           City = comp.City,
                                           State = comp.State,
                                           ZipCode = comp.ZipCode,
                                           ContactPerson = comp.ContactPerson,
                                           MobileNumber = comp.MobileNumber,
                                           IsActive = comp.IsActive,
                                           CreatedBy = comp.CreatedBy,
                                           CreatedDate = comp.CreatedDate
                                       }).AsEnumerable();

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (!string.IsNullOrWhiteSpace(criteria.Name))
                        {
                            efCompanies = efCompanies.Where(x => x.Name != null &&
                                x.Name.ToUpper().Contains(criteria.Name.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.Line1))
                        {
                            efCompanies = efCompanies.Where(x => x.Line1 != null &&
                                x.Line1.ToUpper().Contains(criteria.Line1.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.Line2))
                        {
                            efCompanies = efCompanies.Where(x => x.Line2 != null &&
                                x.Line2.ToUpper().Contains(criteria.Line2.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.City))
                        {
                            efCompanies = efCompanies.Where(x => x.City != null &&
                                x.City.ToUpper().Contains(criteria.City.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.State))
                        {
                            efCompanies = efCompanies.Where(x => x.State != null &&
                                x.State.ToUpper().Contains(criteria.State.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.ContactPerson))
                        {
                            efCompanies = efCompanies.Where(x => x.ContactPerson != null &&
                                x.ContactPerson.ToUpper().Contains(criteria.ContactPerson.ToUpper()));
                        }

                        if (!string.IsNullOrWhiteSpace(criteria.MobileNumber))
                        {
                            efCompanies = efCompanies.Where(x => x.MobileNumber != null &&
                                x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));
                        }

                        //if (!string.IsNullOrWhiteSpace(criteria.Status))
                        //{
                        //    if (criteria.Status == "Active")
                        //    {
                        //        efCompanies = efCompanies.Where(x => x.IsActive);
                        //    }
                        //    else if (criteria.Status == "Disabled")
                        //    {
                        //        efCompanies = efCompanies.Where(x => !x.IsActive);
                        //    }
                        //}
                    }

                    // Pagination logic
                    if (mLister.Pagination != null)
                    {
                        if (mLister.Pagination.TotalRecord == 0)
                        {
                            mLister.Pagination.TotalRecord = efCompanies.Count();
                        }

                        if (mLister.Pagination.Take == -1)
                        {
                            mLister.Pagination.Take = mLister.Pagination.TotalRecord;
                            mLister.Pagination.Skip = 0;
                        }
                        else if (mLister.Pagination.Take > 0)
                        {
                            mLister.Pagination.CurrentPage = (mLister.Pagination.Skip / mLister.Pagination.Take) + 1;
                        }
                        else
                        {
                            mLister.Pagination.CurrentPage = 1;
                        }

                        if (mLister.Pagination.TotalRecord > 0 && mLister.Pagination.Take > 0)
                        {
                            mLister.Pagination.TotalPage = (int)Math.Ceiling((double)mLister.Pagination.TotalRecord / mLister.Pagination.Take);
                        }

                        if (mLister.Pagination.TotalPage == 0)
                        {
                            mLister.Pagination.TotalPage = 1;
                        }
                    }

                    if (mLister.Pagination.TotalRecord > 0)
                    {
                        efCompanies = efCompanies
                            .Skip(mLister.Pagination.Skip)
                            .Take(mLister.Pagination.Take);

                        mLister.List = JsonSerializer.Deserialize<List<Domain.Company>>(
                            JsonSerializer.Serialize(efCompanies.ToList()));
                    }
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
                using (var context = new AttendEdgeWebService.Persistence.AttendEdgeDBEntities())
                {
                    var efCompany = context.Companies.Where(x => x.Id == mCompany.Id).FirstOrDefault();
                    if (efCompany == null || efCompany.Id <= default(int))
                    {
                        efCompany = new Persistence.Company();
                        efCompany.CreatedDate = DateTime.Now;
                        context.Companies.Add(efCompany);
                    }

                    efCompany.Name = mCompany.Name;
                    efCompany.Line1 = mCompany.Line1;
                    efCompany.Line2 = mCompany.Line2;
                    efCompany.City = mCompany.City;
                    efCompany.State = mCompany.State;
                    efCompany.ZipCode = mCompany.ZipCode;
                    efCompany.ContactPerson = mCompany.ContactPerson;
                    efCompany.MobileNumber = mCompany.MobileNumber;
                    efCompany.IsActive = mCompany.IsActive;

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
                using (var context = new AttendEdgeWebService.Persistence.AttendEdgeDBEntities())
                {
                    var efCompany = (from comp in context.Companies
                                     where comp.DeletedDate == null && comp.DeletedBy == null
                                     select new
                                     {
                                         Id = comp.Id,
                                         Name = comp.Name,
                                         Line1 = comp.Line1,
                                         Line2 = comp.Line2,
                                         City = comp.City,
                                         State = comp.State,
                                         ZipCode = comp.ZipCode,
                                         ContactPerson = comp.ContactPerson,
                                         MobileNumber = comp.MobileNumber,
                                         IsActive = comp.IsActive,
                                         CreatedBy = comp.CreatedBy,
                                         CreatedDate = comp.CreatedDate
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
                using (var context = new AttendEdgeWebService.Persistence.AttendEdgeDBEntities())
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
