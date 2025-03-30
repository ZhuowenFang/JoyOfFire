using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    public Button attackButton;
    public Image energyBar;
    public TextMeshProUGUI energyText;
    public Button skillButton1;
    public Button skillButton2;
    public Button skillButton3;

    private float currentEnergy = 0f;
    private float maxEnergy = 10f;

    void Start()
    {
        // 初始设置，技能按钮不可用
        skillButton1.interactable = false;
        skillButton2.interactable = false;
        skillButton3.interactable = false;

        attackButton.onClick.AddListener(OnAttackButtonClick);
    }

    // 攻击按钮点击事件
    void OnAttackButtonClick()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += 1f;
            energyBar.fillAmount = currentEnergy / maxEnergy;
            energyText.text = $"{currentEnergy}/{maxEnergy}";
        }
        if (currentEnergy == maxEnergy)
        {
            ActivateSkillButtons();
        }
    }

    void ActivateSkillButtons()
    {
        skillButton1.interactable = true;
        skillButton2.interactable = true;
        skillButton3.interactable = true;
    }
}
