using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUIBehaviour : MonoBehaviour
{
    public UnityEvent buttonEvent;
    public InventoryData inventoryDataObj;
    public InventoryUIButtonBehaviour inventoryUIPrefab;

    private void Start()
    {
        buttonEvent.Invoke();
    }

    private void AddItemsToUI<T>(List<T> items)
    {
        foreach (var item in items)
        {
            var element = Instantiate(inventoryUIPrefab.gameObject, transform);
            ConfigureElement(element, item);
        }
    }

    private void ConfigureElement<T>(GameObject element, T item)
    {
        if (item is IInventoryItem inventoryItem)
        {
            var elementData = element.GetComponent<InventoryUIButtonBehaviour>();
            if (elementData == null) return;

            elementData.ButtonObj.image.sprite = inventoryItem.PreviewArt;
            elementData.Label.text = inventoryItem.Name;
            elementData.ButtonObj.interactable = !inventoryItem.Used;
            elementData.InventoryItemObj = inventoryItem as InventoryItem;
            if(inventoryItem.GameActionObj != null)
                elementData.ButtonObj.onClick.AddListener(inventoryItem.Raise);
            else
            {
                elementData.ButtonObj.interactable = false;
            }
        }

        if (item is not IStoreItem storeItem) return;
        {
            var elementData = element.GetComponent<StoreUIButtonBehaviour>();
            if (elementData == null) return;
            elementData.ButtonObj.image.sprite = storeItem.PreviewArt;
            elementData.Label.text = storeItem.Name;
            elementData.ButtonObj.interactable = !storeItem.Purchased;
            elementData.StoreItemObj = storeItem;
            elementData.ToggleObj.isOn = storeItem.Purchased;
            elementData.PriceLabel.text = $"${storeItem.Price}";
            elementData.cash = inventoryDataObj.cash;
        }
    }

    public void AddAllInventoryItemsToUI()
    {
        AddItemsToUI(inventoryDataObj.inventoryDataObjList);
    }

    public void AddAllStoreInventoryItemsToUI()
    {
        AddItemsToUI(inventoryDataObj.storeDataObjList);
    }
    
    public void AddAllInventoryItemsPrefabsToScene()
    {
        var i = 0;
        foreach (var item in inventoryDataObj.inventoryDataObjList)
        {
            i++;
            if (item.GameActionObj == null) return;
            if (item.GameArt == null) return;
            var element = Instantiate(item.GameArt, transform);
            var elementData = element.GetComponent<TriggerEventsBehaviour>();  
            elementData.triggerEnterAction = item.GameActionObj;
            elementData.gameObject.transform.transform.position = Vector3.left*i*-10;
        }
    }
    
    public void AddPurchasedInventoryItemsPrefabsToScene()
    {
        var i = 0;
        foreach (var item in inventoryDataObj.storeDataObjList)
        {
            i++;
            if (!item.Purchased) continue;
            if (item is not IInventoryItem storeItem) continue;
            if (storeItem.GameActionObj == null) return;
            if (storeItem.GameArt == null) return;
            var element = Instantiate(storeItem.GameArt, transform);
            var elementData = element.GetComponent<TriggerEventsBehaviour>();  
            elementData.triggerEnterAction = storeItem.GameActionObj;
            elementData.gameObject.transform.transform.position = Vector3.left*i*-10;
        }
    }
}