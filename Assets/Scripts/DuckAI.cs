using UnityEngine;
using UnityEngine.AI;

public class DuckAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("UI")]
    public GameObject tamedIcon;

    [Header("Movement")]
    public float followDistance = 3f;
    public float wanderRadius = 8f;

    [Header("Seed Detection")]
    public float seedSearchRadius = 10f;

    [Header("Taming")]
    public int minSeedsToTame = 2;
    public int maxSeedsToTame = 5;

    private int seedsRequired;
    private int seedsFed = 0;

    private GameObject targetSeed;

    private enum DuckState
    {
        Wild,
        EatSeed,
        Tamed
    }

    private DuckState state;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        seedsRequired = Random.Range(2, 5 + 1);

        state = DuckState.Wild;

        if (tamedIcon != null)
            tamedIcon.SetActive(false);

        InvokeRepeating(nameof(SearchForSeed), 2f, 2f);
    }

    void Update()
    {
        switch (state)
        {
            case DuckState.Wild:
                Wander();
                break;

            case DuckState.EatSeed:
                MoveToSeed();
                break;

            case DuckState.Tamed:
                FollowPlayer();
                break;
        }
    }

    void Wander()
    {
        if (!agent.hasPath)
        {
            Vector3 randomPoint = Random.insideUnitSphere * wanderRadius + transform.position;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void SearchForSeed()
    {
        if (state == DuckState.Tamed)
            return;

        GameObject seed = FindClosestSeed();

        if (seed != null)
        {
            targetSeed = seed;
            state = DuckState.EatSeed;
        }
    }

    void MoveToSeed()
    {
        if (targetSeed == null)
        {
            state = DuckState.Wild;
            return;
        }

        agent.SetDestination(targetSeed.transform.position);

        float dist = Vector3.Distance(transform.position, targetSeed.transform.position);

        if (dist < 1.5f)
        {
            Destroy(targetSeed); // Duck eats the seed
            state = DuckState.Wild;
        }
    }

    void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > followDistance)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            Vector3 targetPos = player.position + dir * followDistance;

            agent.SetDestination(targetPos);
        }
        else
        {
            agent.ResetPath();
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

    // Call this externally when the player tames the duck
    void OnTriggerEnter(Collider other)
    {
        if (state == DuckState.Tamed)
            return;

        if (other.CompareTag("Seed"))
        {
            Seed seed = other.GetComponent<Seed>();

            if (seed != null && seed.thrownByPlayer)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                Destroy(other.gameObject);

                seedsFed++;

                int seedsLeft = seedsRequired - seedsFed;

                Debug.Log(
                    gameObject.name +
                    " consumed a player-thrown seed. (" +
                    seedsFed + "/" + seedsRequired +
                    "). Seeds left to tame: " +
                    Mathf.Max(seedsLeft, 0)
                );

                if (seedsFed >= seedsRequired)
                {
                    TameDuck();
                }
            }
        }
    }
    public void TameDuck()
    {
        state = DuckState.Tamed;

        // Activate the icon in world space
        if (tamedIcon != null)
        {
            tamedIcon.SetActive(true);
        }

        Debug.Log($"{name} has been tamed!");
    }
    void LateUpdate()
    {
        if (tamedIcon != null && tamedIcon.activeSelf)
        {
            tamedIcon.transform.LookAt(Camera.main.transform);
            tamedIcon.transform.Rotate(0, 180f, 0); // Flip because LookAt faces backwards
        }
    }
}