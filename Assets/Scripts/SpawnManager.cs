using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyPrefabs;
    [SerializeField]
    private GameObject Target;

   
    [Header("Time Between Spawn")]
    [SerializeField]
    private float minSpawnTime = 3;
    [SerializeField]
    private float maxSpawnTime = 5;
    

    SphereCollider sphereTrigger;

    float randomSpawnTime = 0;
    float currentTime = 5;

    private const int MAX_SPAWNER_NUMBER = 50;
    static int CURRENT_SPAWNER_COUNT = 0;


    private void Awake()
    {
        sphereTrigger = GetComponent<SphereCollider>();
        randomSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        currentTime = randomSpawnTime;
        CURRENT_SPAWNER_COUNT = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameSystem.START_GAME == false)
        {
            return;
        }

        if (CURRENT_SPAWNER_COUNT >= MAX_SPAWNER_NUMBER)
        {
            return;
        }

        currentTime -= Time.deltaTime;
        
        if(currentTime <= 0)
        {
            SpawnEnemy();
            randomSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            currentTime = randomSpawnTime;
        }
    }

    public void SpawnEnemy()
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length - 1);

        Vector3 randomPos = RandomPointInBounds(sphereTrigger.bounds);
        Vector3 spawnPos = new Vector3(randomPos.x, transform.position.y, randomPos.z);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex]);
        enemy.transform.position = spawnPos;
        enemy.GetComponent<EnemyAI_Base>().m_Target = Target;

        CURRENT_SPAWNER_COUNT++;
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public static bool CanFinishGame(int killedCount)
    {
        return killedCount >= MAX_SPAWNER_NUMBER;
    }
}
