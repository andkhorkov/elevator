using UnityEngine;

namespace Pool
{
    public abstract class PoolObject : MonoBehaviour
    {
        private string path;

        public abstract void OnTakenFromPool();
        public abstract void OnReturnedToPool();
        public abstract void OnPreWarmed();

        public string Path { get { return path; } }

        public void SetPoolKey(string path)
        {
            this.path = path;
        }

        public void SetOrientation(Vector3 position, Quaternion rotation)
        {
            transform.parent = null;
            transform.position = position;
            transform.rotation = rotation;
        }

        public void ReturnObject()
        {
            PoolManager.ReturnObject(this, path);
        }
    }
}
