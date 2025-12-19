using AttendEdgeWebService.Infrastructure.CustomException;
using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Configuration;

namespace AttendEdgeWebService.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepo;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEmailSenderService _emailSenderService;

        public ForgotPasswordService(IUserRepository userRepo, IFileReaderService fileReaderService, IEmailSenderService emailSenderService)
        {
            _userRepo = userRepo;
            _fileReaderService = fileReaderService;
            _emailSenderService = emailSenderService;
        }

        public void Initiate(Domain.ForgotPassword mForgotPassword)
        {
            if (mForgotPassword.EmailAddress.IsNullOrEmpty())
                throw new APIRequestFailedException("Email address is required!");

            var mUser = _userRepo.GetBy(mForgotPassword.EmailAddress);
            if (mUser == null || mUser.Id == default(int))
                throw new APIRequestFailedException("No user found with the provided email address.");

            mUser.PlainTextPassword = StaticMethods.GeneratePassword(8);
            mUser.Password = PasswordEncryptor.Encrypt(mUser.PlainTextPassword, ConfigurationManager.AppSettings.Get("Salt"));
            _userRepo.SetPasswordBy(mUser.Id, mUser.Password);
            SendEmailOnForgotPassword(mUser);
        }

        #region Private Methods

        private string PopulateEmailTemplateForResetPassword(string template, Domain.User user)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{FirstName}}", user.FirstName },
                { "{{EmailAddress}}", user.EmailAddress },
                { "{{Password}}", user.PlainTextPassword},
            };
            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }
            return template;
        }

        private void SendEmailOnForgotPassword(Domain.User mUser)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnForgotPassword.html");
            string finalContent = PopulateEmailTemplateForResetPassword(content, mUser);
            _emailSenderService.SendEmail(finalContent, mUser.EmailAddress , $"Your Access Details for AttendEdge");
        }
        #endregion
    }
}
