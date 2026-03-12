using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Contains functinoality for npcs to say things in the world.
/// </summary>
public class NPCWorldDialogue : MonoBehaviour
{
    [Tooltip("The object that is actually displayed when the npc says something. Since it gets enabled and disabled, should not contain functionality. ")]
    public GameObject TextObject;
    public TMP_Text Text;
    float timeToHide = 0;
    void Start()
    {
        Say("Hello!!");
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        if (Time.time >= timeToHide)
        {
            TextObject.SetActive(false);
        }
        else
        {
            TextObject.SetActive(true);

        }

    }
    public void Say(string message, float duration = 6)
    {
        Text.text = message;
        timeToHide = Time.time + duration;
    }





}