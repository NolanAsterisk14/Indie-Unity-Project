using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BtnLabel { ATTACK, SKILL, GUARD, FLEE }

public class Actions : MonoBehaviour
{
    //Main button set
    [SerializeField] private GameObject actionsPanel; //Inst init
    [SerializeField] private Button[] buttons; //Ins init
    //Cancel button panel
    [SerializeField] private GameObject cancelPanel; //Ins init
    [SerializeField] private Button cancelButton; //Ins init

    [SerializeField] private bool canFlee; //Should start as true

    public void SetActionsInteract(bool value)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == (int)BtnLabel.FLEE && canFlee == false) //If flee button selected and unable to flee, force disable it.
            {
                buttons[i].interactable = false;
            }
            else //Otherwise, set it to the passed value
            {
                buttons[i].interactable = value;
            }
        }
    }

    public void SetActionsActive(bool value)
    {
        actionsPanel.SetActive(value);
    }

    public void SetCanFlee(bool value)
    {
        canFlee = value;
    }

    public void SetCancelInteract(bool value)
    {
        if (cancelButton != null)
        {
            cancelButton.interactable = value;
        }
    }

    public void SetCancelActive(bool value)
    {
        cancelPanel.SetActive(value);
    }
}
