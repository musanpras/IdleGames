using System;


	public static class DateTimeExtensions
	{
		public static DateTime startDate = new DateTime(1970, 1, 1);

		public static int ToTimestamp(this DateTime date)
		{
			return (int)(date - startDate).TotalSeconds;
		}

		public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startofWeek)
		{
			int num = dateTime.DayOfWeek - startofWeek;
			if (num < 0)
			{
				num += 7;
			}
			return dateTime.AddDays(-1 * num).Date;
		}

		public static DateTime ToDateTime(this int value)
		{
			return startDate.AddSeconds(value);
		}
	}

