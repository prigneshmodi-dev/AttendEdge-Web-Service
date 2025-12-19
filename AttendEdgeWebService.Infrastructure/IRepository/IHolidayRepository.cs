namespace AttendEdgeWebService.Infrastructure.IRepository
{
    public interface IHolidayRepository
    {
        Domain.HolidayLister GetAll(Domain.HolidayLister mLister);

        Domain.Holiday Upsert(Domain.Holiday mHoliday);

        Domain.Holiday Get(int id);

        void Delete(int id);
    }
}
