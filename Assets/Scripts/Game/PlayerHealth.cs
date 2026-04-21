using System;
using UnityEngine;
using TMPro;


namespace HealthSystem
{
    public class PlayerHealth : Health
    {
        [SerializeField] private TMP_Text hpText;

        public override void Init(int maxHp, Material mat)
        {
            base.Init(maxHp, mat);
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
        }


        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
        }

    }

}