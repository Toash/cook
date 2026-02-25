using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text text;


    void Update()
    {
        text.text = MoneyManager.I.GetMoney().ToString() + "$";
    }
}