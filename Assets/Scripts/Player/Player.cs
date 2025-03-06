using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //ÃÑ°ý 
    public PlayerController controller;
    public PlayerCondition playerCondition;
    public Equipment equip;

    public ItemData itemData;
    public Action additem;

    public Transform dropPosition;
    private void Awake()
    {
        CharaterManager.Instance.Player = this;
        controller= GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
        equip = GetComponent<Equipment>();
    }

}
