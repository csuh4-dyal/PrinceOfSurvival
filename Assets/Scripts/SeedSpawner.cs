using UnityEngine;
using System.Collections;

public class SeedSpawner : MonoBehaviour
{
    [Header("Seed")]
    public GameObject seedPrefab;

    [Header("Island Reference")]
    public Collider islandCollider; // Assign your island object

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public int minSeedsPerSpawn = 5;
    public int maxSeedsPerSpawn = 7;
    public int maxSpawnAttempts = 20;

    [Header("Spawn Height")]
    public float spawnHeight = 50f;

    void Start()
    {
        if (islandCollider == null)
        {
            Debug.LogError("SeedSpawner: No island collider assigned!");
            return;
        }

        StartCoroutine(SpawnSeeds());
    }

    IEnumerator SpawnSeeds()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int spawnAmount = Random.Range(minSeedsPerSpawn, maxSeedsPerSpawn + 1);

            for (int i = 0; i < spawnAmount; i++)
            {
                TrySpawnSeed();
            }
        }
    }

    void TrySpawnSeed()
    {
        Bounds bounds = islandCollider.bounds;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 rayOrigin = new Vector3(randomX, bounds.max.y + spawnHeight, randomZ);

            // Raycast downward to find ground
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, spawnHeight * 2f))
            {
                // Ensure we hit the island
                if (hit.collider == islandCollider)
                {
                    Vector3 spawnPos = hit.point + Vector3.up * 0.5f;
                    Instantiate(seedPrefab, spawnPos, Quaternion.identity);
                    return;
                }
            }
        }

        Debug.Log("SeedSpawner: Failed to find valid spawn position.");
    }
}