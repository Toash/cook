using UnityEngine;

[RequireComponent(typeof(Snapper))]
public class Trashcan : MonoBehaviour
{
    private Snapper snapper;

    void Awake()
    {
        snapper = GetComponent<Snapper>();
    }

    void OnEnable()
    {
        snapper.OnChildSnapped += OnSnapped;
    }
    void OnDisable()
    {
        snapper.OnChildSnapped -= OnSnapped;
    }


    void OnSnapped(Snapper snapper)
    {
        Destroy(snapper.gameObject);

    }

}