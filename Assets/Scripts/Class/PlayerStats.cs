using System;
using UnityEngine;


namespace Controller.Player
{
    [Serializable]
    public class PlayerStats
    {
        public int currentDamage { get; private set; } = 20;
        public int currentHealth { get; private set; } = 100;
        public int maxHealth { get; private set; } = 100;
        public int currentProjectiles { get; private set; } = 1;
        private float movementSpeedBase = 3f;
        public int movementSpeedPercent { get; private set; } = 0;
        public int lifeStealPercent { get; private set; } = 0;

        public int armor { get; private set; } = 0;
        public float luckBase { get; private set; } = 10f;                //con esto hago que ignore el damage de los enemigos  
        public int luckPercent { get; private set; } = 0;
        private float criticalBase = 5;
        public int criticalPercent { get; private set; } = 0;
        private float attackSpeedBase = 2f;
        public int attackSpeedPercent { get; private set; } = 0;



        public int lvl { get; private set; } = 1;
        public int experience { get; private set; } = 0;
        public int maxExperience { get; private set; } = 1000;

        public event Action OnAddExperience;
        public event Action OnAddLvl;
        public event Action OnTakeDamage;
        public event Action OnDie;

        private bool isDie = false;



        public void AddProjectiles(int amount)
        {
            currentProjectiles += amount;
        }
        public void AddHealth(int amount)
        {
            maxHealth += amount;
            currentHealth += amount;
        }
        public void AddMovementSpeed(int amount)
        {
            movementSpeedPercent += amount;
        }
        public void AddLifeSteal(int amount)
        {
            lifeStealPercent += amount;
        }
        public void AddArmor(int amount)
        {
            armor += amount;
        }
        public void AddLuck(int amount)
        {
            luckPercent += amount;
        }
        public void AddCritical(int amount)
        {
            criticalPercent = Mathf.Min(criticalPercent + amount, 100);
        }
        public void AddDamage(int amount)
        {
            currentDamage += amount;
        }
        public void AddAttackSpeed(int amount)
        {
            attackSpeedPercent += amount;
        }

        public void AddExperience(int amount)
        {
            experience = Mathf.Min(experience + amount, maxExperience);
            if (experience >= maxExperience)
            {
                lvl++;
                experience = 0;
                OnAddLvl?.Invoke();
            }
            OnAddExperience?.Invoke();
        }


        public virtual void TakeDamage(int physicalDamage = 0, int magicalDamage = 0)
        {
            if (physicalDamage > 0)
                physicalDamage = Mathf.Max(physicalDamage - armor, 0);
            if (magicalDamage > 0)
                magicalDamage = (int)Mathf.Max(magicalDamage - armor / 2f, 0);      //la armadura reduce la mitad del daño mágico

            int totalDamage = physicalDamage + magicalDamage;

            currentHealth = Mathf.Max(0, currentHealth - totalDamage);
            OnTakeDamage?.Invoke();
            if (currentHealth <= 0 && !isDie)
            {
                isDie = true;
                OnDie?.Invoke();
            }
        }

        #region GETS
        public float GetMovementSpeed => movementSpeedBase + movementSpeedBase * movementSpeedPercent;
        public float GetLuck => luckBase + luckBase * luckPercent;
        public float GetSpeedAttack => attackSpeedBase + attackSpeedBase * attackSpeedPercent;
        public float GetCritical => criticalBase + criticalBase * criticalPercent;
        #endregion


    }
}