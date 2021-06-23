using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

	public class GameObjectPool : MonoBehaviour
	{
		public static GameObjectPool _instance;

		public GameObject[] prefabs;

	    public int[] totalPreInList;

		public Vector3 poolPosition;

		public Dictionary<string, List<GameObject>> objetos = new Dictionary<string, List<GameObject>>();

	    public Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

		private bool _initialized;

		private void Awake()
		{
			_instance = this;
			Init();
		}

		public void Init()
		{
			if (_initialized)
			{
				return;
			}
			for (int i = 0; i < prefabs.Length; i++)
			{
			string name = prefabs[i].name;
			if (!nameToIndex.ContainsKey(name))
			{

				nameToIndex.Add(name, i);
				objetos[name] = new List<GameObject>();
				for (int j = 0; j < totalPreInList[i]; j++)
				{

					GameObject gameObject = NewInstance(prefabs[i]);
					objetos[name].Add(gameObject);
					gameObject.SetActive(value: false);

				}

			}
			}
			_initialized = true;
		}

		public GameObject getObject(string tipo)
		{
			for (int i = 0; i < objetos[tipo].Count; i++)
			{
			    
				if(! objetos[tipo][i].activeSelf)
				{
				objetos[tipo][i].SetActive(true);
				return objetos[tipo][i];
				}
			}

		GameObject _gameObject = NewInstance(prefabs[nameToIndex[tipo]]);
		objetos[tipo].Add(_gameObject);

			return _gameObject;
		}

		public void unloadObject(GameObject t)
		{
			t.SetActive(value: false);
			t.transform.SetParent(base.transform);
			t.transform.position = poolPosition;
		}

		protected GameObject NewInstance(GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.SetParent(base.transform, worldPositionStays: true);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.position = poolPosition;
			return gameObject;
		}

		internal object getObject(object controller)
		{
			throw new NotImplementedException();
		}
	}
