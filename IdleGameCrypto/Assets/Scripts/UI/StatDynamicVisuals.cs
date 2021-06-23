using System;
using UnityEngine;

    [Serializable]
    public struct StatDynamicVisuals
    {
        [SerializeField]
       // [Range(0, 99)]
        private Vector2 MinMaxValues;
        [SerializeField]
        public Sprite Image;
        [HideInInspector]
        public int MinValue => Mathf.FloorToInt(MinMaxValues.x);
        [HideInInspector]
        public int MaxValue => Mathf.FloorToInt(MinMaxValues.y);
    }
