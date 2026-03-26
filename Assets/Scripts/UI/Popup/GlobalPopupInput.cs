using UnityEngine;
using UnityEngine.InputSystem;
public class GlobalPopupInput : MonoBehaviour
{
    public PlayerController PlayerController;
    public InputActionReference CloseAction;

    void Update()
    {
        if (PlayerController == null) return;
        if (CloseAction == null) return;
        if (PlayerController.CurrentControlMode != PlayerMode.InPopup) return;

        if (CloseAction.action.WasPressedThisFrame())
        {
            PlayerController.CloseCurrentPopup();
        }
    }
}