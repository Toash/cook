using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SupplyItemUI : MonoBehaviour
{
    [Header("Data")]
    public HoldableData Item;

    [Header("Refs")]
    public SupplyOrderController Controller;
    public RawImage ItemImage;
    public TMP_Text NameText;
    public TMP_Text AmountText;

    [Header("Buttons")]
    public Button AddButton;
    public Button RemoveButton;

    private void Awake()
    {
        AddButton.onClick.AddListener(AddOne);
        RemoveButton.onClick.AddListener(RemoveOne);
    }



    private void Start()
    {
        Refresh();
        Controller.CartChanged += OnCartChanged;
    }

    private void OnDestroy()
    {
        AddButton.onClick.RemoveListener(AddOne);
        RemoveButton.onClick.RemoveListener(RemoveOne);
        Controller.CartChanged -= OnCartChanged;
    }

    public void AddOne()
    {
        Controller.AddItem(Item, 1);
    }

    public void RemoveOne()
    {
        Controller.RemoveItem(Item, 1);
    }

    private void OnCartChanged(SupplyOrder order)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (ItemImage != null && Item.Image != null)
            ItemImage.texture = Item.Image;

        NameText.text = Item.Name;

        int amount = Controller.GetAmount(Item);
        AmountText.text = amount.ToString();

        if (RemoveButton != null)
            RemoveButton.interactable = amount > 0;
    }
}