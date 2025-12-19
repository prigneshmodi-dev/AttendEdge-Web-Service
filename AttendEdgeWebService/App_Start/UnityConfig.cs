using AttendEdgeWebService.Infrastructure.IRepository;
using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Repository;
using AttendEdgeWebService.Service;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace AttendEdgeWebService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            //Service
            container.RegisterType<IAMCService, AMCService>();
            container.RegisterType<IAttendanceService, AttendanceService>();
            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IChangePasswordService, ChangePasswordService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IContactService, ContactService>();
            container.RegisterType<IEmailSenderService, EmailSenderService>();
            container.RegisterType<IFileReaderService, FileReaderService>();
            container.RegisterType<IForgotPasswordService, ForgotPasswordService>();
            container.RegisterType<IHolidayService, HolidayService>();
            container.RegisterType<IJWTokenService, JWTokenService>();
            container.RegisterType<ILeaveRequestService, LeaveRequestService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IUserDocumentService, UserDocumentService>();
            container.RegisterType<IUserService, UserService>();
            
            //Repository
            container.RegisterType<IAMCRepository, AMCRepository>();
            container.RegisterType<IAttendanceRepository, AttendanceRepository>();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IContactRepository, ContactRepository>();
            container.RegisterType<IHolidayRepository, HolidayRepository>();
            container.RegisterType<ILeaveRequestRepository, LeaveRequestRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();
            container.RegisterType<IUserDocumentRepository, UserDocumentRepository>();
            container.RegisterType<IUserRepository, UserRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}