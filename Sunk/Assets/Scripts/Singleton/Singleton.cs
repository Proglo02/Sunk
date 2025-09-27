using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                {
                    CreateInstance();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    private static void CreateInstance()
    {
        GameObject obj = new GameObject(typeof(T).Name);
        instance = obj.AddComponent<T>();
    }
}
