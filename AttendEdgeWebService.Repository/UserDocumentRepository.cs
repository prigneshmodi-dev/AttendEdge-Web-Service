using AttendEdgeWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AttendEdgeWebService.Repository
{
    public class UserDocumentRepository : IUserDocumentRepository
    {
        public Domain.UserDocument Create(Domain.UserDocument mDocument)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efDocument = new Persistence.UserDocument
                    {
                        UserId = mDocument.UserId,
                        FilePath = mDocument.FilePath,
                        CreatedBy = mDocument.CreatedBy,
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = mDocument.LastModifiedBy,
                        LastModifiedDate = DateTime.Now,
                    };
                    context.UserDocuments.Add(efDocument);
                    context.SaveChanges();
                    mDocument.Id = efDocument.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mDocument;
        }

        public List<Domain.UserDocument> Get(int userId)
        {
            var mImages = new List<Domain.UserDocument>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efDocuments = (from pi in context.UserDocuments
                                       where pi.UserId == userId && pi.DeletedBy == null && pi.DeletedDate == null
                                       select new
                                       {
                                           Id = pi.Id,
                                           UserId = pi.UserId,
                                           FilePath = pi.FilePath,
                                       }).AsEnumerable();

                    mImages = JsonSerializer.Deserialize<List<Domain.UserDocument>>(JsonSerializer.Serialize(efDocuments.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mImages;
        }


        public List<Domain.UserDocument> FetchUserDocument(int userId)
        {
            var mImages = new List<Domain.UserDocument>();
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efdocuments = (from pi in context.UserDocuments
                                    where pi.UserId == userId && pi.DeletedBy == null && pi.DeletedDate == null
                                    select new
                                    {
                                        Id = pi.Id,
                                        UserId = pi.UserId,
                                        FilePath = pi.FilePath,
                                    }).AsEnumerable();


                    mImages = JsonSerializer.Deserialize<List<Domain.UserDocument>>(JsonSerializer.Serialize(efdocuments.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mImages;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.AttendEdgeDBEntities())
                {
                    var efImage = context.UserDocuments.FirstOrDefault(x => x.Id == id);
                    if (efImage != null && efImage.Id > 0)
                    {
                        context.UserDocuments.Remove(efImage);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
