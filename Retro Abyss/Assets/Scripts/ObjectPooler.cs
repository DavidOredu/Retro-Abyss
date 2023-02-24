using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public PoolTag tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<PoolTag, Queue<GameObject>> poolDictionary;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public const int tagCount = 1;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        var poolList = Resources.Load<PoolList>("PoolList");
        pools = poolList.poolsToSpawn;

        poolDictionary = new Dictionary<PoolTag, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
               GameObject obj =  Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                spawnedObjects.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    private void Update()
    {
        foreach (var pool in pools)
        {
            if(poolDictionary[pool.tag].Count < pool.size)
            {
                var newCount = pool.size - poolDictionary[pool.tag].Count;
                for (int i = 0; i < newCount; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    poolDictionary[pool.tag].Enqueue(obj);
                }
            }
        }
    }
    public List<Pool> GetPoolsWithTags(PoolTag[] tags)
    {
        var newPools = new List<Pool>();

        foreach (var tag in tags)
        {
            foreach (var pool in pools)
            {
                if (pool.tag == tag)
                {
                    if(!newPools.Contains(pool))
                        newPools.Add(pool);
                }
            }
        }
        return newPools;
    }
    public Pool GetPoolWithTag(PoolTag tag)
    {
        foreach (var pool in pools)
        {
            if (pool.tag == tag)
                return pool;
        }
        return null;
    }
    public int GetSizeWithTag(PoolTag tag)
    {
        var pool = GetPoolWithTag(tag);
        return pool.size;
    }
    public int GetSizeWithTags(PoolTag[] tags)
    {
        int size = 0;
        var newPools = GetPoolsWithTags(tags);

        foreach (var newPool in newPools)
        {
            size += newPool.size;
        }
        return size;
    }
    public GameObject SpawnFromPool(PoolTag tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

       GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.parent = parent;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    public GameObject SpawnFromPoolAtRandom(PoolTag[] tags, Vector3 position, Quaternion rotation)
    {
        PoolTag tag = tags[Random.Range(0, tags.Length)];

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    private void OnDestroy()
    {
        
    }
    public enum PoolTag
    {
        Border,
        TriggerSquare,
    }
}
