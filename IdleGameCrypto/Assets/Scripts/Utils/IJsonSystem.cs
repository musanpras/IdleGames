using System;
using System.Collections.Generic;

public interface IJsonSystem
{
    T DeserializeObject<T>(string rawData);
    object DeserializeObject(Type type, string rawData);
    void PopulateObject(string rawData, object target);
    void PopulateDictionary<K, V>(Dictionary<K, V> _data, Dictionary<K, V> _deserialized);
    string SerializeObject(object data);
}