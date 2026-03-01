

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            return instance;
        }
    }


    protected virtual void Awake()
    {
        if (instance == null)
            instance = (T)this;
    }
}

