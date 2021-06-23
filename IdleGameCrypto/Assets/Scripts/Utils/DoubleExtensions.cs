using System.Globalization;
using System.Text;


	public static class DoubleExtensions
	{
		private delegate void OnProcessTimeUnit(StringBuilder stringBuilder, int time, TIME_UNIT timeUnit);

		private static NumberFormatInfo _nfi;

		private static CultureInfo _ci;

		static DoubleExtensions()
		{
			_nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			_nfi.NumberGroupSeparator = " ";
			_ci = IntExtensions.GetCurrentCultureInfo();
		}

		public static string ToDotSeparated(this double value, string format = "N2")
		{
			return value.ToString(format, _ci);
		}

		public static string ToSpaceSeparated(this double value, string format = "#,0.00")
		{
			return value.ToString(format, _nfi);
		}
	}

