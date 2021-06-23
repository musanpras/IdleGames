using Newtonsoft.Json;
using System;
using System.Collections.Generic;

	public class JsonSystem : IJsonSystem
	{
		//private HaxeConverter _haxeConverter;

		//private readonly UnixDateTimeConverter _dateTimeConverter;

		//private ACTJsonConverter _actConverter;

		//private readonly Vector2Converter _vector2Converter;

		//private Vector3Converter _vector3Converter;

		//private IntReactivePropertyConverter _intReactiveConverter;

		//private StringReactivePropertyConverter _stringReactivePropertyConverter;

		private readonly JsonConverter[] _converters;

		private readonly JsonSerializerSettings _settings;

		public JsonSystem()
		{
			/*
			_haxeConverter = new HaxeConverter();
			_dateTimeConverter = new UnixDateTimeConverter();
			_actConverter = new ACTJsonConverter();
			_vector2Converter = new Vector2Converter();
			_vector3Converter = new Vector3Converter();
			_intReactiveConverter = new IntReactivePropertyConverter();
			_stringReactivePropertyConverter = new StringReactivePropertyConverter();
			_converters = new JsonConverter[7]
			{
				_haxeConverter,
				_dateTimeConverter,
				_actConverter,
				_vector2Converter,
				_vector3Converter,
				_intReactiveConverter,
				_stringReactivePropertyConverter
			};
			*/
			_settings = new JsonSerializerSettings
			{
				Converters = _converters
			};
		}

		public T DeserializeObject<T>(string rawData)
		{
			return JsonConvert.DeserializeObject<T>(rawData, _settings);
		}

		public object DeserializeObject(Type type, string rawData)
		{
			return JsonConvert.DeserializeObject(rawData, type, _settings);
		}

		public void PopulateObject(string rawData, object target)
		{
			JsonConvert.PopulateObject(rawData, target, _settings);
		}

		public string SerializeObject(object data)
		{
			return JsonConvert.SerializeObject(data, _settings);
		}

		public void PopulateDictionary<K, V>(Dictionary<K, V> _data, Dictionary<K, V> _deserialized)
		{
			foreach (KeyValuePair<K, V> item in _deserialized)
			{
				if (!_data.ContainsKey(item.Key))
				{
					_data[item.Key] = item.Value;
				}
				else
				{
					string rawData = JsonConvert.SerializeObject(item.Value, new JsonSerializerSettings
					{
						Converters = _converters,
						NullValueHandling = NullValueHandling.Ignore
					});
					PopulateObject(rawData, _data[item.Key]);
				}
			}
		}
	}
