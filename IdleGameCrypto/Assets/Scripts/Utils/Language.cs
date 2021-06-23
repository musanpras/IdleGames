using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEngine;

public static class Language
{
	public static string settingsAssetPath;

	private static LocalizationSettings _settings;

	private static List<string> availableLanguages;

	private static LanguageCode currentLanguage;

	private static Dictionary<string, Dictionary<string, string>> currentEntrySheets;

	public static LocalizationSettings settings
	{
		get
		{
			if (_settings == null)
			{
				_settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));
			}
			return _settings;
		}
	}

	static Language()
	{
		settingsAssetPath = "Assets/Standard Assets/Localization/Resources/Languages/LocalizationSettings.asset";
		_settings = null;
		currentLanguage = LanguageCode.N;
		LoadAvailableLanguages();
		bool useSystemLanguagePerDefault = settings.useSystemLanguagePerDefault;
		LanguageCode code = LocalizationSettings.GetLanguageEnum(settings.defaultLangCode);
		string @string = PlayerPrefs.GetString("M2H_lastLanguage", "");
		if (@string != "" && availableLanguages.Contains(@string))
		{
			SwitchLanguage(@string);
			return;
		}
		if (useSystemLanguagePerDefault)
		{
			LanguageCode languageCode = LanguageNameToCode(Application.systemLanguage);
			if (languageCode == LanguageCode.N)
			{
				string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName != "iv")
				{
					languageCode = LocalizationSettings.GetLanguageEnum(twoLetterISOLanguageName);
				}
			}
			if (availableLanguages.Contains(string.Concat(languageCode)))
			{
				code = languageCode;
			}
			else
			{
				if (languageCode == LanguageCode.PT && availableLanguages.Contains(string.Concat(LanguageCode.PT_BR)))
				{
					code = LanguageCode.PT_BR;
				}
				switch (languageCode)
				{
				case LanguageCode.ZH:
					if (availableLanguages.Contains(string.Concat(LanguageCode.ZH_CN)))
					{
						code = LanguageCode.ZH_CN;
					}
					else if (availableLanguages.Contains(string.Concat(LanguageCode.ZH_TW)))
					{
						code = LanguageCode.ZH_TW;
					}
					break;
				case LanguageCode.EN:
					if (availableLanguages.Contains(string.Concat(LanguageCode.EN_GB)))
					{
						code = LanguageCode.EN_GB;
					}
					break;
				default:
					if (languageCode == LanguageCode.EN && availableLanguages.Contains(string.Concat(LanguageCode.EN_US)))
					{
						code = LanguageCode.EN_US;
					}
					break;
				}
			}
		}
		SwitchLanguage(code);
	}

	private static void LoadAvailableLanguages()
	{
		availableLanguages = new List<string>();
		if (settings.sheetTitles == null || settings.sheetTitles.Length == 0)
		{
			UnityEngine.Debug.Log("None available");
			return;
		}
		foreach (LanguageCode value in Enum.GetValues(typeof(LanguageCode)))
		{
			if (HasLanguageFile(string.Concat(value), settings.sheetTitles[0]))
			{
				availableLanguages.Add(string.Concat(value));
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static string[] GetLanguages()
	{
		return availableLanguages.ToArray();
	}

	public static bool SwitchLanguage(string langCode)
	{
		return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}

	public static bool SwitchLanguage(LanguageCode code)
	{
		if (availableLanguages.Contains(string.Concat(code)))
		{
			DoSwitch(code);
			return true;
		}
		UnityEngine.Debug.LogError("Could not switch from language " + currentLanguage + " to " + code);
		if (currentLanguage == LanguageCode.N)
		{
			if (availableLanguages.Count > 0)
			{
				DoSwitch(LocalizationSettings.GetLanguageEnum(availableLanguages[0]));
				UnityEngine.Debug.LogError("Switched to " + currentLanguage + " instead");
			}
			else
			{
				UnityEngine.Debug.LogError(("Please verify that you have the file: Resources/Languages/" + code) ?? "");
				UnityEngine.Debug.Break();
			}
		}
		return false;
	}

	private static void DoSwitch(LanguageCode newLang)
	{
		PlayerPrefs.SetString("M2H_lastLanguage", string.Concat(newLang));
		currentLanguage = newLang;
		currentEntrySheets = new Dictionary<string, Dictionary<string, string>>();
		string[] sheetTitles = settings.sheetTitles;
		foreach (string text in sheetTitles)
		{
			currentEntrySheets[text] = new Dictionary<string, string>();
			string languageFileContents = GetLanguageFileContents(text);
			if (languageFileContents != "")
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(languageFileContents)))
				{
					while (xmlReader.ReadToFollowing("entry"))
					{
						xmlReader.MoveToFirstAttribute();
						string value = xmlReader.Value;
						xmlReader.MoveToElement();
						string text2 = xmlReader.ReadElementContentAsString().Trim();
						text2 = text2.Replace("\\n", "\n");
						text2 = text2.UnescapeXML();
						currentEntrySheets[text][value] = text2;
					}
				}
			}
		}
		LocalizedAsset[] array = (LocalizedAsset[])UnityEngine.Object.FindObjectsOfType(typeof(LocalizedAsset));
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LocalizeAsset();
		}
		SendMonoMessage("ChangedLanguage", currentLanguage);
	}

	public static UnityEngine.Object GetAsset(string name)
	{
		return Resources.Load("Languages/Assets/" + CurrentLanguage() + "/" + name);
	}

	private static bool HasLanguageFile(string lang, string sheetTitle)
	{
		return (TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null;
	}

	private static string GetLanguageFileContents(string sheetTitle)
	{
		TextAsset textAsset = (TextAsset)Resources.Load("Languages/" + currentLanguage + "_" + sheetTitle, typeof(TextAsset));
		if (!(textAsset != null))
		{
			return "";
		}
		return textAsset.text;
	}

	public static LanguageCode CurrentLanguage()
	{
		return currentLanguage;
	}

	public static string Get(string key)
	{
		return Get(key, settings.sheetTitles[0]);
	}

	public static Dictionary<string, string> GetSheet(string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			UnityEngine.Debug.LogError("The sheet with title \"" + sheetTitle + "\" does not exist!");
			return null;
		}
		return currentEntrySheets[sheetTitle];
	}

	public static string Get(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			UnityEngine.Debug.LogError("The sheet with title \"" + sheetTitle + "\" does not exist!");
			return "";
		}
		if (currentEntrySheets[sheetTitle].ContainsKey(key))
		{
			return currentEntrySheets[sheetTitle][key];
		}
		return "#!#" + key + "#!#";
	}

	public static bool Has(string key)
	{
		return Has(key, settings.sheetTitles[0]);
	}

	public static bool Has(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			return false;
		}
		return currentEntrySheets[sheetTitle].ContainsKey(key);
	}

	private static void SendMonoMessage(string methodString, params object[] parameters)
	{
		if (parameters != null && parameters.Length > 1)
		{
			UnityEngine.Debug.LogError("We cannot pass more than one argument currently!");
		}
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject && gameObject.transform.parent == null)
			{
				if (parameters != null && parameters.Length == 1)
				{
					gameObject.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static LanguageCode LanguageNameToCode(SystemLanguage name)
	{
		switch (name)
		{
		case SystemLanguage.Afrikaans:
			return LanguageCode.AF;
		case SystemLanguage.Arabic:
			return LanguageCode.AR;
		case SystemLanguage.Basque:
			return LanguageCode.BA;
		case SystemLanguage.Belarusian:
			return LanguageCode.BE;
		case SystemLanguage.Bulgarian:
			return LanguageCode.BG;
		case SystemLanguage.Catalan:
			return LanguageCode.CA;
		case SystemLanguage.Chinese:
			return LanguageCode.ZH;
		case SystemLanguage.Czech:
			return LanguageCode.CS;
		case SystemLanguage.Danish:
			return LanguageCode.DA;
		case SystemLanguage.Dutch:
			return LanguageCode.NL;
		case SystemLanguage.English:
			return LanguageCode.EN;
		case SystemLanguage.Estonian:
			return LanguageCode.ET;
		case SystemLanguage.Faroese:
			return LanguageCode.FA;
		case SystemLanguage.Finnish:
			return LanguageCode.FI;
		case SystemLanguage.French:
			return LanguageCode.FR;
		case SystemLanguage.German:
			return LanguageCode.DE;
		case SystemLanguage.Greek:
			return LanguageCode.EL;
		case SystemLanguage.Hebrew:
			return LanguageCode.HE;
		case SystemLanguage.Hungarian:
			return LanguageCode.HU;
		case SystemLanguage.Icelandic:
			return LanguageCode.IS;
		case SystemLanguage.Indonesian:
			return LanguageCode.ID;
		case SystemLanguage.Italian:
			return LanguageCode.IT;
		case SystemLanguage.Japanese:
			return LanguageCode.JA;
		case SystemLanguage.Korean:
			return LanguageCode.KO;
		case SystemLanguage.Latvian:
			return LanguageCode.LA;
		case SystemLanguage.Lithuanian:
			return LanguageCode.LT;
		case SystemLanguage.Norwegian:
			return LanguageCode.NO;
		case SystemLanguage.Polish:
			return LanguageCode.PL;
		case SystemLanguage.Portuguese:
			return LanguageCode.PT;
		case SystemLanguage.Romanian:
			return LanguageCode.RO;
		case SystemLanguage.Russian:
			return LanguageCode.RU;
		case SystemLanguage.SerboCroatian:
			return LanguageCode.SH;
		case SystemLanguage.Slovak:
			return LanguageCode.SK;
		case SystemLanguage.Slovenian:
			return LanguageCode.SL;
		case SystemLanguage.Spanish:
			return LanguageCode.ES;
		case SystemLanguage.Swedish:
			return LanguageCode.SW;
		case SystemLanguage.Thai:
			return LanguageCode.TH;
		case SystemLanguage.Turkish:
			return LanguageCode.TR;
		case SystemLanguage.Ukrainian:
			return LanguageCode.UK;
		case SystemLanguage.Vietnamese:
			return LanguageCode.VI;
		default:
			if (name == SystemLanguage.Hungarian)
			{
				return LanguageCode.HU;
			}
			return LanguageCode.N;
		}
	}
}
