using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBars : MonoBehaviour
{
    public Slider StaminaSlider;
    public Slider AttackCharge;
    public Slider BossHPSlider;

    public void MaxStamina(float stamina)
    {
        StaminaSlider.maxValue = stamina;
        StaminaSlider.value = stamina;
    }

    public void SetStamina(float stamina)
    {
        StaminaSlider.value = stamina;
    }

    public void MaxATKCharge(float charge)
    {
        StaminaSlider.maxValue = charge;
        StaminaSlider.value = charge;
    }

    public void SetATKCharge(float charge)
    {
        AttackCharge.value = charge;
    }

    public void MaxBossHP(float hp)
    {
        BossHPSlider.maxValue = hp;
        BossHPSlider.value = hp;
    }

    public void SetBossHP(float hp)
    {
        BossHPSlider.value = hp;
    }
}
