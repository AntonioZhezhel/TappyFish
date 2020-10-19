using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallaxer : MonoBehaviour
{

    class PoolObject
    {
        public Transform transform;
        public bool inUse;

        public PoolObject(Transform t)
        {
            transform = t;
        }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }
    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
	//частота появления 
    public float spawnRate;
    
    
    
    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;


    private float spawnTimer;
    private float targetAspect;
    private PoolObject[] poolObjects;
    private GameManager game;

    void Awake()
    {
        Configure();
    }	

    void OnEnable()
    {
        //GameManager указывает на окончание игры
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.Instance;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

        // Update is called once per frame
    void Update()
    {
        if (game.GameOver)
        {
            return;
        }

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
        
    }
    
    //появление компонентов
    void Spawn()
    {
        Transform t = GetPoolObject();
        //размер пула слишком мал 
        if (t == null)
        {
            return;
        }

        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect)/ targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;

    }

    //моментальное появление
    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        //размер пула слишком мал 
        if (t == null)
        {
            return;
        }

        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect)/ targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    //перемешение компонентов 
    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }
    
    //удаление обектов
    //проверка находится ли обект за приделами экрана 
    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect)/ targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }

        return null;
    }
}
