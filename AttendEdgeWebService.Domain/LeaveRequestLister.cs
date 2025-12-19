using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Domain
{
    public class LeaveRequestLister
    {
        public List<LeaveRequest> List { get; set; } = new List<LeaveRequest>();

        public LeaveRequest SearchCriteria { get; set; } = new LeaveRequest();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
