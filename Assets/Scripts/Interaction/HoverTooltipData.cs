using System;
using UnityEngine;

[System.Serializable]
public class HoverTooltipData
{
    public Transform Parent;
    public Vector3 Offset = new Vector3(0, 8, 0);


    private string staticText;
    private Func<string> getText;

    public string GetText
    {
        get
        {
            if (getText != null)
            {
                return getText();
            }
            return staticText;
        }
    }

    // Static text constructor (keep this for simple cases)
    public HoverTooltipData(Transform parent, string text)
    {
        Parent = parent;
        staticText = text;
    }

    public HoverTooltipData(Transform parent, string text, Vector2 offset)
    {
        Parent = parent;
        staticText = text;
        Offset = offset;
    }

    // Dynamic text constructor
    public HoverTooltipData(Transform parent, Func<string> getText)
    {
        Parent = parent;
        this.getText = getText;
    }

    public HoverTooltipData(Transform parent, Func<string> getText, Vector2 offset)
    {
        Parent = parent;
        this.getText = getText;
        Offset = offset;
    }
}