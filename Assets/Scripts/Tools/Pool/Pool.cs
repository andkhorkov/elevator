using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class Pool
    {
        private Queue<PoolObject> objects;
        private string path;
        private PoolObject asset;
        private HashSet<PoolObject> set;

        public Pool(string path)
        {
            this.path = path;
            asset = Resources.Load<PoolObject>(path);
            objects = new Queue<PoolObject>();
            set = new HashSet<PoolObject>();
        }

        public T GetObject<T>() where T : PoolObject
        {
            T obj;

            if (objects.Count > 0)
            {
                obj = (T) objects.Dequeue();
            }
            else
            {
                obj = (T) Object.Instantiate(asset);
                obj.SetPoolKey(path);
            }

            obj.OnTakenFromPool();
            set.Remove(obj);

            return obj;
        }

        public void ReturnObject(PoolObject obj)
        {
            if (set.Contains(obj))
            {
                return;
            }

            set.Add(obj);
            objects.Enqueue(obj);
            obj.OnReturnedToPool();
        }
    }
}
