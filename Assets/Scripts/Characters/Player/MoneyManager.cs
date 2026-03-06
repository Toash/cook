using System;
using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager I;


    private float money;



    public float GetMoney()
    {
        return money;
    }

    public void AddMoney(float money)
    {
        this.money = Math.Max(this.money + money, 0);
    }


    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }


}
