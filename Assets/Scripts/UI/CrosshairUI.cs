using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public RawImage Crosshair;

    public PlayerController playerController;

    void OnEnable()
    {
        playerController.PopupShow += OnPopupShow;
        playerController.PopupHide += OnPopupHide;
    }
    void OnDisable()
    {
        playerController.PopupShow -= OnPopupShow;
        playerController.PopupHide -= OnPopupHide;
    }
    void OnPopupShow(PopupType type)
    {
        Crosshair.transform.gameObject.SetActive(false);

    }
    void OnPopupHide(PopupType type)
    {
        Crosshair.transform.gameObject.SetActive(true);

    }





}