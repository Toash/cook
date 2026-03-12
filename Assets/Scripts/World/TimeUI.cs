using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    public TMP_Text Text;

    void Update()
    {
        Text.text = TimeManager.I.Get24HourTime();
    }

}