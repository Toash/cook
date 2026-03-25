using TMPro;
using UnityEngine;

public class DisplaySingleCondiment : MonoBehaviour
{
    public TMP_Text text;


    public void Populate(CondimentData condiment)
    {
        text.text = "- " + condiment.Name;
    }

}