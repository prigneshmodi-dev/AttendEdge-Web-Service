using System.Collections.Generic;

namespace AttendEdgeWebService.Domain
{
    public class ContactLister
    {
        public List<Contact> List { get; set; } = new List<Contact>();

        public Domain.Contact SearchCriteria { get; set; } = new Domain.Contact();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
