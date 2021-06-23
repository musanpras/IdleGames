using System.Globalization;
using System.Text;
using UnityEngine;


	public static class FloatExtensions
	{
		private delegate void OnProcessTimeUnit(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit);

		private static NumberFormatInfo _nfi;

		static FloatExtensions()
		{
			_nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			_nfi.NumberGroupSeparator = " ";
		}

		public static string ToDotSeparated(this float value, string format = "N2")
		{
			return value.ToString(format, CultureInfo.CurrentCulture);
		}

		public static string ToSpaceSeparated(this float value, string format = "#,0.00")
		{
			return value.ToString(format, _nfi);
		}

		public static string ToCronometer(this float value, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.MINUTE)
		{
			return Mathf.FloorToInt(value).ToCronometer(lowestTimeUnit, highestTimeUnit);
		}

		public static string ToShortReadableTime(this float value, int maxTimeUnits = -1, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.DAY)
		{
			return Mathf.FloorToInt(value).ToShortReadableTime(maxTimeUnits, lowestTimeUnit, highestTimeUnit);
		}

		public static string ToLongReadableTime(this float value, int maxTimeUnits = -1, TIME_UNIT lowestTimeUnit = TIME_UNIT.SECOND, TIME_UNIT highestTimeUnit = TIME_UNIT.DAY)
		{
			return Mathf.FloorToInt(value).ToLongReadableTime(maxTimeUnits, lowestTimeUnit, highestTimeUnit);
		}
	}

