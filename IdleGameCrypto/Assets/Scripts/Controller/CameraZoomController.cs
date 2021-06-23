using DG.Tweening;
using HedgehogTeam.EasyTouch;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

	public class CameraZoomController : MonoBehaviour
	{
		public Camera Camera;

		//[MinMaxSlider(0f, 50f, true)]
		[SerializeField]
		private Vector2 _zoom;

		private Vector3 OriginalDist;

		public CameraMovementController cameraMovement;

		private bool initialized;

		private bool wasCameraMovementEnabled;

		public float ZOOM_INVERSE_SPEED = 750f;

		private float MaxZoom => _zoom.x;

		private float MinZoom => _zoom.y;

		public bool ListeningEvents
		{
			get;
			protected set;
		}

		public float CurrentZoomFactor
		{
			get;
			protected set;
		}

		public float DistanceToGround => (Camera.farClipPlane - Camera.nearClipPlane) * 0.5f;

		public void OnEnable()
		{
			if (!initialized)
			{
				initialized = true;
				CurrentZoomFactor = 1f;
			}
			if (!ListeningEvents)
			{
				EnableZoom();
				ListeningEvents = true;
			}
		}

		public void EnableZoomFromTutorial()
		{
			if (!ListeningEvents)
			{
				ListeningEvents = true;
				EasyTouch.On_PinchIn += HandleOn_PinchIn;
				EasyTouch.On_PinchOut += HandleOn_PinchOut;
				EasyTouch.On_Pinch += HandleOn_Pinch;
				EasyTouch.On_PinchEnd += HandleOn_PinchEnd;
			}
		}

		public void DisableZoomFromTutorial()
		{
			if (ListeningEvents)
			{
				ListeningEvents = false;
				EasyTouch.On_PinchIn -= HandleOn_PinchIn;
				EasyTouch.On_PinchOut -= HandleOn_PinchOut;
				EasyTouch.On_Pinch -= HandleOn_Pinch;
				EasyTouch.On_PinchEnd -= HandleOn_PinchEnd;
				HandleOn_PinchEnd(null);
			}
		}

		public void EnableZoom()
		{
			if (!ListeningEvents)
			{
				ListeningEvents = true;
				EasyTouch.On_PinchIn += HandleOn_PinchIn;
				EasyTouch.On_PinchOut += HandleOn_PinchOut;
				EasyTouch.On_Pinch += HandleOn_Pinch;
				EasyTouch.On_PinchEnd += HandleOn_PinchEnd;
			}
		}

		public void DisableZoom()
		{
			if (ListeningEvents)
			{
				ListeningEvents = false;
				EasyTouch.On_PinchIn -= HandleOn_PinchIn;
				EasyTouch.On_PinchOut -= HandleOn_PinchOut;
				EasyTouch.On_Pinch -= HandleOn_Pinch;
				EasyTouch.On_PinchEnd -= HandleOn_PinchEnd;
				HandleOn_PinchEnd(null);
			}
		}

		private void OnDisable()
		{
			DisableZoom();
		}

		protected bool IsPointerOverUiObject(int pointerId)
		{
			return EventSystem.current.IsPointerOverGameObject(pointerId);
		}

		private void HandleOn_Pinch(Gesture gesture)
		{
			if (!IsPointerOverUiObject(gesture.fingerIndex) && !gesture.isOverGui && cameraMovement != null && cameraMovement.ListeningEvents)
			{
				wasCameraMovementEnabled = true;
				cameraMovement.OnDisable();
			}
		}

		private void HandleOn_PinchEnd(Gesture gesture)
		{
			if (cameraMovement != null && wasCameraMovementEnabled)
			{
				cameraMovement.OnEnable();
			}
			wasCameraMovementEnabled = false;
		}

		private void HandleOn_PinchOut(Gesture gesture)
		{
			if (!IsPointerOverUiObject(gesture.fingerIndex) && !gesture.isOverGui)
			{
				ZoomOut(gesture.deltaPinch / ZOOM_INVERSE_SPEED);
			}
		}

		private void HandleOn_PinchIn(Gesture gesture)
		{
			if (!IsPointerOverUiObject(gesture.fingerIndex) && !gesture.isOverGui)
			{
				ZoomIn(gesture.deltaPinch / ZOOM_INVERSE_SPEED);
			}
		}

		private void ZoomIn(float zoomDelta)
		{
			SetZoom(CurrentZoomFactor + zoomDelta);
		}

		private void ZoomOut(float delta)
		{
			SetZoom(CurrentZoomFactor - delta);
		}

		private void SetZoom(float zoomFactor)
		{
			CurrentZoomFactor = Mathf.Clamp01(zoomFactor);
			Camera.orthographicSize = Mathf.Lerp(MaxZoom, MinZoom, zoomFactor);
		}

		public void SetZoomAnimated(float zoomFactor, float duration = 0.2f, Action _completeCallback = null)
		{
			zoomFactor = Mathf.Clamp01(zoomFactor);
			Camera.DOOrthoSize(zoomFactor, duration).OnComplete(delegate
			{
				CurrentZoomFactor = zoomFactor;
				_completeCallback?.Invoke();
			});
		}
	}

