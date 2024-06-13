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
    [SerializeField] private float lerpSpeed = 0.1f;

    private float time;
    public int currentHealth;

    /// <summary>
    /// Sets the Hud for a particular Unit
    /// </summary>
    /// <param name="unit">Detailed data related to the unit</param>
    public void SetHUD(Unit unit)
    {
        nameText.text = unit.GetName();
        levelText.text = "lvl " + unit.GetLevel();
        hpSlider.maxValue = unit.GetMaxHP();
        hpSlider.value = unit.GetCurrentHP();
        currentHealth = unit.GetCurrentHP();
    }

    /// <summary>
    /// Changes hp slider based on current hp
    /// </summary>
    /// <param name="hp">current hp</param>
    public void SetHP(int hp)
    {
        currentHealth = hp;
        time = 0;
    }

    private void Update()
    {
        float targetHealth = currentHealth;
        float startHealth = hpSlider.value;
        time += Time.deltaTime * lerpSpeed;

        hpSlider.value = Mathf.Lerp(startHealth, targetHealth, time);
    }
}
