using UnityEngine;

namespace Singletons
{
    public abstract class PersistentSingleton<T> : Singleton<T>
        where T: Component
    {
        private Transform _parent;

        public Transform Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                transform.SetParent(value);
            }
        }
        protected override void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}