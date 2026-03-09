using UnityEngine;
public class PopupUI : MonoBehaviour
{
    public PopupType Type;

    [Tooltip("The reference to the popup that shows and hides. It should not contain this script")]
    public GameObject Popup;
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
        if (this.Type != type) return;
        Popup.SetActive(true);

    }
    void OnPopupHide(PopupType type)
    {
        if (this.Type != type) return;
        Popup.SetActive(false);

    }

}