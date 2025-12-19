using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace AttendEdgeWebService.Service
{
    public class CompanyService : ICompanyService
    {
        #region Declaration
        private readonly ICompanyRepository _repo;

        private readonly IUserService _userService;

        public CompanyService(ICompanyRepository repo, IUserService userService)
        {
            _repo = repo;
            _userService = userService;
        }

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }
        #endregion

        #region Public Methods
        public Domain.CompanyLister GetAll(Domain.CompanyLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public List<Domain.Company> GetActiveCompanies()
        {
            return _repo.GetActiveCompanies();
        }

        public Domain.Company Upsert(Domain.Company mCompany)
        {
            bool isCreate = mCompany.Id == 0 ? true : false;

            var mResult = _repo.Upsert(mCompany);

            if (isCreate)
            {
                var mUser = PrepareUserModel(mResult.Id, mCompany);
                _userService.Upsert(mUser);
            }

            return mResult;
        }

        public Domain.Company Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }
        #endregion

        #region Private Methods
        private Domain.User PrepareUserModel(int companyId, Domain.Company mCompany)
        {
            var splitStrings = StaticMethods.SplitString(mCompany.ContactPerson, ' ');

            var mUser = new Domain.User
            {
                FirstName = splitStrings[0],
                LastName = splitStrings[1],
                EmailAddress = mCompany.EmailAddress,
                MobileNumber = mCompany.MobileNumber,
                RoleId = 2,
                CompanyId = companyId,
                IsActive = true,
                CreatedBy = mCompany.CreatedBy,
                LastModifiedBy = mCompany.LastModifiedBy,
                Password = PasswordEncryptor.Encrypt(StaticMethods.GeneratePassword(8), ConfigurationManager.AppSettings.Get("Salt")).StrToUpper()
            };

            return mUser;
        }
        #endregion
    }
}
