using System;
using System.Collections.Generic;
using UnityEngine;


namespace BulletSystem
{
    public abstract class BulletScriptable : ScriptableObject
    {
        [Header("DESIGN")]
        public Sprite sprite;
        public float lifeTime;
        public float speed;
        public int physicalDamage = 10;
        public int magicDamage = 10;

        public virtual (Bullet bullet, int count) TakeBulletPrefab() { return (null, 0); }
        public virtual (Color startColor, Color endColor) TakeColor() { return (Color.white, Color.white); }

    }
}


