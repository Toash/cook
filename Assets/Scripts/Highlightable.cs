using UnityEngine;

public class Highlightable : MonoBehaviour
{
    public float Width = 8;
    private Outline outline;
    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = Width;
        SetHighlight(false);

    }

    public void SetHighlight(bool highlight)
    {
        outline.enabled = highlight;
    }

}