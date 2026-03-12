using UnityEngine;

public class PlayerCrosshair : MonoBehaviour
{
    [Tooltip("Should not be the same gameobject that this monobehaviour is located in.")]
    public GameObject Crosshair;
    public PlayerController PlayerController;



    void OnEnable()
    {
        PlayerController.PopupShow += PopupShow;
        PlayerController.PopupHide += PopupHide;

    }
    void OnDisable()
    {
        PlayerController.PopupShow -= PopupShow;
        PlayerController.PopupHide -= PopupHide;

    }

    public void Show()
    {
        Crosshair.SetActive(true);
    }
    public void Hide()
    {
        Crosshair.SetActive(false);
    }

    void PopupShow(PopupType _)
    {
        Show();

    }
    void PopupHide(PopupType _)
    {
        Hide();

    }

}