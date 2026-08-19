using UnityEngine;
using UnityEngine.AI;

public class EnemyChaser : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }

    [Header("Referencias")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Visión")]
    public float viewRadius = 15f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;   // paredes, puertas, etc.
    public LayerMask playerMask;     // capa del jugador

    [Header("Movimiento")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float searchTime = 5f;    // segundos buscando antes de rendirse

    private NavMeshAgent agent;
    private State currentState = State.Patrol;
    private int currentPatrolIndex = 0;
    private Vector3 lastKnownPosition;
    private float searchTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        bool canSeePlayer = CanSeePlayer();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (canSeePlayer)
                    EnterChase();
                break;

            case State.Chase:
                if (canSeePlayer)
                {
                    lastKnownPosition = player.position;
                    agent.SetDestination(player.position);
                }
                else
                {
                    EnterSearch();
                }
                break;

            case State.Search:
                agent.SetDestination(lastKnownPosition);
                if (canSeePlayer)
                {
                    EnterChase();
                }
                else if (Vector3.Distance(transform.position, lastKnownPosition) < 1f)
                {
                    searchTimer -= Time.deltaTime;
                    if (searchTimer <= 0f)
                        EnterPatrol();
                }
                break;
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - transform.position);
        float distToPlayer = dirToPlayer.magnitude;

        if (distToPlayer > viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) return false;

        // Chequeo de obstáculos entre el enemigo y el jugador
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer.normalized,
            out RaycastHit hit, distToPlayer, obstacleMask | playerMask))
        {
            if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                return true;
        }
        return false;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void EnterChase()
    {
        currentState = State.Chase;
        agent.speed = chaseSpeed;
    }

    void EnterSearch()
    {
        currentState = State.Search;
        agent.speed = chaseSpeed;
        lastKnownPosition = player.position;
        searchTimer = searchTime;
    }

    void EnterPatrol()
    {
        currentState = State.Patrol;
        agent.speed = patrolSpeed;
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    // Para visualizar el cono de visión en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}