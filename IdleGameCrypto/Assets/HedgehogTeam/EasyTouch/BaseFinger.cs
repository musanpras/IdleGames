using UnityEngine;

namespace HedgehogTeam.EasyTouch
{
	public class BaseFinger
	{
		public int fingerIndex;

		public int touchCount;

		public Vector2 startPosition;

		public Vector2 position;

		public Vector2 deltaPosition;

		public float actionTime;

		public float deltaTime;

		public Camera pickedCamera;

		public GameObject pickedObject;

		public bool isGuiCamera;

		public bool isOverGui;

		public GameObject pickedUIElement;

		public Gesture GetGesture()
		{
			return new Gesture
			{
				fingerIndex = fingerIndex,
				touchCount = touchCount,
				startPosition = startPosition,
				position = position,
				deltaPosition = deltaPosition,
				actionTime = actionTime,
				deltaTime = deltaTime,
				isOverGui = isOverGui,
				pickedCamera = pickedCamera,
				pickedObject = pickedObject,
				isGuiCamera = isGuiCamera,
				pickedUIElement = pickedUIElement
			};
		}
	}
}
