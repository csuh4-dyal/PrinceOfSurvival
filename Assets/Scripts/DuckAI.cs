using UnityEngine;
using UnityEngine.AI;

public class DuckAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    private GameManager gameManager;

    [Header("UI")]
    public GameObject tamedIcon;

    [Header("Movement")]
    public float followDistance = 3f;
    public float wanderRadius = 8f;
    public float wanderInterval = 3f;
    private float wanderTimer = 0f;

    [Header("Seed Detection")]
    public float seedSearchRadius = 10f;

    [Header("Taming")]
    public int minSeedsToTame = 2;
    public int maxSeedsToTame = 3;

    private int seedsRequired;
    private int seedsFed = 0;
    private GameObject targetSeed;

    private enum DuckState { Wild, Tamed }
    private DuckState state;

    void Awake()
    {
        // Find GameManager automatically in the scene
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
            Debug.LogWarning("No GameManager found in the scene!");
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Configure NavMeshAgent
        agent.stoppingDistance = 0f;
        agent.autoBraking = false;
        agent.acceleration = 20f;
        agent.angularSpeed = 1000f;
        agent.speed = 4f;

        seedsRequired = Random.Range(minSeedsToTame, maxSeedsToTame + 1);
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
                WildBehavior();
                break;
            case DuckState.Tamed:
                TamedBehavior();
                break;
        }
    }

    void WildBehavior()
    {
        Wander();

        if (targetSeed != null)
        {
            agent.SetDestination(targetSeed.transform.position);

            if (Vector3.Distance(transform.position, targetSeed.transform.position) < 1.5f)
            {
                ConsumeSeed(targetSeed);
                targetSeed = null;
            }
        }
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval || !agent.hasPath)
        {
            Vector3 randomPoint = Random.insideUnitSphere * wanderRadius + transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            wanderTimer = 0f;
        }
    }

    void SearchForSeed()
    {
        if (state == DuckState.Tamed)
            return;

        GameObject seed = FindClosestSeed();
        if (seed != null)
            targetSeed = seed;
    }

    void ConsumeSeed(GameObject seedObj)
    {
        Seed seed = seedObj.GetComponent<Seed>();

        if (seed != null && seed.thrownByPlayer)
        {
            seedsFed++;
            int seedsLeft = seedsRequired - seedsFed;
            Debug.Log(name + " consumed a PLAYER seed (" + seedsFed + "/" + seedsRequired + "). Left: " + Mathf.Max(seedsLeft, 0));

            if (seedsFed >= seedsRequired)
                TameDuck();
        }
        else
        {
            Debug.Log(name + " ate a normal seed.");
        }

        Destroy(seedObj);
    }

    GameObject FindClosestSeed()
    {
        Collider[] seeds = Physics.OverlapSphere(transform.position, seedSearchRadius);
        float closest = Mathf.Infinity;
        GameObject closestSeed = null;

        foreach (Collider col in seeds)
        {
            if (!col.CompareTag("Seed"))
                continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestSeed = col.gameObject;
            }
        }

        return closestSeed;
    }

    void TamedBehavior()
    {
        if (player == null)
            return;

        Vector3 offset = (transform.position - player.position).normalized * followDistance;
        Vector3 targetPos = player.position + offset;
        agent.SetDestination(targetPos);
    }

    public void TameDuck()
    {
        state = DuckState.Tamed;

        CancelInvoke(nameof(SearchForSeed));
        targetSeed = null;

        agent.isStopped = false;
        agent.stoppingDistance = followDistance;
        agent.autoBraking = false;

        if (tamedIcon != null)
        {
            tamedIcon.SetActive(true);
            tamedIcon.transform.SetParent(transform);
            tamedIcon.transform.localPosition = new Vector3(0, 2f, 0);
        }

        Debug.Log(name + " has been tamed!");

        // Notify GameManager automatically
        if (gameManager != null)
            gameManager.OnDuckTamed();
    }

    void LateUpdate()
    {
        if (tamedIcon != null && tamedIcon.activeSelf)
        {
            tamedIcon.transform.LookAt(Camera.main.transform);
            tamedIcon.transform.Rotate(0, 180f, 0);
        }
    }
}