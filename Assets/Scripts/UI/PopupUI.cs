using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Subscribes to the playercontroller popup events to activate and deactivate a gameobject based on a popuptype
/// </summary>
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
    void Start()
    {
        Popup.SetActive(false);
    }
    void OnPopupShow(PopupType type)
    {
        if (this.Type != type) return;

        BeforePopupShowActive();
        Popup.SetActive(true);
        AfterPopupShowActive();


    }
    void OnPopupHide(PopupType type)
    {
        if (this.Type != type) return;
        StartCoroutine(HideRoutine());

    }
    protected virtual void BeforePopupShowActive()
    {

    }
    protected virtual void AfterPopupShowActive()
    {

    }

    protected virtual IEnumerator BeforePopupHideActive()
    {
        yield break;

    }

    IEnumerator HideRoutine()
    {
        yield return StartCoroutine(BeforePopupHideActive());
        Popup.SetActive(false);
    }

}