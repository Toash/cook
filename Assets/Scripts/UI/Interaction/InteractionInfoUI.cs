using TMPro;
using UnityEngine;
public class InteractionInfoUI : MonoBehaviour
{
    public TMP_Text Keybind;
    public TMP_Text Description;

    public void Populate(InteractInfo info)
    {
        Keybind.text = info.Type.ToString();
        Description.text = info.Description;
    }

}
