using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCam;
    private float _raycastDistance = 100f;
    private Vector2 inputValue;
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        inputValue = context.ReadValue<Vector2>();
        _= OnPointerClick(inputValue);
    }
    private async UniTaskVoid OnPointerClick(Vector2 pointValue)
    {
        await UniTask.NextFrame();
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 pointerPosition = pointValue;
        Ray ray = mainCam.ScreenPointToRay(pointerPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, interactableLayer))
        {
            hit.collider.gameObject.GetComponent<IClickable>()?.OnClick();
        }
        else
        {
            UiManager.UiManagerInstance.HideBuildingPanel();
        }
    }
}