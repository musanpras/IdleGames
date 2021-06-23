using System;


	public static class TimeExtensionsUtil
	{
		private static string[] _timeUnitShortNames = new string[4]
		{
			"CM_SECOND_SHORT",
			"CM_MINUTE_SHORT",
			"CM_HOUR_SHORT",
			"CM_DAY_SHORT"
		};

		private static string[] _timeUnitLongNameKeys = new string[4]
		{
			"CM_SECOND",
			"CM_MINUTE",
			"CM_HOUR",
			"CM_DAY"
		};

		private static int[] _timeUnitDurations = new int[4]
		{
			1,
			60,
			3600,
			86400
		};

		public static double GetTotalTimeLapsed(TimeSpan timeSpan, TIME_UNIT timeUnit)
		{
			switch (timeUnit)
			{
			case TIME_UNIT.DAY:
				return timeSpan.TotalDays;
			case TIME_UNIT.HOUR:
				return timeSpan.TotalHours;
			case TIME_UNIT.MINUTE:
				return timeSpan.TotalMinutes;
			case TIME_UNIT.SECOND:
				return timeSpan.TotalSeconds;
			default:
				return 0.0;
			}
		}

		public static int GetTimeLapsed(TimeSpan timeSpan, TIME_UNIT timeUnit)
		{
			switch (timeUnit)
			{
			case TIME_UNIT.DAY:
				return timeSpan.Days;
			case TIME_UNIT.HOUR:
				return timeSpan.Hours;
			case TIME_UNIT.MINUTE:
				return timeSpan.Minutes;
			case TIME_UNIT.SECOND:
				return timeSpan.Seconds;
			default:
				return 0;
			}
		}

		public static int GetTimeUnitDuration(TIME_UNIT timeUnit)
		{
			return _timeUnitDurations[(int)timeUnit];
		}

		public static string GetTimeUnitShortName(int timeLapsed, TIME_UNIT timeUnit)
		{
			return Language.Get(_timeUnitShortNames[(int)timeUnit], "COMMON");
		}

		public static string GetTimeUnitLongName(int timeLapsed, TIME_UNIT timeUnit)
		{
			string text = _timeUnitLongNameKeys[(int)timeUnit];
			if (timeLapsed != 1)
			{
				text += "S";
			}
			return " " + Language.Get(text, "COMMON");
		}
	}

