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
        private Dictionary<Type, List<Bullet>> pools = new();

        private CancellationTokenSource ctsCreator;


        private void OnDestroy()
        {
            ctsCreator?.ClearCts();
            ctsCreator = null;
        }

        public async UniTask CreateBullet<T>(T prefab, int count, BulletScriptable bulletScriptable) where T : Bullet
        {
            Type type = prefab.GetType();
            if (pools.ContainsKey(type)) return;

            ctsCreator?.Cancel();
            ctsCreator = new();

            try
            {
                pools[type] = new List<Bullet>();

                for (int i = 0; i < count; i++)
                {
                    T clone = Instantiate(prefab, transform);
                    clone.Init(bulletScriptable);
                    clone.OnRelease();
                    pools[type].Add(clone);

                    if (i % 5 == 0) await UniTask.Yield(ctsCreator.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
            finally
            {
                ctsCreator?.ClearCts();
                ctsCreator = null;
            }
        }



        public T Get<T>(T prefab, Vector3 direction, Vector3 pos, BulletScriptable bulletScriptable) where T : Bullet
        {
            Type type = prefab.GetType();
            if (pools.TryGetValue(type, out var list) && list.Count > 0)
            {
                int lastIndex = list.Count - 1;
                Bullet bullet = list[lastIndex];
                list.RemoveAt(lastIndex);

                bullet.OnGet(direction, pos);
                return (T)bullet;
            }

            T newBullet = Instantiate(prefab, transform);
            newBullet.Init(bulletScriptable);

            newBullet.OnGet(direction, pos);
            return newBullet;
        }

        public void Return<T>(T bullet) where T : Bullet
        {
            Type type = typeof(T);

            if (!pools.ContainsKey(type))
                pools[type] = new List<Bullet>();

            bullet.OnRelease();
            pools[type].Add(bullet);
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


