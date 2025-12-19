using AttendEdgeWebService.Infrastructure.CustomException;
using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;


namespace AttendEdgeWebService.Service
{
    public class UserService : IUserService
    {
        #region Declaration
        private readonly IUserRepository _repo;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository repo, IFileReaderService fileReaderService, IEmailSenderService emailSenderService, ICompanyRepository companyRepository)
        {
            _repo = repo;
            _fileReaderService = fileReaderService;
            _emailSenderService = emailSenderService;
            _companyRepository = companyRepository;
        }
        #endregion

        #region Public Methods
        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.User Upsert(Domain.User mUser)
        {
            if (mUser == null)
                throw new APIRequestFailedException("User object must not be null.");

            SetUserPassword(mUser);

            var mCreatedUser = _repo.Upsert(mUser);
            if (IsValidUser(mCreatedUser))
            {
                var mCompany = _companyRepository.Get(mCreatedUser.CompanyId);
                SendEmailOnCreateUser(mCreatedUser, mCompany);
            }

            return mCreatedUser;
        }

        public Domain.User Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<Domain.User> GetActiveUsers(int companyId)
        {
            return _repo.GetActiveUsers(companyId);
        }

        public string SaveDocuments(Domain.UserDocument mImage)
        {
            // Define base path (e.g., ~/images/products)
            string relativeFolder = "~/Upload/Images/document/";
            string serverFolder = HttpContext.Current.Server.MapPath(relativeFolder);

            // Strip metadata if present (e.g., "data:image/jpeg;base64,...")
            var base64Data = Regex.Replace(mImage.Base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);

            // Convert from Base64 String into Bytes
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Ensure directory exists
            if (!Directory.Exists(serverFolder))
                Directory.CreateDirectory(serverFolder);

            // Generate unique filename
            string fileExtension = Path.GetExtension(mImage.FilePath);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string fullPath = Path.Combine(serverFolder, fileName);

            // Save file to disk
            File.WriteAllBytes(fullPath, imageBytes);

            // Return relative path for DB storage
            string relativePath = Path.Combine("images", "document", fileName).Replace("\\", "/");
            return relativePath;
        }
        #endregion

        #region Private Methods
        private void SendEmailOnCreateUser(Domain.User mUser, Domain.Company mCompany)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnCreateUser.html");
            string finalContent = PopulateEmailTemplate(content, mUser, mCompany);
            _emailSenderService.SendEmail(finalContent, mUser.EmailAddress, $"Welcome to {mCompany.Name} – Your Account Details Inside");
        }

        private string PopulateEmailTemplate(string template, Domain.User mUser, Domain.Company mCompany)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{CompanyName}}", mCompany.Name },
                { "{{FirstName}}", mUser.FirstName },
                { "{{Username}}", mUser.EmailAddress },
                { "{{Password}}", mUser.PlainTextPassword},
                { "{{Year}}", DateTime.Now.Year.ToString() },
                { "{{CompanyLogo}}", mCompany.Logo }
            };

            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }

            return template;
        }

        private void SetUserPassword(Domain.User user)
        {
            user.PlainTextPassword = StaticMethods.GeneratePassword(8);
            var salt = ConfigurationManager.AppSettings.Get("Salt");
            user.Password = PasswordEncryptor.Encrypt(user.PlainTextPassword, salt);
        }

        private bool IsValidUser(Domain.User user)
        {
            return user != null && user.Id > 0;
        }
        #endregion
    }
}