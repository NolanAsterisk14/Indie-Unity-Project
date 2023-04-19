using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isPlayerUnit;
    public string unitName;
    public int unitLevel;

    public int damage;
    public int speed;

    public int currentHealth;
    public int maxHealth;
    public int currentEssence;
    public int maxEssence;

    public static int CompareUnitSpeed(GameObject x, GameObject y)
    {
        int speedX = x.GetComponent<Unit>().speed;
        int speedY = y.GetComponent<Unit>().speed;

        return speedX.CompareTo(speedY);
    }

    void Awake()
    {
        this.gameObject.tag = isPlayerUnit == true ? "PlayerUnit" : "EnemyUnit";
    }

    public bool TakeDamage(int damage, float fadeTime, Del healthHandler)
    {
        //currentHealth = Math.Max((currentHealth - damage), 0); //Take damage but don't fall below zero
        StartCoroutine(FadeDown(currentHealth, damage, fadeTime, healthHandler));
        if (currentHealth == 0) //If health is at 0, unit has died
        {
            return true;
        }
        else //Otherwise, they're still alive
        {
            return false;
        }
    }

    IEnumerator FadeDown(int currentHealth, int damage, float fadeTime, Del healthHandler)
    {
        yield return null;
    }
}
