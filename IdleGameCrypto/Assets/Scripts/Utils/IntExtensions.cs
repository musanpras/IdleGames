using System;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;


	public static class IntExtensions
	{
		private delegate bool OnProcessTimeUnit(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit);

		private static NumberFormatInfo _nfi;

		static IntExtensions()
		{
			_nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			_nfi.NumberGroupSeparator = " ";
		}

		public static CultureInfo GetCurrentCultureInfo()
		{
			SystemLanguage currentLanguage = Application.systemLanguage;
			CultureInfo cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault((CultureInfo x) => x.EnglishName.Equals(currentLanguage.ToString()));
			if (cultureInfo != null)
			{
				return CultureInfo.CreateSpecificCulture(cultureInfo.TwoLetterISOLanguageName);
			}
			return CultureInfo.CurrentCulture;
		}

		public static DateTime ToDate(this int timestamp)
		{
			return DateTimeExtensions.startDate.AddSeconds(timestamp);
		}

		public static string ToDotSeparated(this int value, string format = "N0")
		{
			return value.ToString(format, CultureInfo.CurrentCulture);
		}

		public static string ToSpaceSeparated(this int value, string format = "#,0")
		{
			return value.ToString(format, _nfi);
		}

		public static string ToCronometer(this int value, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.MINUTE)
		{
			return ProcessTime(value, -1, lowestTimeUnit, highestTimeUnit, delegate(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(":");
					stringBuilder.Append(time.ToString("00"));
				}
				else
				{
					stringBuilder.Append(time.ToString("#0"));
				}
				return true;
			});
		}

		public static string ToShortReadableTime(this int value, int maxTimeUnits = -1, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.DAY)
		{
			return ProcessTime(value, maxTimeUnits, lowestTimeUnit, highestTimeUnit, delegate(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit)
			{
				if (time <= 0 && (timeUnit != lowestTimeUnit || stringBuilder.Length > 0))
				{
					return false;
				}
				string timeUnitShortName = TimeExtensionsUtil.GetTimeUnitShortName(time, timeUnit);
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(time + timeUnitShortName);
				return true;
			});
		}

		public static string ToLongReadableTime(this int value, int maxTimeUnits = -1, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.DAY)
		{
			return ProcessTime(value, maxTimeUnits, lowestTimeUnit, highestTimeUnit, delegate(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit)
			{
				if (time <= 0 && (timeUnit != lowestTimeUnit || stringBuilder.Length > 0))
				{
					return false;
				}
				string timeUnitLongName = TimeExtensionsUtil.GetTimeUnitLongName(time, timeUnit);
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(time + timeUnitLongName);
				return true;
			});
		}

		private static string ProcessTime(int value, int maxTimeUnits, TIME_UNIT lowestTimeUnit, TIME_UNIT highestTimeUnit, OnProcessTimeUnit processTimeUnitCallback)
		{
			StringBuilder stringBuilder = new StringBuilder((highestTimeUnit - lowestTimeUnit + 1) * 3);
			int num = 0;
			int num2 = value;
			for (int num3 = (int)highestTimeUnit; num3 >= (int)lowestTimeUnit; num3--)
			{
				TIME_UNIT timeUnit = (TIME_UNIT)num3;
				int timeUnitDuration = TimeExtensionsUtil.GetTimeUnitDuration(timeUnit);
				int num4 = num2 / timeUnitDuration;
				num2 -= num4 * timeUnitDuration;
				if (processTimeUnitCallback(stringBuilder, num4, timeUnit))
				{
					num++;
					if (maxTimeUnits >= 0 && num >= maxTimeUnits)
					{
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static float Normalize(this int value, int min, int max)
		{
			return Mathf.Clamp01((float)(value - min) / (float)(max - min));
		}
	}

