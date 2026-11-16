using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Saga.Game
{
    // Left/right click and hover callbacks for code-built UI.
    public class UiHooks : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action OnLeft, OnRight, OnEnter, OnExit;

        public void OnPointerClick(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Left) OnLeft?.Invoke();
            else if (e.button == PointerEventData.InputButton.Right) OnRight?.Invoke();
        }

        public void OnPointerEnter(PointerEventData e) => OnEnter?.Invoke();
        public void OnPointerExit(PointerEventData e) => OnExit?.Invoke();
    }
}
