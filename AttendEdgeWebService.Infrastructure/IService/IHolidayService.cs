namespace AttendEdgeWebService.Infrastructure.IService
{
    public interface IHolidayService
    {
        Domain.HolidayLister GetAll(Domain.HolidayLister mLister);

        Domain.Holiday Upsert(Domain.Holiday mHoliday);

        Domain.Holiday Get(int id);

        void Delete(int id);
    }
}
