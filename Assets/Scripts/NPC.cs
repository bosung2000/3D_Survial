using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public  enum AIState
{
    Idle,
    Wandering,
    Attaking
}

public class NPC : MonoBehaviour,IDamageable
{
    [Header("Stats")]
    public int Health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aIState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWatiTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

    }

    // Start is called before the first frame update
    void Start()
    {
        SetState(AIState.Wandering);
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharaterManager.Instance.Player.transform.position);
        animator.SetBool("Moving", aIState != AIState.Idle);

        switch (aIState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                passivUpdate();
                break;
            case AIState.Attaking:
                AttackingUpdate();
                break;
            default:
                break;
        }
    }

    public void SetState(AIState state)
    {
        aIState = state;

        switch (aIState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed =walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attaking:
                agent.speed =runSpeed;
                agent.isStopped = false;
                break;
            default:
                break;
        }
        animator.speed =agent.speed /walkSpeed;
    }
    void passivUpdate()
    {
        if (aIState ==AIState.Wandering && agent.remainingDistance <0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWatiTime));

        }

        if (playerDistance<detectDistance)
        {
            SetState(AIState.Attaking);
        }
    }

    void WanderToNewLocation()
    {
        if (aIState != AIState.Idle) return;
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)),out hit,maxWanderDistance,NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position,hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        }

        return hit.position;
    }
    void AttackingUpdate()
    {
        if (playerDistance <attackDistance &&IsPlayerInFiledOfView() )
        {
            agent.isStopped= true;
            if (Time.time -lastAttackTime >attackRate)
            {
                lastAttackTime = Time.time;
                CharaterManager.Instance.Player.controller.GetComponent<IDamageable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
            else
            {
                if (playerDistance <detectDistance)
                {
                    agent.isStopped= false;
                    NavMeshPath path =new NavMeshPath();
                    if (agent.CalculatePath(CharaterManager.Instance.Player.transform.position,path))
                    {
                        agent.SetDestination(CharaterManager.Instance.Player.transform.position);
                    }
                    else
                    {
                        agent.SetDestination(transform.position);
                        agent.isStopped= true;
                        SetState(AIState.Wandering);
                    }
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }

        }
    }

    bool IsPlayerInFiledOfView()
    {
        Vector3 directionToPlayer = CharaterManager.Instance.Player.transform.position -transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView*0.5f;
    }

    public void TakePhysicalDamage(int damge)
    {
        Health -=damge;

        if (Health<=0)
        {
            Die();
        }

        StartCoroutine(DamageFlash());

    }

    void Die()
    {
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab,transform.position+Vector3.up *2,Quaternion.identity);
        }
        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }
}
