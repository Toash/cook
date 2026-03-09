using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a single grocery store item.
/// </summary>
public class DisplaySingleGroceryStoreItemUI : MonoBehaviour
{
    public TMP_Text Name;
    public RawImage Image;
    public TMP_Text Amount;
    public TMP_Text Price;
    public Button BuyButton;

    private float price;


    void Update()
    {
        BuyButton.interactable = MoneyManager.I.HasMoney(price);

    }

    public void Populate(GroceryStore store, GroceryStoreItem item)
    {
        this.price = item.Price;

        Name.text = item.Ingredient.Name;
        Image.texture = item.Ingredient.Image;
        Amount.text = "x " + item.Amount.ToString();
        Price.text = "$ " + item.Price.ToString();
        BuyButton.onClick.AddListener(() => { store.BuyItem(item); });
    }









}