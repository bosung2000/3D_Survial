using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondion : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    // Start is called before the first frame update
    void Start()
    {
        CharaterManager.Instance.Player.playerCondition.uICondion = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
