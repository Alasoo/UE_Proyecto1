using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using MyExtensions;
using System.Collections.Generic;
using System;


namespace BulletSystem
{
    public class BulletPool : Singleton<BulletPool>
    {
        private readonly Dictionary<Type, Stack<Bullet>> pools = new();

        private bool taked = false;
        int contador = 0;


        public void CreateBullet(Bullet prefab, int count, BulletScriptable bulletScriptable)
        {
            Type type = prefab.GetType();

            if (!pools.TryGetValue(type, out Stack<Bullet> pool))
            {
                pool = new Stack<Bullet>(count);
                pools[type] = pool;
            }

            for (int i = pool.Count; i < count; i++)
            {
                Bullet bullet = Instantiate(prefab, transform);
                bullet.Init(bulletScriptable);
                bullet.OnRelease();
                pool.Push(bullet);
            }
        }

        public T Get<T>(T prefab, Vector3 direction, Vector3 position, BulletScriptable bulletScriptable) where T : Bullet
        {
            Type type = prefab.GetType();

            if (!pools.TryGetValue(type, out Stack<Bullet> pool))
            {
                pool = new Stack<Bullet>();
                pools[type] = pool;
            }

            Bullet bullet;

            if (pool.Count > 0)
            {
                bullet = pool.Pop();
            }
            else
            {
                bullet = Instantiate(prefab, transform);
                bullet.Init(bulletScriptable);
            }

            bullet.OnGet(direction, position);

            if (!taked)
            {
                contador++;
                if (contador == 15)
                {
                    taked = true;
                    bullet.transform.name = "Bullet1";
                }
            }
            return (T)bullet;
        }

        public void Return(Bullet bullet)
        {
            if (bullet.IsReleased) return;

            Type type = bullet.GetType();

            if (!pools.TryGetValue(type, out Stack<Bullet> pool))
            {
                pool = new Stack<Bullet>();
                pools[type] = pool;
            }

            bullet.OnRelease();
            pool.Push(bullet);
        }

        public void ClearBullets()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            pools.Clear();
        }






    }
}

