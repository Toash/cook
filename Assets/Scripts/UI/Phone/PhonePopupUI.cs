using System.Collections;
using DG.Tweening;
using UnityEngine;
public class PhonePopupUI : PopupUI
{
    public float FadeInOutTime = .6f;
    public float OutOfScreenY = -850;

    protected override IEnumerator BeforePopupHideActive()
    {
        var tween = DOTween.To(() => Popup.transform.localPosition, vec => Popup.transform.localPosition = vec, new Vector3(0, OutOfScreenY, 0), FadeInOutTime);
        yield return tween.WaitForCompletion();
        Debug.Log("Done hide tween");
    }
    protected override void BeforePopupShowActive()
    {
        Popup.transform.localPosition = new Vector3(0, OutOfScreenY, 0);

    }
    protected override void AfterPopupShowActive()
    {
        DOTween.To(() => Popup.transform.localPosition, vec => Popup.transform.localPosition = vec, Vector3.zero, FadeInOutTime);

    }



}