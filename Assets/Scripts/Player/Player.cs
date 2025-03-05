using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //ÃÑ°ý 
    public PlayerController controller;
    public PlayerCondition playerCondition;
    private void Awake()
    {
        CharaterManager.Instance.Player = this;
        controller= GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
    }

}
