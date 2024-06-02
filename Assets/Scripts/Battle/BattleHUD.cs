using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider hpSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.GetName();
        levelText.text = "lvl " + unit.GetLevel();
        hpSlider.maxValue = unit.GetMaxHP();
        hpSlider.value = unit.GetCurrentHP();
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
