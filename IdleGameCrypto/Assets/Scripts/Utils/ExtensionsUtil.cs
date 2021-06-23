
using System.Text;


	public static class ExtensionsUtil
	{
		public enum UNIT
		{
			NONE = -1,
			MILLION,
			BILLION,
			TRILLION,
			QUADRILLION,
			QUINTILLION,
			SEXTILLION,
			SEPTILLION,
			OCTILLION,
			NONILLION,
			DECILLION,
			UNDECILLION,
			DUODECILLION,
			TREDECILLION,
			QUATTORDECILLION,
			QUINDECILLION,
			SEXDECILLION,
			SEPTENDECILLION,
			OCTODECILLION,
			NOVEMDECILLION,
			VIGINTILLION,
			UNVIGINTILLION,
			DUOVIGINTILLION,
			TRESVIGINTILLION,
			QUATTUORVIGINTILLION,
			QUINQUAVIGINTILLION,
			SESVIGINTILLION,
			SEPTEMVIGINTILLION,
			OCTOVIGINTILLION,
			NOVEMVIGINTILLION,
			TRIGINTILLION,
			UNTRIGINTILLION,
			DUOTRIGINTILLION,
			TRESTRIGINTILLION,
			QUATTUORTRIGINTILLION,
			QUINQUATRIGINTILLION,
			SESTRIGINTILLION,
			SEPTENTRIGINTILLION,
			OCTOTRIGINTILLION,
			NOVENTRIGINTILLION,
			A_LOT,
			MAX_UNIT
		}

		private delegate void OnProcessUnit(StringBuilder stringBuilder, double unitValue, UNIT unit);

		private static string[] _unitShortNames = new string[40]
		{
			"M",
			"B",
			"T",
			"q",
			"Q",
			"s",
			"S",
			"o",
			"N",
			"d",
			"U",
			"D",
			"Td",
			"qd",
			"Qd",
			"sd",
			"Sd",
			"Od",
			"Nd",
			"V",
			"Uv",
			"Dv",
			"Tv",
			"qv",
			"Qv",
			"sv",
			"Sv",
			"Ov",
			"Nv",
			"Tg",
			"Ut",
			"Dt",
			"Tt",
			"qt",
			"Qt",
			"st",
			"St",
			"Ot",
			"Nt",
			"[X]"
		};

		private static string[] _unitLongNameKeys = new string[40]
		{
			"CM_MILLION",
			"CM_BILLION",
			"CM_TRILLION",
			"CM_QUADRILLION",
			"CM_QUINTILLION",
			"CM_SEXTILLION",
			"CM_SEPTILLION",
			"CM_OCTILLION",
			"CM_NONILLION",
			"CM_DECILLION",
			"CM_UNDECILLION",
			"CM_DUODECILLION",
			"CM_TREDECILLION",
			"CM_QUATTORDECILLION",
			"CM_QUINDECILLION",
			"CM_SEXDECILLION",
			"CM_SEPTENDECILLION",
			"CM_OCTODECILLION",
			"CM_NOVEMDECILLION",
			"CM_VIGINTILLION",
			"CM_UNVIGINTILLION",
			"CM_DUOVIGINTILLION",
			"CM_TRESVIGINTILLION",
			"CM_QUATTUORVIGINTILLION",
			"CM_QUINQUAVIGINTILLION",
			"CM_SESVIGINTILLION",
			"CM_SEPTEMVIGINTILLION",
			"CM_OCTOVIGINTILLION",
			"CM_NOVEMVIGINTILLION",
			"CM_TRIGINTILLION",
			"CM_UNTRIGINTILLION",
			"CM_DUOTRIGINTILLION",
			"CM_TRESTRIGINTILLION",
			"CM_QUATTUORTRIGINTILLION",
			"CM_QUINQUATRIGINTILLION",
			"CM_SESTRIGINTILLION",
			"CM_SEPTENTRIGINTILLION",
			"CM_OCTOTRIGINTILLION",
			"CM_NOVENTRIGINTILLION",
			"CM_A_LOT"
		};

		public static void TestUnits()
		{
			double num = 1.21;
			for (int i = 0; i < 60; i++)
			{
				UnityEngine.Debug.LogError("unitValue: " + num + ", short: " + num.ToShortUnit(out string unitStr) + unitStr + ", nodecimals: " + num.ToShortNoDecimalsUnit(out string unitStr2) + unitStr2 + ", perSec: " + num.ToPerSecondUnit(out string unitStr3) + unitStr3 + ", long: " + num.ToLongUnit(out string _) + ", noDecimalsNoUnit: " + num.ToNoDecimalsNoUnit());
				num *= 10.0;
				num += 0.011;
			}
		}

		public static string GetUnitShortName(double unitValue, UNIT unit)
		{
			return _unitShortNames[(int)unit];
		}

		public static string GetUnitLongName(double unitValue, UNIT unit)
		{
			string text = _unitLongNameKeys[(int)unit];
			if (unitValue != 1.0)
			{
				text += "S";
			}
			return Language.Get(text, "COMMON");
		}

		public static string ToNoDecimalsNoUnit(this double value)
		{
			return ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
				}
			});
		}

		public static string ToShortNoDecimalsUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
					tempUnitStr = GetUnitShortName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToShortUnitWithUnits(this double value)
		{
			string unitStr;
			return value.ToShortUnit(out unitStr) + unitStr;
		}

		public static string ToShortUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated());
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N3"));
					tempUnitStr = GetUnitShortName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToShortNoDecimalsInUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated());
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
					tempUnitStr = GetUnitShortName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToShortNeverDecimalsInUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
					tempUnitStr = GetUnitShortName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToLongUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated());
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N3"));
					tempUnitStr = GetUnitLongName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToLongNoDecimalsUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
					tempUnitStr = GetUnitLongName(unitValue, unit);
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToPerSecondUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated());
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					tempUnitStr = "/sec";
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N3"));
					tempUnitStr = GetUnitShortName(unitValue, unit) + "/sec";
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		public static string ToNoDecimalsPerMinuteUnit(this double value, out string unitStr)
		{
			string tempUnitStr = "";
			string result = ProcessUnit(value, delegate(StringBuilder stringBuilder, double unitValue, UNIT unit)
			{
				if (unit == UNIT.NONE)
				{
					if (value < 100.0)
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					else
					{
						stringBuilder.Append(value.ToDotSeparated("N0"));
					}
					tempUnitStr = "/min";
				}
				else
				{
					stringBuilder.Append(((float)unitValue).ToDotSeparated("N0"));
					tempUnitStr = GetUnitShortName(unitValue, unit) + "/min";
				}
			});
			unitStr = tempUnitStr;
			return result;
		}

		private static string ProcessUnit(double value, OnProcessUnit processUnitCallback)
		{
			StringBuilder stringBuilder = new StringBuilder(8);
			if (value < 100000000.0)
			{
				processUnitCallback(stringBuilder, value, UNIT.NONE);
				return stringBuilder.ToString();
			}
			double num = value / 1000000.0;
			int num2 = 0;
			int num3 = 39;
			while (num >= 999.99999999 && num2 < num3)
			{
				num /= 1000.0;
				num2++;
			}
			UNIT uNIT = (UNIT)num2;
			if (uNIT == UNIT.A_LOT)
			{
				return Language.Get(_unitLongNameKeys[num2], "COMMON");
			}
			processUnitCallback(stringBuilder, num, uNIT);
			return stringBuilder.ToString();
		}
	}

