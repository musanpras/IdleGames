using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


	public class FilteredInputModule : PointerInputModule
	{
		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public enum InputMode
		{
			Mouse,
			Buttons
		}

		public delegate void OnTouchUnhandled();

		public delegate void OnTouchHandled();

		private float m_PrevActionTime;

		private Vector2 m_LastMoveVector;

		private int m_ConsecutiveMoveCount;

		private Vector2 m_LastMousePosition;

		private Vector2 m_MousePosition;

		[SerializeField]
		private string m_HorizontalAxis = "Horizontal";

		[SerializeField]
		private string m_VerticalAxis = "Vertical";

		[SerializeField]
		private string m_SubmitButton = "Submit";

		[SerializeField]
		private string m_CancelButton = "Cancel";

		[SerializeField]
		private float m_InputActionsPerSecond = 10f;

		[SerializeField]
		private float m_RepeatDelay = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
		private bool m_ForceModuleActive;

		[HideInInspector]
		public OnTouchUnhandled onTouchUnhandled;

		[HideInInspector]
		public OnTouchHandled onTouchHandled;

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public InputMode inputMode => InputMode.Mouse;

		[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
		public bool allowActivationOnMobileDevice
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public bool forceModuleActive
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public float inputActionsPerSecond
		{
			get
			{
				return m_InputActionsPerSecond;
			}
			set
			{
				m_InputActionsPerSecond = value;
			}
		}

		public float repeatDelay
		{
			get
			{
				return m_RepeatDelay;
			}
			set
			{
				m_RepeatDelay = value;
			}
		}

		public string horizontalAxis
		{
			get
			{
				return m_HorizontalAxis;
			}
			set
			{
				m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return m_VerticalAxis;
			}
			set
			{
				m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return m_SubmitButton;
			}
			set
			{
				m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return m_CancelButton;
			}
			set
			{
				m_CancelButton = value;
			}
		}

		public string clickableObject
		{
			get;
			set;
		}

		public string[] clickableWithoutCallbackObjects
		{
			get;
			set;
		}

		public bool isFilterEnabled => !string.IsNullOrEmpty(clickableObject);

		protected FilteredInputModule()
		{
		}

		public override void UpdateModule()
		{
			m_LastMousePosition = m_MousePosition;
			m_MousePosition = base.input.mousePosition;
		}

		public override bool IsModuleSupported()
		{
			if (!m_ForceModuleActive && !base.input.mousePresent)
			{
				return base.input.touchSupported;
			}
			return true;
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			bool forceModuleActive = m_ForceModuleActive;
			forceModuleActive |= base.input.GetButtonDown(m_SubmitButton);
			forceModuleActive |= base.input.GetButtonDown(m_CancelButton);
			forceModuleActive |= !Mathf.Approximately(base.input.GetAxisRaw(m_HorizontalAxis), 0f);
			forceModuleActive |= !Mathf.Approximately(base.input.GetAxisRaw(m_VerticalAxis), 0f);
			forceModuleActive |= ((m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f);
			forceModuleActive |= base.input.GetMouseButtonDown(0);
			if (base.input.touchCount > 0)
			{
				forceModuleActive = true;
			}
			return forceModuleActive;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			m_MousePosition = base.input.mousePosition;
			m_LastMousePosition = base.input.mousePosition;
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			ClearSelection();
		}

		public override void Process()
		{
			bool flag = SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag |= SendMoveEventToSelectedObject();
				}
				if (!flag)
				{
					SendSubmitEventToSelectedObject();
				}
			}
			if (!ProcessTouchEvents() && base.input.mousePresent)
			{
				ProcessMouseEvent();
			}
		}

		private bool ProcessTouchEvents()
		{
			for (int i = 0; i < base.input.touchCount; i++)
			{
				Touch touch = base.input.GetTouch(i);
				if (touch.type != TouchType.Indirect)
				{
					bool pressed;
					bool released;
					PointerEventData touchPointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
					ProcessTouchPress(touchPointerEventData, pressed, released);
					if (!released)
					{
						ProcessMove(touchPointerEventData);
						ProcessDrag(touchPointerEventData);
					}
					else
					{
						RemovePointerData(touchPointerEventData);
					}
				}
			}
			return base.input.touchCount > 0;
		}

		protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
		{
			GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(gameObject);
				bool flag = true;
				if (eventHandler == null)
				{
					eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
					flag = false;
				}
				if (IsClickable(eventHandler, pressed, released, out bool _) && flag)
				{
					ExecuteEvents.Execute(eventHandler, pointerEvent, ExecuteEvents.pointerDownHandler);
				}
				float unscaledTime = Time.unscaledTime;
				if (eventHandler == pointerEvent.lastPress)
				{
					if (unscaledTime - pointerEvent.clickTime < 0.3f)
					{
						int num = ++pointerEvent.clickCount;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = eventHandler;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (IsClickable(eventHandler2, pressed, released, out bool _))
				{
					pointerEvent.pointerDrag = eventHandler2;
					if (pointerEvent.pointerDrag != null)
					{
						ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
					}
				}
			}
			if (!released)
			{
				return;
			}
			bool isMainClickableGO3;
			bool flag2 = IsClickable(pointerEvent.pointerPress, pressed, released, out isMainClickableGO3);
			if (flag2)
			{
				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
			}
			GameObject eventHandler3 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if (pointerEvent.pointerPress == eventHandler3 && pointerEvent.eligibleForClick)
			{
				if (flag2)
				{
					ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
					if (isMainClickableGO3 && onTouchHandled != null)
					{
						onTouchHandled();
					}
				}
				else if (onTouchUnhandled != null)
				{
					onTouchUnhandled();
				}
			}
			else if (pointerEvent.pointerDrag != null && pointerEvent.dragging && flag2)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, pointerEvent, ExecuteEvents.dropHandler);
			}
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
			if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.dragging = false;
			pointerEvent.pointerDrag = null;
			if (pointerEvent.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.pointerDrag = null;
			ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
			pointerEvent.pointerEnter = null;
		}

		protected bool SendSubmitEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			if (base.input.GetButtonDown(m_SubmitButton))
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
			}
			if (base.input.GetButtonDown(m_CancelButton))
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			}
			return baseEventData.used;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = base.input.GetAxisRaw(m_HorizontalAxis);
			zero.y = base.input.GetAxisRaw(m_VerticalAxis);
			if (base.input.GetButtonDown(m_HorizontalAxis))
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (base.input.GetButtonDown(m_VerticalAxis))
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		protected bool SendMoveEventToSelectedObject()
		{
			float unscaledTime = Time.unscaledTime;
			Vector2 rawMoveVector = GetRawMoveVector();
			if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
			{
				m_ConsecutiveMoveCount = 0;
				return false;
			}
			bool flag = base.input.GetButtonDown(m_HorizontalAxis) || base.input.GetButtonDown(m_VerticalAxis);
			bool flag2 = Vector2.Dot(rawMoveVector, m_LastMoveVector) > 0f;
			if (!flag)
			{
				flag = ((!flag2 || m_ConsecutiveMoveCount != 1) ? (unscaledTime > m_PrevActionTime + 1f / m_InputActionsPerSecond) : (unscaledTime > m_PrevActionTime + m_RepeatDelay));
			}
			if (!flag)
			{
				return false;
			}
			AxisEventData axisEventData = GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
			if (axisEventData.moveDir != MoveDirection.None)
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
				if (!flag2)
				{
					m_ConsecutiveMoveCount = 0;
				}
				m_ConsecutiveMoveCount++;
				m_PrevActionTime = unscaledTime;
				m_LastMoveVector = rawMoveVector;
			}
			else
			{
				m_ConsecutiveMoveCount = 0;
			}
			return axisEventData.used;
		}

		protected void ProcessMouseEvent()
		{
			ProcessMouseEvent(0);
		}

		protected virtual bool ForceAutoSelect()
		{
			return false;
		}

		protected void ProcessMouseEvent(int id)
		{
			MouseState mousePointerEventData = GetMousePointerEventData(id);
			MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			if (ForceAutoSelect())
			{
				base.eventSystem.SetSelectedGameObject(eventData.buttonData.pointerCurrentRaycast.gameObject, eventData.buttonData);
			}
			ProcessMousePress(eventData);
			ProcessMove(eventData.buttonData);
			ProcessDrag(eventData.buttonData);
			ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				ExecuteEvents.ExecuteHierarchy(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		protected void ProcessMousePress(MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(gameObject);
				bool flag = true;
				if (eventHandler == null)
				{
					eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
					flag = false;
				}
				if (IsClickable(eventHandler, data, out bool _) && flag)
				{
					ExecuteEvents.Execute(eventHandler, buttonData, ExecuteEvents.pointerDownHandler);
				}
				float unscaledTime = Time.unscaledTime;
				if (eventHandler == buttonData.lastPress)
				{
					if (unscaledTime - buttonData.clickTime < 0.3f)
					{
						int num = ++buttonData.clickCount;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = eventHandler;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (IsClickable(eventHandler2, data, out bool _))
				{
					buttonData.pointerDrag = eventHandler2;
					if (buttonData.pointerDrag != null)
					{
						ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
					}
				}
			}
			if (!data.ReleasedThisFrame())
			{
				return;
			}
			bool isMainClickableGO3;
			bool flag2 = IsClickable(buttonData.pointerPress, data, out isMainClickableGO3);
			if (flag2)
			{
				ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
			}
			GameObject eventHandler3 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if (buttonData.pointerPress == eventHandler3 && buttonData.eligibleForClick)
			{
				if (flag2)
				{
					ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
					if (isMainClickableGO3 && onTouchHandled != null)
					{
						onTouchHandled();
					}
				}
				else if (onTouchUnhandled != null)
				{
					onTouchUnhandled();
				}
			}
			else if (buttonData.pointerDrag != null && buttonData.dragging && flag2)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.dropHandler);
			}
			buttonData.eligibleForClick = false;
			buttonData.pointerPress = null;
			buttonData.rawPointerPress = null;
			if (buttonData.pointerDrag != null && buttonData.dragging)
			{
				ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
			}
			buttonData.dragging = false;
			buttonData.pointerDrag = null;
			if (gameObject != buttonData.pointerEnter)
			{
				HandlePointerExitAndEnter(buttonData, null);
				HandlePointerExitAndEnter(buttonData, gameObject);
			}
		}

		private bool IsClickable(GameObject targetGO, MouseButtonEventData data, out bool isMainClickableGO)
		{
			return IsClickable(targetGO, data.PressedThisFrame(), data.ReleasedThisFrame(), out isMainClickableGO);
		}

		private bool IsClickable(GameObject targetGO, bool pressed, bool released, out bool isMainClickableGO)
		{
			bool result = false;
			isMainClickableGO = false;
			if (targetGO != null && (pressed | released))
			{
				if (clickableObject == null || targetGO.name == clickableObject)
				{
					isMainClickableGO = true;
					result = true;
				}
				else if (clickableWithoutCallbackObjects != null && Array.IndexOf(clickableWithoutCallbackObjects, targetGO.name) >= 0)
				{
					isMainClickableGO = false;
					result = true;
				}
				else
				{
					result = false;
					isMainClickableGO = false;
				}
			}
			return result;
		}
	}

