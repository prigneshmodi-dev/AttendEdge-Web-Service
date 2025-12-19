using System.Collections.Generic;
using System.Web;

namespace AttendEdgeWebService.Domain
{
    public class User
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PlainTextPassword { get; set; }
        public string MobileNumber { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public List<UserDocument> Images = new List<UserDocument>();
    }
}
