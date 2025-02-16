using UnityEngine;

namespace SenteAI.Core
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                bool isRuntimeValid =
                    Application.isPlaying && instance != null && instance.gameObject.scene.isLoaded;

                if (instance == null && isRuntimeValid)
                    AgentLogger.LogWarning($"Instance of {typeof(T).Name} is null!");

                return instance;
            }
            protected set { instance = value; }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                AgentLogger.LogWarning($"{typeof(T).Name} already exists, destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
