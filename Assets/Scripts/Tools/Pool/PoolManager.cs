using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class PoolManager : MonoBehaviour //could've been non monobehaviour
    {
        private static Dictionary<string, Pool> pools;

        public static PoolManager Instance { get; private set; }

        void Awake()
        {
            pools = new Dictionary<string, Pool>();
            Instance = this;
        }

        void OnDestroy()
        {
            pools = null;
            Instance = null;
        }

        public static T GetObject<T>(string path) where T : PoolObject
        {
            if (!pools.TryGetValue(path, out var pool))
            {
                pool = new Pool(path);
                pools.Add(path, pool);
            }

            var poolObj = pool.GetObject<T>();

            return poolObj;
        }

        public static void ReturnObject(PoolObject obj, string path)
        {
            if (!pools.TryGetValue(path, out var pool))
            {
                Debug.LogErrorFormat("PoolManager: there is no pool at given path {0}", path);
                return;
            }

            pool.ReturnObject(obj);
        }

        public static void PreWarm<T>(string path, int objectCount) where T : PoolObject
        {
            if (pools.ContainsKey(path))
            {
                return;
            }

            var objects = new List<T>(objectCount);

            for (int i = 0; i < objectCount; i++)
            {
                objects.Add(GetObject<T>(path));
            }

            for (int i = 0; i < objectCount; i++)
            {
                objects[i].OnPreWarmed();
                objects[i].ReturnObject();
            }
        }

        public static void ClearAllPools()
        {
            pools.Clear();
        }
    }
}
