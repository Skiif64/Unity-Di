using UnityEngine;

namespace Singletons
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get => GetInstance();
            protected set => _instance = value;
        }

        private static T GetInstance()
        {
            if (_instance == null)
            {
                var instance = FindAnyObjectByType<T>();

                if (instance != null) return instance;
            }

            var go = new GameObject($"[Singleton] {typeof(T).Name} Auto generated");
            return go.AddComponent<T>();
        }

        private void Awake()
        {
            if (!Application.isPlaying) return;

            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            _instance = this as T;
        }
    }
}
