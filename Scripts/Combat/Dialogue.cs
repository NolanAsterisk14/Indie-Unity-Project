using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;

    public void SetText(string text)
    {
        dialogueText.text = text;
    }
}
