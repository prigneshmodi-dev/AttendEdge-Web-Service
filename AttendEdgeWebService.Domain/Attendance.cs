using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeWebService.Domain
{
    public class Attendance
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? BreakInTime { get; set; }
        public DateTime? BreakOutTime { get; set; }
        public decimal LatitudeCheckIn { get; set; }
        public decimal LongitudeCheckIn { get; set; }
        public decimal? LatitudeCheckOut { get; set; }
        public decimal? LongitudeCheckOut { get; set; }
        public decimal? LatitudeBreakIn { get; set; }
        public decimal? LongitudeBreakIn { get; set; }
        public decimal? LatitudeBreakOut { get; set; }
        public decimal? LongitudeBreakOut { get; set; }
        public string CheckInImagePath { get; set; }
        public string CheckOutImagePath { get; set; }
        public string BreakInImagePath { get; set; }
        public string BreakOutImagePath { get; set; }
        public string LocationDescription { get; set; }
        public string DeviceInfo { get; set; }
        public string IPAddress { get; set; }
        public int CreatedBy { get; set; }        
        public int? LastModifiedBy { get; set; }
        public string CheckInImageBase64 { get; set; }
        public string CheckOutImageBase64 { get; set; }
        public string BreakInImageBase64 { get; set; }
        public string BreakOutImageBase64 { get; set; }

    }
}
