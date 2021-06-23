
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;


    public abstract class OfficeManagerTab : MonoBehaviour, IDisposable
    {
        public UnityEvent OnOperationPerformed;
        
        protected bool IsSubscribedToEvents
        {
            get;
            private set;
        }
        public void Initialize()
        {
            InitializeInternal();
            SubscribeToEvents();
        }
        protected virtual void InitializeInternal()
        {
        }
        public void Dispose()
        {
            UnSubscribeToEvents();
        }
        public abstract Sequence ShowAnimated(float duration = 0.2f);
        public abstract Sequence HideAnimated(float duration = 0.2f);
        private void SubscribeToEvents()
        {
            if (IsSubscribedToEvents)
            {
                UnSubscribeToEvents();
            }
           // MessageBus.Subscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
            SubscribeToEventsInternal();
            IsSubscribedToEvents = true;
        }
        private void UnSubscribeToEvents()
        {
            if (IsSubscribedToEvents)
            {
               // MessageBus.UnSubscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
                UnSubscribeToEventsInternal();
                IsSubscribedToEvents = false;
            }
        }
        protected virtual void SubscribeToEventsInternal()
        {
        }
        protected virtual void UnSubscribeToEventsInternal()
        {
        }
        private void OnSecondElapsedEvent()
        {
            OnSecondElapsed();
        }
        protected virtual void OnSecondElapsed()
        {
        }
    }
