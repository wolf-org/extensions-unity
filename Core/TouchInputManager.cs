using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VirtueSky.Inspector;

namespace VirtueSky.TouchInput
{
    [HideMonoScript]
    public class TouchInputManager : MonoBehaviour
    {
        public static event Action<Touch> InputEventTouchBegin;
        public static event Action<Touch> InputEventTouchMove;
        public static event Action<Touch> InputEventTouchStationary;
        public static event Action<Touch> InputEventTouchEnd;
        public static event Action<Touch> InputEventTouchCancel;

        public static event Action<Vector3> InputEventMouseDown;
        public static event Action<Vector3> InputEventMouseUpdate;
        public static event Action<Vector3> InputEventMouseUp;

        [SerializeField, Tooltip("Use mouse in editor")]
        private bool useMouse = false;

        [SerializeField, Tooltip("Disable when touching UI")]
        private bool ignoreUI = true;

        [ShowIf(nameof(IsPlaying)), SerializeField]
        private Vector3 touchPosition;

        private bool IsPlaying => Application.isPlaying;
        private bool _mouseDown;
        private bool _mouseUpdate;

        private void Update()
        {
            if (ignoreUI && EventSystem.current != null)
            {
                if (Input.touchCount > 0 && EventSystem.current.currentSelectedGameObject == null)
                {
                    HandleTouch();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    HandleTouch();
                }
            }
#if UNITY_EDITOR
            if (useMouse)
            {
                if (ignoreUI && EventSystem.current != null)
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        HandleMouse();
                    }
                }
                else
                {
                    HandleMouse();
                }
            }

#endif
        }

        void HandleTouch()
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    InputEventTouchBegin?.Invoke(touch);

                    break;
                case TouchPhase.Moved:
                    InputEventTouchMove?.Invoke(touch);

                    break;
                case TouchPhase.Stationary:
                    InputEventTouchStationary?.Invoke(touch);

                    break;
                case TouchPhase.Ended:
                    InputEventTouchEnd?.Invoke(touch);

                    break;
                case TouchPhase.Canceled:
                    InputEventTouchCancel?.Invoke(touch);

                    break;
            }

            touchPosition = touch.position;
        }

        void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_mouseDown)
                {
                    _mouseDown = true;
                    _mouseUpdate = true;
                    InputEventMouseDown?.Invoke(Input.mousePosition);
                    touchPosition = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
                _mouseUpdate = false;
                InputEventMouseUp?.Invoke(Input.mousePosition);
                touchPosition = Input.mousePosition;
            }

            if (_mouseDown && _mouseUpdate)
            {
                InputEventMouseUpdate?.Invoke(Input.mousePosition);
                touchPosition = Input.mousePosition;
            }
        }
    }
}