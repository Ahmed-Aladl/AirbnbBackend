namespace Application.Shared
{
    public static class DateTimeExtensions
    {
        public static DateTime Max(this DateTime date1, DateTime date2) =>
            date1 > date2 ? date1 : date2;

        public static DateTime Min(this DateTime date1, DateTime date2) =>
            date1 < date2 ? date1 : date2;
    }
}
