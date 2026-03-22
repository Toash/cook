using UnityEngine;

public class CookableVisual : MonoBehaviour
{
    [SerializeField] private Cookable cookable;
    [SerializeField] private Renderer targetRenderer;

    private MaterialPropertyBlock mpb;
    private static readonly int CookAmountId = Shader.PropertyToID("_CookAmount");

    void Awake()
    {
        if (cookable == null)
            cookable = GetComponent<Cookable>();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        mpb = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (cookable == null || targetRenderer == null)
            return;

        //update uniform
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(CookAmountId, cookable.CookNormalized);
        targetRenderer.SetPropertyBlock(mpb);
    }
}