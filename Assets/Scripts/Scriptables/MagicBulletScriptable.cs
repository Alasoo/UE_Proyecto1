using System;
using System.Collections.Generic;
using BulletSystem;
using UnityEngine;

namespace BulletSystem
{
    [CreateAssetMenu(fileName = "NewMagicBullet", menuName = "ScriptableObjects/Bullet/Magic", order = 1)]
    public class MagicBulletScriptable : BulletScriptable
    {
        [Header("BASE")]
        public MagicBullet bulletPrefab;
        public int bulletCount;
        [Space]
        public Color startColor = Color.white;
        public Color endColor = Color.white;


        public override (Bullet bullet, int count) TakeBulletPrefab()
        {
            return (bulletPrefab, bulletCount);
        }
        public override (Color startColor, Color endColor) TakeColor()
        {
            return (startColor, endColor);
        }

    }
}


