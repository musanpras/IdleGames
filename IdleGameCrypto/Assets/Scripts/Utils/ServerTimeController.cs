using System;
using UnityEngine;

    public class ServerTimeController 
    {
        public class SecondElapsedEvent 
        {
            public int Time
            {
                get;
                private set;
            }
            public SecondElapsedEvent(int time)
            {
                Time = time;
            }
        }
       
        private int _serverReferenceTime;
        private int _localTimeWhenReferenceSetted;
        private float _timeElapsed;
        public int ServerReferenceTime
        {
            get
            {
                return _serverReferenceTime;
            }
            set
            {
                _localTimeWhenReferenceSetted = DateTime.UtcNow.ToTimestamp();
                _serverReferenceTime = value;
            }
        }
        public int Time => GetServerTimestamp();
        protected int GetDelta()
        {
            return DateTime.UtcNow.ToTimestamp() - _localTimeWhenReferenceSetted;
        }
        public DateTime GetServerTime()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(GetServerTimestamp());
        }
        public int GetServerTimestamp()
        {
            return _serverReferenceTime + GetDelta();
        }
        public int GetHoursDifferenceWithServer()
        {
            return (DateTime.Now - DateTime.UtcNow).Hours;
        }
        public void Tick()
        {
            if (_timeElapsed >= 1f)
            {
                _timeElapsed -= 1f;
            }
            else
            {
                _timeElapsed += UnityEngine.Time.deltaTime;
            }
        }
    }
