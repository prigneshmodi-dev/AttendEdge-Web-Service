using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Domain
{
    public class AMCLister
    {
        public List<AMC> List { get; set; } = new List<AMC>();

        public Domain.AMC SearchCriteria { get; set; } = new Domain.AMC();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
