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

    public float floorLimit = 0.5f;
    public float ceilingLimit = 9.5f;

    // time after which two obstacles are spawned instead of one
    public float doubleSpawnTime = 20f;
    // time after which obstacle height starts increasing
    public float tallObstacleTime = 40f;
    // maximum height for tall obstacles
    public float maxObstacleHeight = 4f;

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
        float gameTime = Time.timeSinceLevelLoad;
        int count = gameTime >= doubleSpawnTime ? 2 : 1;

        float prevY = float.NaN;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GetPooledObject();
            if (obj == null) return;

            float height = 1f;
            if (gameTime >= tallObstacleTime)
            {
                height = Random.Range(1f, maxObstacleHeight);
                float maxAllowed = (ceilingLimit - floorLimit) - 1f;
                height = Mathf.Min(height, maxAllowed);
            }

            obj.transform.localScale = new Vector3(1f, height, 1f);

            float halfHeight = height / 2f;
            float y = Random.Range(minY, maxY);
            y = Mathf.Clamp(y, floorLimit + halfHeight, ceilingLimit - halfHeight);
            if (!float.IsNaN(prevY) && Mathf.Abs(y - prevY) < 0.5f)
            {
                y = Mathf.Clamp(prevY + Random.Range(1f, 2f) * (Random.value > 0.5f ? 1f : -1f),
                                 floorLimit + halfHeight, ceilingLimit - halfHeight);
            }
            prevY = y;

            obj.transform.position = new Vector3(spawnX, y, 0f);
            obj.SetActive(true);
        }
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
