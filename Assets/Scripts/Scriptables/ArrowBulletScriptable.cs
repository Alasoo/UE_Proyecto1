using System;
using System.Collections.Generic;
using BulletSystem;
using UnityEngine;

namespace BulletSystem
{
    [CreateAssetMenu(fileName = "NewArrowBullet", menuName = "ScriptableObjects/Bullet/Arrow", order = 1)]
    public class ArrowBulletScriptable : BulletScriptable
    {
        [Header("BASE")]
        public ArrowBullet bulletPrefab;
        public int bulletCount;


        public override (Bullet bullet, int count) TakeBulletPrefab()
        {
            return (bulletPrefab, bulletCount);
        }
    }
}


