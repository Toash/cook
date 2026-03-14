using System.Collections;
using TMPro;
using UnityEngine;


public class OrderCompletedUI : MonoBehaviour
{
    public float ShowTime = 6;
    public GameObject Root;
    public TMP_Text Profit;


    void Start()
    {
        Root.gameObject.SetActive(false);
        OrderManager.I.NPCEvaluatedOrder += Show;
    }
    void OnDestroy()
    {
        OrderManager.I.NPCEvaluatedOrder -= Show;
    }


    void Show(OrderSubmissionResult result)
    {
        Profit.text = result.Payout.ToString();
        StartCoroutine(ShowRoutine());

    }


    IEnumerator ShowRoutine()
    {
        Root.gameObject.SetActive(true);
        yield return new WaitForSeconds(ShowTime);
        Root.gameObject.SetActive(false);

    }


}