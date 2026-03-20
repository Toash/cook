using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerFlashlight : MonoBehaviour
{
    public InputActionReference ToggleFlashlightAction;
    public Light Flashlight;


    void Update()
    {
        if (ToggleFlashlightAction.action.WasPressedThisFrame())
        {
            Toggle();
        }
    }
    public void Toggle()
    {
        Flashlight.enabled = !Flashlight.enabled;
    }

}