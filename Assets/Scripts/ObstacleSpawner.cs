using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int poolSize = 5;
    public float spawnInterval = 2f;
    public float spawnX = 12f;
    public float minY = 1f;
    public float maxY = 9f;

    private readonly List<GameObject> pool = new List<GameObject>();
    private float timer;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab, Vector3.one * 1000f, Quaternion.identity);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return;
        float y = Random.Range(minY, maxY);
        obj.transform.position = new Vector3(spawnX, y, 0f);
        obj.SetActive(true);
    }

    GameObject GetPooledObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        // expand pool if none available
        GameObject objNew = Instantiate(obstaclePrefab, Vector3.one * 1000f, Quaternion.identity);
        objNew.SetActive(false);
        pool.Add(objNew);
        return objNew;
    }
}
