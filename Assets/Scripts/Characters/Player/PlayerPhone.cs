using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhone : MonoBehaviour
{
    public Player Player;
    public InputActionReference OpenClose;

    bool opened = false;


    void Update()
    {
        if (OpenClose.action.WasPressedThisFrame())
        {
            TogglePhone();
        }

    }


    void TogglePhone()
    {
        opened = !opened;
        if (opened)
        {
            _OnOpen();

        }
        else
        {
            _OnClose();

        }


    }

    void _OnOpen()
    {
        // PhoneUIRoot.SetActive(true);
        Player.Controller.ShowPopup(PopupType.Phone);


    }
    void _OnClose()
    {
        // PhoneUIRoot.SetActive(false);
        Player.Controller.CloseCurrentPopup();

    }

}