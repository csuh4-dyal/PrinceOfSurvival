using UnityEngine;
using UnityEngine.AI;

public class DuckAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("Movement")]
    public float followDistance = 3f;
    public float wanderRadius = 4f;

    [Header("Idle Detection")]
    public float idleTimeRequired = 3f;

    [Header("Seed Detection")]
    public float seedSearchRadius = 10f;

    [Header("Taming")]
    public int minSeedsToTame = 3;
    public int maxSeedsToTame = 7;

    private int seedsRequired;
    private int seedsFed = 0;

    private float idleTimer = 0f;
    private Vector3 lastPlayerPos;

    private GameObject targetSeed;

    private enum DuckState
    {
        FollowPlayer,
        Wander,
        CollectSeed,
        Tamed
    }

    private DuckState state;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        seedsRequired = Random.Range(minSeedsToTame, maxSeedsToTame + 1);

        lastPlayerPos = player.position;

        state = DuckState.FollowPlayer;
    }

    void Update()
    {
        DetectPlayerIdle();

        switch (state)
        {
            case DuckState.FollowPlayer:
                FollowPlayer();
                break;

            case DuckState.Wander:
                WanderNearPlayer();
                break;

            case DuckState.CollectSeed:
                MoveToSeed();
                break;

            case DuckState.Tamed:
                FollowPlayer();
                break;
        }
    }

    void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > followDistance)
        {
            agent.SetDestination(player.position);
        }
        else if (!agent.hasPath)
        {
            state = DuckState.Wander;
        }
    }

    void WanderNearPlayer()
    {
        if (!agent.hasPath)
        {
            Vector3 randomPoint = Random.insideUnitSphere * wanderRadius;
            randomPoint += player.position;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void DetectPlayerIdle()
    {
        float movement = Vector3.Distance(player.position, lastPlayerPos);

        if (movement < 0.1f)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0;
        }

        if (idleTimer > idleTimeRequired)
        {
            GameObject seed = FindClosestSeed();

            if (seed != null)
            {
                targetSeed = seed;
                state = DuckState.CollectSeed;
            }
        }

        lastPlayerPos = player.position;
    }

    void MoveToSeed()
    {
        if (targetSeed == null)
        {
            state = DuckState.FollowPlayer;
            return;
        }

        agent.SetDestination(targetSeed.transform.position);

        float dist = Vector3.Distance(transform.position, targetSeed.transform.position);

        if (dist < 1.5f)
        {
            EatSeed(targetSeed);
        }
    }

    GameObject FindClosestSeed()
    {
        Collider[] seeds = Physics.OverlapSphere(transform.position, seedSearchRadius);

        float closest = Mathf.Infinity;
        GameObject closestSeed = null;

        foreach (Collider col in seeds)
        {
            if (col.CompareTag("Seed"))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);

                if (dist < closest)
                {
                    closest = dist;
                    closestSeed = col.gameObject;
                }
            }
        }

        return closestSeed;
    }

    void EatSeed(GameObject seed)
    {
        Destroy(seed);

        seedsFed++;

        if (seedsFed >= seedsRequired)
        {
            TameDuck();
        }
        else
        {
            state = DuckState.FollowPlayer;
        }
    }

    void TameDuck()
    {
        state = DuckState.Tamed;

        Debug.Log("Duck has been tamed!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seed"))
        {
            EatSeed(other.gameObject);
        }
    }
}