using TMPro;
using UnityEngine;

public class SupplyCartItemUI : MonoBehaviour
{
    public TMP_Text Text;

    public void Init(HoldableData item, int amount)
    {
        if (item == null || Text == null) return;

        Text.text = $"{amount} x {item.Name}";
    }
}