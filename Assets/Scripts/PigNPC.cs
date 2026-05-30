using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class PigNPC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float wanderRadius  = 5f;
    [SerializeField] private float minWaitTime   = 2f;
    [SerializeField] private float maxWaitTime   = 6f;
    [SerializeField] private float moveSpeed     = 1.2f;

    [Header("Oink")]
    [SerializeField] private AudioClip[] oinkSounds;
    [SerializeField] private float minOinkInterval = 5f;
    [SerializeField] private float maxOinkInterval = 15f;

    private NavMeshAgent agent;
    private AudioSource  audioSource;
    private Vector3      startPosition;
    private float        nextOinkTime;
    private float        waitTimer;
    private bool         isWaiting;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed           = moveSpeed;
        agent.angularSpeed    = 120f;
        agent.acceleration    = 6f;
        agent.stoppingDistance = 0.3f;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance  = 12f;
        audioSource.rolloffMode  = AudioRolloffMode.Linear;
    }

    private void Start()
    {
        startPosition = transform.position;
        ScheduleOink();
        SetNewDestination();
    }

    private void Update()
    {
        if (Time.time >= nextOinkTime)
            PlayOink();

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetNewDestination();
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
    }

    private void SetNewDestination()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 rnd = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = startPosition + new Vector3(rnd.x, 0f, rnd.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    private void PlayOink()
    {
        if (oinkSounds != null && oinkSounds.Length > 0)
        {
            var clip = oinkSounds[Random.Range(0, oinkSounds.Length)];
            if (clip != null)
                audioSource.PlayOneShot(clip);
        }
        ScheduleOink();
    }

    private void ScheduleOink()
    {
        nextOinkTime = Time.time + Random.Range(minOinkInterval, maxOinkInterval);
    }
}