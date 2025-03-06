using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipeButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition conditoin;

    int curEquopindex;

    ItemData selectItem;
    int selectedItemIndex = 0;


    private void Start()
    {
        controller = CharaterManager.Instance.Player.controller;
        conditoin = CharaterManager.Instance.Player.playerCondition;
        dropPosition = CharaterManager.Instance.Player.dropPosition;


        controller.inventory += Toggle;
        CharaterManager.Instance.Player.additem += AddItem;
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] =slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index =i;
            slots[i].inventory = this;

        }

        ClearSelectedItemWindow();
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatValue.text = string.Empty;
        selectedStatName.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipeButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharaterManager.Instance.Player.itemData;

        //아이템이 중복 가능한지 canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharaterManager.Instance.Player.itemData = null;
                return;
            }
        }
        // 비어있는 슬롯 가져온다.
        ItemSlot emptySlot = GetEmptSlot();
        //있다면 
        if (emptySlot !=null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharaterManager.Instance.Player.itemData = null;
            return;
        }
        //없다면 
        ThrowItem(data);

        CharaterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item !=null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item ==data && slots[i].quantity <data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item ==null)
            {
                return slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab,dropPosition.position,Quaternion.Euler(Vector3.one*Random.value*360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectItem.displayName;
        selectedItemDescription.text = selectItem.description;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectItem.consumables.Length; i++)
        {
            selectedStatName.text += selectItem.consumables[i].type.ToString() +"\n";
            selectedStatValue.text += selectItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectItem.type ==ItemType.Consumable);
        equipButton.SetActive(selectItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipeButton.SetActive(selectItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void ONUseButton()
    {
        if (selectItem.type ==ItemType.Consumable)
        {
            for (int i = 0; i < selectItem.consumables.Length; i++)
            {
                switch (selectItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        conditoin.Heal(selectItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        conditoin.Eat(selectItem.consumables[i].value);
                        break;
                    default:
                        break;
                }
            }
            RemoveSelectItem();
        }
    }
    public void ONDropButton()
    {
        ThrowItem(selectItem);
        RemoveSelectItem();
    }

    void RemoveSelectItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <=0)
        {
            selectItem = null;
            slots[selectedItemIndex].item= null;
            selectedItemIndex= -1;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }


    public void OnEquipButton()
    {
        if (slots[curEquopindex].equipped)
        {
            UnEquip(curEquopindex);
        }

        slots[selectedItemIndex].equipped= true;
        curEquopindex= selectedItemIndex;
        CharaterManager.Instance.Player.equip.EquipNew(selectItem);
        UpdateUI();
        SelectItem(selectedItemIndex);

    }

    void UnEquip(int index)
    {
        slots[index].equipped= false;
        CharaterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIndex ==index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
