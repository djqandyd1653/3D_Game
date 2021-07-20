using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image hpBarImage;

    public void SetMaxHpBar(float maxHp)
    {
        slider.maxValue = maxHp;
        slider.value = maxHp;

        hpBarImage.color = gradient.Evaluate(1f);
    }

    public void SetHpBar(float currHp)
    {
        slider.value = currHp;

        hpBarImage.color = gradient.Evaluate(slider.normalizedValue);
    }
}
