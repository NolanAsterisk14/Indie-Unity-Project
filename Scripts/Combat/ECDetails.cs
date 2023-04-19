using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate void Del(Unit unit);

public class ECDetails : MonoBehaviour
{
    [SerializeField] private GameObject[] barObjs; //Init in inspector for now
    [SerializeField] private List<DetailBar> bars = new List<DetailBar>();

    public static ECDetails Instance { get; private set; } //Instance was needed to pass a delegate
    public Del healthHandler;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < barObjs.Length; i++)    //Iterate through each bar object to populate a bar class instance.
        {
            DetailBar bar = new DetailBar();        //Store all elements on the object that need updates.
            bar.nameText = barObjs[i].transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
            bar.levelText = barObjs[i].transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>();
            bar.healthSlider = barObjs[i].transform.Find("Health_Slider").gameObject.GetComponent<Slider>();
            bar.healthText = barObjs[i].transform.Find("Health_Value").gameObject.GetComponent<TextMeshProUGUI>();
            bar.essenceSlider = barObjs[i].transform.Find("Essence_Slider").gameObject.GetComponent<Slider>();
            bar.essenceText = barObjs[i].transform.Find("Essence_Value").gameObject.GetComponent<TextMeshProUGUI>();
            bars.Add(bar);                          //Add bar instance to dictionary.
        }

        healthHandler = Instance.SetHealth;
    }

    public void SetDetails(List<Unit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (barObjs[i].activeSelf == false)         //Set the number of bars active based on number of player units.
            { 
                barObjs[i].SetActive(true); 
            }

            bars[i].barUnit = units[i];
            bars[i].nameText.text = units[i].unitName;  //Then assign values to UI elements.
            bars[i].levelText.text = "Lv." + units[i].unitLevel.ToString();
            bars[i].healthSlider.maxValue = (float)units[i].maxHealth;
            bars[i].healthSlider.value = (float)units[i].currentHealth;
            bars[i].healthText.text = units[i].currentHealth.ToString() + " / " + units[i].maxHealth.ToString();
            bars[i].essenceSlider.maxValue = (float)units[i].maxEssence;
            bars[i].essenceSlider.value = (float)units[i].currentEssence;
            bars[i].essenceText.text = units[i].currentEssence.ToString() + " / " + units[i].maxEssence.ToString();
        }
    }

    public void SetDetails(Unit unit)
    {
        int index = -1;
        for (int i = 0; i < bars.Count; i++)                                        //Get the index based on which unit matches in the bar object
        {
            index = bars[i].barUnit == unit ? i : -1;
        }

        if (barObjs[index].activeSelf == false)                                     //Make sure the bar is active.
        {
            barObjs[index].SetActive(true);
        }

        bars[index].nameText.text = unit.unitName;                                  //Then assign values to UI elements.
        bars[index].levelText.text = "Lv." + unit.unitLevel.ToString();
        bars[index].healthSlider.maxValue = (float)unit.maxHealth;
        bars[index].healthSlider.value = (float)unit.currentHealth;
        bars[index].healthText.text = unit.currentHealth.ToString() + " / " + unit.maxHealth.ToString();
        bars[index].essenceSlider.maxValue = (float)unit.maxEssence;
        bars[index].essenceSlider.value = (float)unit.currentEssence;
        bars[index].essenceText.text = unit.currentEssence.ToString() + " / " + unit.maxEssence.ToString();
    }

    public void SetHealth(Unit unit)
    {
        int index = -1;
        for (int i = 0; i < bars.Count; i++)                                        //Get the index based on which unit matches in the bar object
        {
            index = bars[i].barUnit == unit ? i : -1;
        }

        if (barObjs[index].activeSelf == false)                                     //Make sure the bar is active.
        {
            barObjs[index].SetActive(true);
        }

        bars[index].healthSlider.value = (float)unit.currentHealth;                 //Then assign values to UI elements
        bars[index].healthText.text = unit.currentHealth.ToString() + " / " + unit.maxHealth.ToString();
    }

}


public class DetailBar
{
    public Unit barUnit;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Slider essenceSlider;
    public TextMeshProUGUI essenceText;
}
