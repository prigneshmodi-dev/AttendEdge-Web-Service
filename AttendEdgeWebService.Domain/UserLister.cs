using System.Collections.Generic;

namespace AttendEdgeWebService.Domain
{
    public class UserLister
    {
        public List<User> List { get; set; } = new List<User>();

        public User SearchCriteria { get; set; } = new User();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
