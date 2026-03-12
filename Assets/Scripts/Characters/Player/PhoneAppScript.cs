using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles opening and closing apps
/// 
/// Apps are referred to by their child indices based off of some parent object
/// </summary>
public class PhoneAppScript : MonoBehaviour
{

    [Tooltip("Object for the homepage")]
    public Transform HomePage;
    [Tooltip("Parent object that contains all of the apps")]
    public Transform AppParent;

    public void EnableApp(int index)
    {
        HomePage.gameObject.SetActive(false);
        for (int i = 0; i < AppParent.childCount - 1; i++)
        {
            Transform child = AppParent.GetChild(i);
            if (i == index)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

    }

    public void Home()
    {
        for (int i = 0; i < AppParent.childCount - 1; i++)
        {
            Transform child = AppParent.GetChild(i);
            child.gameObject.SetActive(false);
        }

        HomePage.gameObject.SetActive(true);

    }
}
