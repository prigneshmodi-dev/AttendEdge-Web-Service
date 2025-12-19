using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class AMCRepository : IAMCRepository
    {
        public Domain.AMCLister GetAll(Domain.AMCLister mLister)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var query = from amc in context.AMCs
                                where amc.DeletedDate == null && amc.DeletedBy == null
                                select new
                                {
                                    Id = amc.Id,
                                    CompanyId = amc.CompanyId,
                                    Name = amc.Name,
                                    ContactPerson = amc.ContactPerson,
                                    MobileNumber = amc.MobileNumber,
                                    Email = amc.Email,
                                    ContractStartDate = amc.ContractStartDate,
                                    ContractEndDate = amc.ContractEndDate,
                                    ServicesCovered = amc.ServicesCovered,
                                    ContractDocumentPath = amc.ContractDocumentPath,
                                    Notes = amc.Notes
                                };

                    // Server-side filtering
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.ContactPerson.IsNotNullOrEmpty())
                            query = query.Where(x => x.ContactPerson != null && x.ContactPerson.ToUpper().Contains(criteria.ContactPerson.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.ServicesCovered.IsNotNullOrEmpty())
                            query = query.Where(x => x.ServicesCovered != null && x.ServicesCovered.ToUpper().Contains(criteria.ServicesCovered.ToUpper()));
                    }

                    var efAMCs = query.AsEnumerable();

                    // Pagination
                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = efAMCs.Count();

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0
                        ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take)
                        : 1;

                    efAMCs = efAMCs.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.AMC>>(JsonSerializer.Serialize(efAMCs.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.AMC Upsert(Domain.AMC mAMC)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efAMC = context.AMCs.FirstOrDefault(x => x.Id == mAMC.Id);
                    if (efAMC == null || efAMC.Id <= default(int))
                    {
                        efAMC = new Persistence.AMC();
                        efAMC.CreatedDate = DateTime.Now;
                        efAMC.CreatedBy = mAMC.CreatedBy;
                        context.AMCs.Add(efAMC);
                    }
                    efAMC.CompanyId = mAMC.CompanyId;
                    efAMC.Name = mAMC.Name;
                    efAMC.ContactPerson = mAMC.ContactPerson;
                    efAMC.MobileNumber = mAMC.MobileNumber;
                    efAMC.Email = mAMC.Email;
                    efAMC.ContractStartDate = mAMC.ContractStartDate;
                    efAMC.ContractEndDate = mAMC.ContractEndDate;
                    efAMC.ServicesCovered = mAMC.ServicesCovered;
                    efAMC.ContractDocumentPath = mAMC.ContractDocumentPath;
                    efAMC.Notes = mAMC.Notes;
                    efAMC.LastModifiedDate = DateTime.Now;
                    efAMC.LastModifiedBy = mAMC.LastModifiedBy;
                    context.SaveChanges();
                    mAMC.Id = efAMC.Id;
                }
            }
            catch
            {
                throw;
            }

            return mAMC;
        }

        public Domain.AMC Get(int id)
        {
            var mAMC = new Domain.AMC();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efAMC = (from amc in context.AMCs
                                 where amc.DeletedDate == null && amc.DeletedBy == null && amc.Id == id
                                 select new
                                 {
                                     Id = amc.Id,
                                     CompanyId = amc.CompanyId,
                                     Name = amc.Name,
                                     ContactPerson = amc.ContactPerson,
                                     MobileNumber = amc.MobileNumber,
                                     Email = amc.Email,
                                     ContractStartDate = amc.ContractStartDate,
                                     ContractEndDate = amc.ContractEndDate,
                                     ServicesCovered = amc.ServicesCovered,
                                     ContractDocumentPath = amc.ContractDocumentPath,
                                     Notes = amc.Notes
                                 }).AsEnumerable();

                    mAMC = JsonSerializer.Deserialize<Domain.AMC>(JsonSerializer.Serialize(efAMC.FirstOrDefault()));
                }
            }
            catch
            {
                throw;
            }

            return mAMC;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efAMC = context.AMCs.Where(x => x.Id == id && x.DeletedBy == null).FirstOrDefault();
                    if (efAMC != null && efAMC.Id > default(int))
                    {
                        efAMC.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
