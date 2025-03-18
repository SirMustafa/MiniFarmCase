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

    public async void OnLeftMouseBtn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        await UniTask.Yield(PlayerLoopTiming.Update);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePosition);

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