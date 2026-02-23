using UnityEngine;

public class LookDown : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
