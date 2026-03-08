using UnityEngine;
using System.Collections;

public class SeedSpawner : MonoBehaviour
{
    [Header("Seed")]
    public GameObject seedPrefab;

    [Header("Island Spawn Area")]
    public Vector2 islandSize = new Vector2(200f, 200f);
    public Vector3 islandCenter = Vector3.zero;

    [Header("Spawn Settings")]
    public float spawnInterval = 30f;
    public int maxSeedsPerSpawn = 7;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

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
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float randomX = Random.Range(-islandSize.x / 2, islandSize.x / 2);
            float randomZ = Random.Range(-islandSize.y / 2, islandSize.y / 2);

            Vector3 spawnPos = new Vector3(
                islandCenter.x + randomX,
                islandCenter.y + 50f,
                islandCenter.z + randomZ
            );

            RaycastHit hit;

            if (Physics.Raycast(spawnPos, Vector3.down, out hit, 100f, groundLayer))
            {
                Instantiate(seedPrefab, hit.point, Quaternion.identity);
                return;
            }
        }
    }
}