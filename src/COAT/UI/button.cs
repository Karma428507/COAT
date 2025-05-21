using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MultiClickHandler : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLeftClick;
    public UnityEvent onRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftClick?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            onRightClick?.Invoke();
    }
}
