using System.Collections.Generic;

namespace AttendEdgeWebService.Domain
{
    public class CompanyLister
    {
        public List<Company> List { get; set; } = new List<Company>();

        public Domain.Company SearchCriteria { get; set; } = new Domain.Company();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
