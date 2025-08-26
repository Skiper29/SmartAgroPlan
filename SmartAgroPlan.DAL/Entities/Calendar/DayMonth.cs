namespace SmartAgroPlan.DAL.Entities.Calendar;

public struct DayMonth
{
    public int Day { get; set; }
    public int Month { get; set; }

    public DayMonth(int day, int month)
    {
        Day = day;
        Month = month;
    }

    public DayMonth() { }
}
