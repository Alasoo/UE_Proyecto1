using UnityEngine;



public class SingletonDontDestroy<T> : MonoBehaviour where T : SingletonDontDestroy<T>
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);


                // 2. Si lo acabamos de encontrar en la escena (incluso si está apagado)
                if (instance != null)
                {
                    // Lo desvinculamos de cualquier padre para evitar advertencias de Unity
                    instance.transform.SetParent(null);


                    // ¡Lo protegemos de la destrucción aquí mismo!
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }




    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        // Si ya hay una instancia y no es esta, nos destruimos y NO aplicamos DontDestroyOnLoad
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}



