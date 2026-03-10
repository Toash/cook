using System;
using IngameDebugConsole;
using Sirenix.OdinInspector;
using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager I;


    private float money;

    private event Action MoneyChanged;


    public void Start()
    {
        ChangeMoney(1000);

    }

    public float GetMoney()
    {
        return money;
    }

    [ConsoleMethod("AddMoney", "Add money.")]
    public static void StaticChangeMoney(float money)
    {
        if (I == null) return;
        I.ChangeMoney(money);

    }
    public void ChangeMoney(float money)
    {
        I.money = Math.Max(I.money + money, 0);
        MoneyChanged?.Invoke();
    }


    public bool TryTake(float amount)
    {
        if (amount > money) return false;

        ChangeMoney(-amount);

        return true;
    }

    public bool HasMoney(float amount)
    {
        return amount <= money;
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
