using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class ContactRepository : IContactRepository
    {
        public Domain.ContactLister GetAll(Domain.ContactLister mLister)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efContacts = (from cu in context.Contacts
                                      join c in context.Companies on cu.CompanyId equals c.Id into joinUC
                                      from c in joinUC.DefaultIfEmpty()
                                      where cu.DeletedDate == null && cu.DeletedBy == null
                                      select new
                                      {
                                          Id = cu.Id,
                                          CompanyId = cu.CompanyId,
                                          FirstName = cu.FirstName,
                                          LastName = cu.LastName,
                                          EmailAddress = cu.EmailAddress,
                                          MobileNumber = cu.MobileNumber,
                                          Message = cu.Message,
                                          IsActive = cu.IsActive
                                      }).AsEnumerable();

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.FirstName.IsNotNullOrEmpty())
                            efContacts = efContacts.Where(x => x.FirstName != null && x.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()));

                        if (criteria.LastName.IsNotNullOrEmpty())
                            efContacts = efContacts.Where(x => x.LastName != null && x.LastName.ToUpper().Contains(criteria.LastName.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            efContacts = efContacts.Where(x => x.EmailAddress != null && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            efContacts = efContacts.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.Message.IsNotNullOrEmpty())
                            efContacts = efContacts.Where(x => x.Message != null && x.Message.ToUpper().Contains(criteria.Message.ToUpper()));

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status.IsNotNullOrEmpty())
                            {
                                if (criteria.Status == "Active")
                                    efContacts = efContacts.Where(x => x.IsActive);
                                else if (criteria.Status == "Disabled")
                                    efContacts = efContacts.Where(x => !x.IsActive);
                            }
                        }
                    }

                    // Pagination logic
                    if (mLister.Pagination != null)
                    {
                        if (mLister.Pagination.TotalRecord == 0)
                            mLister.Pagination.TotalRecord = efContacts.Count();

                        if (mLister.Pagination.Take == -1)
                        {
                            mLister.Pagination.Take = mLister.Pagination.TotalRecord;
                            mLister.Pagination.Skip = 0;
                        }
                        else if (mLister.Pagination.Take > 0)
                            mLister.Pagination.CurrentPage = (mLister.Pagination.Skip / mLister.Pagination.Take) + 1;
                        else
                            mLister.Pagination.CurrentPage = 1;

                        if (mLister.Pagination.TotalRecord > 0 && mLister.Pagination.Take > 0)
                            mLister.Pagination.TotalPage = (int)Math.Ceiling((double)mLister.Pagination.TotalRecord / mLister.Pagination.Take);

                        if (mLister.Pagination.TotalPage == 0)
                            mLister.Pagination.TotalPage = 1;
                    }

                    if (mLister.Pagination.TotalRecord > 0)
                    {
                        efContacts = efContacts.Skip(mLister.Pagination.Skip).Take(mLister.Pagination.Take);

                        mLister.List = JsonSerializer.Deserialize<List<Domain.Contact>>(JsonSerializer.Serialize(efContacts.ToList()));
                    }
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.Contact Upsert(Domain.Contact mContactUs)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efContact = context.Contacts.Where(x => x.Id == mContactUs.Id).FirstOrDefault();
                    if (efContact == null || efContact.Id <= default(int))
                    {
                        efContact = new Persistence.Contact();
                        efContact.CreatedBy = mContactUs.CreatedBy;
                        efContact.CreatedDate = DateTime.Now;
                        context.Contacts.Add(efContact);
                    }

                    efContact.CompanyId = mContactUs.CompanyId;
                    efContact.FirstName = mContactUs.FirstName;
                    efContact.LastName = mContactUs.LastName;
                    efContact.EmailAddress = mContactUs.EmailAddress;
                    efContact.MobileNumber = mContactUs.MobileNumber;
                    efContact.Message = mContactUs.Message;
                    efContact.IsActive = mContactUs.IsActive;
                    efContact.LastModifiedBy = mContactUs.LastModifiedBy;
                    efContact.LastModifiedDate = DateTime.Now;

                    context.SaveChanges();

                    mContactUs.Id = efContact.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mContactUs;
        }

        public Domain.Contact Get(int id)
        {
            var mContactUs = new Domain.Contact();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efContact = (from cu in context.Contacts
                                     where cu.DeletedDate == null && cu.DeletedBy == null && cu.Id == id
                                     select new
                                     {
                                         cu.Id,
                                         cu.CompanyId,
                                         cu.FirstName,
                                         cu.LastName,
                                         cu.EmailAddress,
                                         cu.MobileNumber,
                                         cu.Message,
                                         cu.IsActive
                                     }).AsEnumerable();

                    mContactUs = JsonSerializer.Deserialize<Domain.Contact>(JsonSerializer.Serialize(efContact.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mContactUs;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efContact = context.Contacts.Where(x => x.Id == id).FirstOrDefault();
                    if (efContact != null && efContact.Id > default(int))
                    {
                        efContact.DeletedDate = DateTime.Now;
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
