using UnityEngine;
using System.Collections;

public class SeedSpawner : MonoBehaviour
{
    [Header("Seed")]
    public GameObject seedPrefab;

    [Header("Island Area")]
    public float islandWidth = 600f;
    public float islandLength = 250f;

    [Header("Spawn Height")]
    public float spawnHeight = 50f;

    [Header("Spawn Settings")]
    public float spawnInterval = 30f;
    public int maxSeedsPerSpawn = 7;
    public int maxSpawnAttempts = 20;
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
                TrySpawnSeed();
            }
        }
    }

    void TrySpawnSeed()
    {
        int groundMask = LayerMask.GetMask("Ground");
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float randomX = Random.Range(-islandWidth / 2f, islandWidth / 2f);
            float randomZ = Random.Range(-islandLength / 2f, islandLength / 2f);

            Vector3 rayOrigin = new Vector3(randomX, spawnHeight, randomZ);

            // Cast downward and check if we hit the island
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, spawnHeight * 2f, groundMask))
            {
                Debug.Log("Ray hit: " + hit.collider.name + " | Tag: " + hit.collider.tag + " | Layer: " + hit.collider.gameObject.layer);
                if (hit.collider.CompareTag("GroundLayer"))
                {
                    // Spawn just above the hit point so it lands naturally
                    Vector3 spawnPos = hit.point + Vector3.up * 1f;
                    Instantiate(seedPrefab, spawnPos, Quaternion.identity);
                    return; // success, stop trying
                }
            }
            else
            {
                Debug.Log("Ray hit NOTHING - origin: " + rayOrigin);
            }
        }

        Debug.Log("SeedSpawner: could not find a valid island spot after " + maxSpawnAttempts + " attempts.");
    }
}