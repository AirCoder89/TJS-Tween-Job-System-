using UnityEngine;

namespace AirCoder.TJ.Core
{
    public abstract class Singleton<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
                return _instance;
            }
        }
    }
}