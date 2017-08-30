using UnityEngine;

namespace BRO
{
    /// <summary>
    /// This Singleton boilerplate makes sure that duplicates of the instance are prevented.
    /// It makes the component globally available.
    /// If an instance is not present yet, it instantiates one.
    /// If the instance shall be persistent across scenes, call DontDestroyOnLoad(this) manually.
    /// </summary>
    /// <typeparam name="T">The type of the component which is supposed to be a Singleton.</typeparam>
    [DisallowMultipleComponent] // "Prevents MonoBehaviour of same type (or subtype) to be added more than once to a GameObject."
    public abstract class GenericSingleton<T> : MonoBehaviour where T : Component
    {
        #region Member Fields
        private static T _instance;
        private static object _lock = new object();
        #endregion

        #region Member Properties
        public static T Instance
        {
            get
            {
                lock (_lock) // Lock marks code blocks as sensitive. This prevents other threads from accessing this code if the code is already in use by a thread.
                {
                    if (!_instance)
                    {
                        _instance = FindObjectOfType<T>(); // Find the component of type T within the scope of the scene
                        if (!_instance)
                        {
                            Instantiate(); // if an instance was not found in the present scene, create an instance (GameObject  + T)
                        }
                    }
                    return _instance;
                }
            }
        }
        #endregion

        #region Constructor
        protected GenericSingleton() { }    // Preventing the call of the constructor by subclasses
        #endregion

        #region UnityLifecycle
        /// <summary>
        /// Awake checks if this component is a duplicate or not.
        /// </summary>
        public virtual void Awake()
        {
            RemoveDuplicate();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Instantiates a GameObject and adds an instance of the T component to that Gameobject.
        /// </summary>
        private static void Instantiate()
        {
            GameObject go = new GameObject();
            go.name = typeof(T).Name;
            _instance = go.AddComponent<T>();
        }

        /// <summary>
        /// In the case of being a duplicate, this GameObject will be destroyed. If not, this instance is set as the instance.
        /// </summary>
        private void RemoveDuplicate()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }
        #endregion
    }
}