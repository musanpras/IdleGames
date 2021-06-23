
using System.Collections.Generic;
using UnityEngine;


	public class MarketVisualModel : MonoBehaviour
	{
		public Transform exit;

		public BoxCollider waitZone;

		public BoxCollider checkoutWaitZone;

		[SerializeField]
		private Transform zone_bakery;

		[SerializeField]
		private Transform zone_fruit;

		[SerializeField]
		private Transform zone_meat;

		[SerializeField]
		private Transform zone_fish;

		[SerializeField]
		private Transform zone_clothing;

		[SerializeField]
		private Transform zone_fragance;

		[SerializeField]
		private Transform zone_electronics;

		[SerializeField]
		private Transform zone_smartphone;

		[SerializeField]
		private Transform zone_jewels;

		[SerializeField]
		private Transform zone_cars;

		[SerializeField]
		private Transform zone_house;

		[SerializeField]
		private Transform zone_checkout1;

		[SerializeField]
		private Transform zone_checkout2;

		[SerializeField]
		private Transform zone_checkout3;

		[SerializeField]
		private Transform zone_checkout4;

		[SerializeField]
		private Transform zone_checkout5;

		[SerializeField]
		private Transform zone_checkout6;

		[SerializeField]
		private Transform zone_checkout7;

		[SerializeField]
		private Transform zone_checkout8;

		[SerializeField]
		private Transform zone_checkout9;

		[SerializeField]
		private Transform zone_checkout10;

		public Transform[] pointsVIPGems;

		public Transform[] pointsVIPShops;

		public Transform generalManager;

		//public WorldNotificationAlert managerNotification;

		public Dictionary<string, Transform> zone2Transform = new Dictionary<string, Transform>();

		public void Init()
		{
			zone2Transform.Add("BAKERY", zone_bakery);
			zone2Transform.Add("FRUIT", zone_fruit);
			zone2Transform.Add("MEAT", zone_meat);
			zone2Transform.Add("FISH", zone_fish);
			zone2Transform.Add("CLOTHING", zone_clothing);
			zone2Transform.Add("FRAGANCE", zone_fragance);
			zone2Transform.Add("ELECTRONICS", zone_electronics);
			zone2Transform.Add("SMARTPHONE", zone_smartphone);
			zone2Transform.Add("JEWEL", zone_jewels);
			zone2Transform.Add("CARS", zone_cars);
			zone2Transform.Add("HOUSE", zone_house);
			zone2Transform.Add("CHECKOUT1", zone_checkout1);
			zone2Transform.Add("CHECKOUT2", zone_checkout2);
			zone2Transform.Add("CHECKOUT3", zone_checkout3);
			zone2Transform.Add("CHECKOUT4", zone_checkout4);
			zone2Transform.Add("CHECKOUT5", zone_checkout5);
			zone2Transform.Add("CHECKOUT6", zone_checkout6);
			zone2Transform.Add("CHECKOUT7", zone_checkout7);
			zone2Transform.Add("CHECKOUT8", zone_checkout8);
			zone2Transform.Add("CHECKOUT9", zone_checkout9);
			zone2Transform.Add("CHECKOUT10", zone_checkout10);
			//managerNotification.Initialize();
		}

		public int GetMaxCashiers()
		{
		/*
			for (int i = 1; i <= MarketConfigurationStaticModel.maxCashiers; i++)
			{
				if (!zone2Transform.ContainsKey("CHECKOUT" + i) || zone2Transform["CHECKOUT" + i] == null)
				{
					return i - 1;
				}
			}
			*/
			return 0;
		}
	}

