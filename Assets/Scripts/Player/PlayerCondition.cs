using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damge);
}

public class PlayerCondition : MonoBehaviour,IDamageable
{
    public UICondion uICondion;

    Condition health { get { return uICondion.health; } }
    Condition hunger { get { return uICondion.hunger; } }
    Condition stanima { get { return uICondion.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;
    void Update()
    {
        hunger.Subtract(hunger.passiveValue *Time.deltaTime);
        stanima.Add(stanima.passiveValue *Time.deltaTime);

        if (hunger.curValue <=0f)
        {
            health.Subtract(noHungerHealthDecay*Time.deltaTime);
        }
        if (health.curValue ==0f)
        {
            Die();
        }
    }
    public void Heal(float amout)
    {
        health.Add(amout);
    }
    public void Eat(float amout)
    {
        hunger.Add(amout);
    }
    private void Die()
    {
        Debug.Log("ав╬З╢ы");
    }

    public void TakePhysicalDamage(int dmamge)
    {
        health.Subtract(dmamge);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (stanima.curValue -amount <0f)
        {
            return false;
        }
        stanima.Subtract(amount);
        return true;
    }
}
