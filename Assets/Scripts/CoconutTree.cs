using UnityEngine;

public class CoconutTree : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject coconutPrefab;
    public int minCoconuts = 1;
    public int maxCoconuts = 3;

    [Header("Spawn Area")]
    public Transform leafCenter; // point under leaves
    public float spawnRadius = 1.5f;
    public float heightOffset = -0.5f; // slightly below leaves

    void Start()
    {
        SpawnCoconuts();
    }

    void SpawnCoconuts()
    {
        int count = Random.Range(minCoconuts, maxCoconuts + 1);

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                heightOffset,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Vector3 spawnPosition = leafCenter.position + randomOffset;

            GameObject coconut = Instantiate(coconutPrefab, spawnPosition, Quaternion.identity);

            // Optional: slight random rotation
            coconut.transform.rotation = Random.rotation;
        }
    }
}
