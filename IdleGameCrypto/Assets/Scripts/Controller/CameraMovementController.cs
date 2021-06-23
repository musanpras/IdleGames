using DG.Tweening;
using HedgehogTeam.EasyTouch;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


	public class CameraMovementController : MonoBehaviour
	{
		private enum enumStatus
		{
			NONE,
			SWIPE,
			INTERTIA,
			AUTOMOVE
		}

		public enum enumAxis
		{
			BOTH,
			HORIZONTAL,
			VERTICAL
		}

		public CameraZoomController cameraZoom;

		private enumStatus status;

		public enumAxis Axis;

		private Camera currentCamera;

		[HideInInspector]
		public Transform currentCameraTransform;

		public float inertia = 5f;

		private Vector2[] touch2D = new Vector2[2]
		{
			Vector2.zero,
			Vector2.zero
		};

		private Vector3[] touch3D = new Vector3[2]
		{
			Vector3.zero,
			Vector3.zero
		};

		public Plane plane = new Plane(new Vector3(0f, 1f, 0f), 0f);

		private Vector3 targetPosition;

		private Vector3 lastMove;

		[SerializeField]
		private Transform _cameraBound1;

		[SerializeField]
		private Transform _cameraBound2;

		[HideInInspector]
		public Transform priorityBound1;

		[HideInInspector]
		public Transform priorityBound2;

		public float horizontalMargin;

		public float depthMargin;

		//public Camera UICamera;

		public GameObject[] NonGUIObjects;

		private bool lastSwipeStartOk;

		public Camera CurrentCamera
		{
			get
			{
				return currentCamera;
			}
			set
			{
				currentCamera = value;
				currentCameraTransform = currentCamera.transform;
			}
		}

		public bool ListeningEvents
		{
			get;
			private set;
		}

		public Transform CameraBound1
		{
			get
			{
				if (priorityBound1 != null)
				{
					return priorityBound1;
				}
				return _cameraBound1;
			}
		}

		public Transform CameraBound2
		{
			get
			{
				if (priorityBound2 != null)
				{
					return priorityBound2;
				}
				return _cameraBound2;
			}
		}

		public bool IsInitialized
		{
			get;
			private set;
		}

		public bool IsLocked
		{
			get;
			private set;
		}

		public GameObject ElementLockedOn
		{
			get;
			private set;
		}

		public bool IsLockedOnElement => ElementLockedOn != null;

		private void Start()
		{
			if (!IsInitialized)
			{
				Initialize();
			}
		}

		public void Initialize()
		{

			IsInitialized = true;
			CurrentCamera = GetComponent<Camera>();
			targetPosition = currentCameraTransform.position;
			Vector3 pointInPlane = MathUtil.GetPointInPlane(CurrentCamera, new Vector3(Screen.width / 2, Screen.height / 2, 0f), plane);
			Vector3 position = pointInPlane + (CurrentCamera.transform.position - pointInPlane).normalized * cameraZoom.DistanceToGround;
			CurrentCamera.transform.position = position;
			Unlock();
		}

		public void OnEnableFromTutorial()
		{
			if (!ListeningEvents)
			{
				EasyTouch.On_Swipe += HandleOn_Swipe;
				EasyTouch.On_SwipeStart += HandleOn_SwipeStart;
				EasyTouch.On_SwipeEnd += HandleOn_SwipeEnd;
				ListeningEvents = true;
			}
		}

		public void OnDisableFromTutorial()
		{
			EasyTouch.On_Swipe -= HandleOn_Swipe;
			EasyTouch.On_SwipeStart -= HandleOn_SwipeStart;
			EasyTouch.On_SwipeEnd -= HandleOn_SwipeEnd;
			ListeningEvents = false;
		}

		public void OnEnable()
		{
			if (!ListeningEvents)
			{
			    EasyTouch.On_Swipe += HandleOn_Swipe;
				EasyTouch.On_SwipeStart += HandleOn_SwipeStart;
				EasyTouch.On_SwipeEnd += HandleOn_SwipeEnd;
				ListeningEvents = true;
			}
		}

		public void OnDisable()
		{
			EasyTouch.On_Swipe -= HandleOn_Swipe;
			EasyTouch.On_SwipeStart -= HandleOn_SwipeStart;
			EasyTouch.On_SwipeEnd -= HandleOn_SwipeEnd;
			ListeningEvents = false;
		}

		protected bool IsPointerOverUiObject(int pointerId)
		{
			return EventSystem.current.IsPointerOverGameObject(pointerId);
		}

		private void HandleOn_SwipeStart(Gesture gesture)
		{
			if (!(currentCamera == null))
			{
				if (IsPointerOverUiObject(gesture.fingerIndex))
				{
					lastSwipeStartOk = false;
					return;
				}
				lastSwipeStartOk = true;
				targetPosition = currentCameraTransform.position;
				status = enumStatus.SWIPE;
				touch2D[0] = gesture.startPosition;
				HandleOn_Swipe(gesture);
			}
		}

		private void HandleOn_Swipe(Gesture gesture)
		{
		    // Debug.Log("SWPIPE 123");
			if (lastSwipeStartOk && !(currentCamera == null) && Input.touches.Length <= 1)
			{
				touch2D[1] = gesture.position;
				touch3D[0] = MathUtil.GetPointInPlane(CurrentCamera, touch2D[0], plane);
				touch3D[1] = MathUtil.GetPointInPlane(CurrentCamera, touch2D[1], plane);
				Vector3 vector = touch3D[1] - touch3D[0];
				switch (Axis)
				{
				case enumAxis.HORIZONTAL:
					vector.y = 0f;
					break;
				case enumAxis.VERTICAL:
					vector.x = 0f;
					break;
				}
				touch2D[0] = touch2D[1];
				targetPosition -= vector;
			}
		}

		private void HandleOn_SwipeEnd(Gesture gesture)
		{
			status = enumStatus.INTERTIA;
		}

		public void ApplyMovement(Vector3 _delta)
		{
			targetPosition = currentCameraTransform.position - currentCameraTransform.right * _delta.x - currentCameraTransform.up * _delta.y;
			targetPosition = GetCorrectedCameraPosition(targetPosition);
			status = enumStatus.AUTOMOVE;
		}

		public void MoveTween(Vector3 target, float timeTransition, bool applyAxisLimitation = true)
		{
			Vector3 pointInPlane = MathUtil.GetPointInPlane(currentCamera, new Vector3(Screen.width / 2, Screen.height / 2, 0f), plane);
			Vector3 b = target - pointInPlane;
			targetPosition = GetCorrectedCameraPosition(currentCameraTransform.position + b);
			if (applyAxisLimitation && Axis != 0)
			{
				Vector3 point = currentCamera.WorldToScreenPoint(target);
				switch (Axis)
				{
				case enumAxis.HORIZONTAL:
					point.y = Screen.height / 2;
					break;
				case enumAxis.VERTICAL:
					point.x = Screen.width / 2;
					break;
				}
				Vector3 pointInPlane2 = MathUtil.GetPointInPlane(currentCamera, point, plane);
				MoveTween(pointInPlane2, timeTransition, applyAxisLimitation: false);
			}
			if (timeTransition == 0f)
			{
				SetCameraPosition(targetPosition);
				RepositionCamera();
			}
			else
			{
				currentCameraTransform.DOMove(targetPosition, timeTransition).OnUpdate(delegate
				{
					SetCameraPosition(currentCameraTransform.position);
					RepositionCamera();
				}).SetEase(Ease.OutQuad);
			}
		}

		private void Update()
		{
			if (IsLockedOnElement)
			{
				FocusOnWorldPoint(ElementLockedOn.transform.position);
			}
			else
			{
				if (IsLocked)
				{
					return;
			//	Debug.Log("LOCK CAM");
				}
		//	Debug.Log("UNLOCK CAM");
			switch (status)
				{
				case enumStatus.SWIPE:
				{
					Vector3 vector = Vector3.Lerp(currentCameraTransform.position, targetPosition, Mathf.Clamp(15f * Time.deltaTime, 0f, 1f));
					lastMove = vector - currentCameraTransform.position;
					SetCameraPosition(vector);
					RepositionCamera();
					break;
				}
				case enumStatus.INTERTIA:
					SetCameraPosition(currentCameraTransform.position + lastMove);
					lastMove = Vector3.Lerp(lastMove, Vector3.zero, Mathf.Clamp(5f * Time.deltaTime, 0f, 1f));
					if (lastMove.magnitude < 0.05f)
					{
						status = enumStatus.NONE;
					}
					RepositionCamera();
					break;
				case enumStatus.AUTOMOVE:
				{
					Vector3 vector = Vector3.Lerp(currentCameraTransform.position, targetPosition, Mathf.Clamp(15f * Time.deltaTime, 0f, 1f));
					lastMove = vector - currentCameraTransform.position;
					SetCameraPosition(vector);
					RepositionCamera();
					if ((targetPosition - currentCameraTransform.position).magnitude < 0.1f)
					{
						status = enumStatus.INTERTIA;
					}
					break;
				}
				}
			}
		}

		public void RepositionCamera()
		{
			Vector3 position = currentCameraTransform.position;
			if (currentCameraTransform.position.x < CameraBound1.position.x)
			{
				position.x = CameraBound1.position.x;
			}
			if (currentCameraTransform.position.x > CameraBound2.position.x)
			{
				position.x = CameraBound2.position.x;
			}
			if (currentCameraTransform.position.z < CameraBound1.position.z)
			{
				position.z = CameraBound1.position.z;
			}
			if (currentCameraTransform.position.z > CameraBound2.position.z)
			{
				position.z = CameraBound2.position.z;
			}
			currentCameraTransform.position = position;
		}

		protected void IteratePosition(Vector3 optimum, Vector3 direction, bool testX, bool testY)
		{
			int num = 150;
			int num2 = 20;
			float num3 = 5f;
			Vector3 vector = Vector3.zero;
			Vector3 cameraPosition = currentCameraTransform.position;
			for (int i = 0; i < num; i++)
			{
				vector = optimum + direction * (num3 * (float)i);
				vector = SetCameraPosition(vector);
				if (IsValidPosition(testX, testY))
				{
					break;
				}
			}
			if (IsValidPosition(testX, testY))
			{
				for (int j = 0; j < num2; j++)
				{
					Vector3 cameraPosition2 = (optimum + vector) / 2f;
					cameraPosition2 = SetCameraPosition(cameraPosition2);
					if (IsValidPosition(testX, testY))
					{
						vector = cameraPosition2;
						cameraPosition = vector;
					}
					else
					{
						optimum = cameraPosition2;
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError("IteratePosition(): Valid position not found!");
			}
			SetCameraPosition(cameraPosition);
		}

		public Vector3 SetCameraPosition(Vector3 pos)
		{
			currentCameraTransform.position = pos;
			Vector3 pointInPlane = MathUtil.GetPointInPlane(CurrentCamera, new Vector3(Screen.width / 2, Screen.height / 2, 0f), plane);
			Vector3 position = pointInPlane + (pos - pointInPlane).normalized * cameraZoom.DistanceToGround;
			currentCameraTransform.position = position;
			return currentCameraTransform.position;
		}

		public Vector3 GetCorrectedCameraPosition(Vector3 posCamera)
		{
			Vector3 position = currentCameraTransform.position;
			currentCameraTransform.position = posCamera;
			Vector3 pointInPlane = MathUtil.GetPointInPlane(CurrentCamera, new Vector3(Screen.width / 2, Screen.height / 2, 0f), plane);
			Vector3 result = pointInPlane + (posCamera - pointInPlane).normalized * cameraZoom.DistanceToGround;
			currentCameraTransform.position = position;
			return result;
		}

		protected bool IsValidPosition(bool testX, bool testY)
		{
			RefreshBounds(out Vector3 _, out Vector3 _);
			if ((CameraBound1.position.x >= currentCamera.transform.position.x || CameraBound2.position.x <= currentCamera.transform.position.x) & testX)
			{
				return false;
			}
			if ((CameraBound1.position.z >= currentCamera.transform.position.z || CameraBound2.position.z <= currentCamera.transform.position.z) & testY)
			{
				return false;
			}
			return true;
		}

		public void RefreshBounds(out Vector3 lowerBound, out Vector3 upperBound)
		{
			if (CameraBound1 == null || CameraBound2 == null)
			{
				lowerBound = Vector3.zero;
				upperBound = Vector3.zero;
				return;
			}
			Vector3 position = CameraBound1.position;
			Vector3 position2 = CameraBound2.position;
			if (position.x < position2.x)
			{
				lowerBound.x = position.x;
				upperBound.x = position2.x;
			}
			else
			{
				lowerBound.x = position2.x;
				upperBound.x = position.x;
			}
			if (position.y < position2.y)
			{
				lowerBound.y = position.y;
				upperBound.y = position2.y;
			}
			else
			{
				lowerBound.y = position2.y;
				upperBound.y = position.y;
			}
			if (position.z < position2.z)
			{
				lowerBound.z = position.z;
				upperBound.z = position2.z;
			}
			else
			{
				lowerBound.z = position2.z;
				upperBound.z = position.z;
			}
		}

		public void Lock()
		{
			IsLocked = true;
		}

		public void Unlock()
		{
			IsLocked = false;
			ElementLockedOn = null;
		}

		public void LockOnElement(GameObject element)
		{
			Lock();
			ElementLockedOn = element;
		}

		public void FocusOnWorldPointAnimated(Vector3 worldPoint, float duration = 1f, Action onFinished = null)
		{
			Camera camera = CurrentCamera;
			Vector3 endValue = CalculateCameraPositionToFocusOnPoint(camera, worldPoint);
			camera.transform.DOMove(endValue, duration).OnComplete(delegate
			{
				onFinished?.Invoke();
			});
		}

		public void FocusOnWorldPoint(Vector3 worldPoint)
		{
			Camera camera = CurrentCamera;
			Vector3 position = CalculateCameraPositionToFocusOnPoint(camera, worldPoint);
			camera.transform.position = position;
		}

		private Vector3 CalculateCameraPositionToFocusOnPoint(Camera camera, Vector3 worldPoint)
		{
			Vector3 b = CalculateFocusedPoint(camera);
			Vector3 b2 = worldPoint - b;
			return camera.transform.position + b2;
		}

		private Vector3 CalculateFocusedPoint(Camera camera)
		{
			Ray ray = camera.ViewportPointToRay(Vector2.one * 0.5f);
			float enter = 0f;
			if (plane.Raycast(ray, out enter))
			{
				return ray.GetPoint(enter);
			}
			return Vector3.zero;
		}
	}

