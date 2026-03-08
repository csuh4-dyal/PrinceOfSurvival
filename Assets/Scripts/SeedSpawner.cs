using UnityEngine;
using System.Collections;

public class SeedSpawner : MonoBehaviour
{
    [Header("Seed")]
    public GameObject seedPrefab;

    [Header("Island Area")]
    public float islandWidth = 200f;
    public float islandLength = 200f;

    [Header("Spawn Height")]
    public float spawnHeight = 50f;

    [Header("Spawn Settings")]
    public float spawnInterval = 30f;
    public int maxSeedsPerSpawn = 7;

    void Start()
    {
        StartCoroutine(SpawnSeeds());
    }

    IEnumerator SpawnSeeds()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int spawnAmount = Random.Range(0, maxSeedsPerSpawn + 1);

            for (int i = 0; i < spawnAmount; i++)
            {
                SpawnSeed();
            }
        }
    }

    void SpawnSeed()
    {
        float randomX = Random.Range(-islandWidth / 2f, islandWidth / 2f);
        float randomZ = Random.Range(-islandLength / 2f, islandLength / 2f);

        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        Instantiate(seedPrefab, spawnPosition, Quaternion.identity);
    }
}