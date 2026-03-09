using UnityEngine;

/// <summary>
/// Data used for world space hover tooltips
/// </summary>
[System.Serializable]
public class HoverTooltipData
{
    public Transform Parent;
    public string Text;
    public Vector2 Offset = new Vector2(0, -100);

    public HoverTooltipData(Transform parent, string text)
    {
        this.Parent = parent;
        Text = text;
    }
    public HoverTooltipData(Transform parent, string text, Vector2 offset)
    {
        this.Parent = parent;
        Text = text;
        Offset = offset;
    }


}