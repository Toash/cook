using System;
using IngameDebugConsole;
using Sirenix.OdinInspector;
using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager I;


    private float money;



    public float GetMoney()
    {
        return money;
    }

    [ConsoleMethod("AddMoney", "Add money.")]
    public static void StaticAddMoney(float money)
    {
        if (I == null) return;
        I.AddMoney(money);

    }
    public void AddMoney(float money)
    {
        I.money = Math.Max(I.money + money, 0);
    }


    public bool TryTake(float amount)
    {
        if (amount > money) return false;

        this.money -= amount;

        return true;
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
