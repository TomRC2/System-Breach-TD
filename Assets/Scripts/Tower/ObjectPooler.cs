using System.Collections.Generic;
using UnityEngine;

// Pool generico de GameObjects por prefab: evita el costo de Instantiate/Destroy
// (y la basura que generan para el GC) en objetos que se crean y destruyen todo
// el tiempo, como los proyectiles. Se auto-inicializa igual que AudioManager /
// CRTOverlay, asi que no hace falta agregar nada a mano en las escenas.
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("ObjectPooler");
        go.AddComponent<ObjectPooler>();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Saca una instancia lista para usar (reciclada si hay una disponible, nueva si no).
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }

        // Descarta referencias muertas (p. ej. instancias que Unity destruyo al
        // descargar la escena anterior porque no eran DontDestroyOnLoad).
        while (queue.Count > 0 && queue.Peek() == null)
            queue.Dequeue();

        GameObject obj;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
            instanceToPrefab[obj] = prefab;
        }

        return obj;
    }

    // Devuelve una instancia al pool en vez de destruirla.
    public void Release(GameObject obj)
    {
        if (obj == null) return;

        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            // No vino de Get(): no hay pool a la que volver, se destruye normalmente.
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}
