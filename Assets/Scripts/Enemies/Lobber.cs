using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lobber : Enemies
{
    public float wanderRadius = 10;
    public float wanderTimer = 10;

    private float timer;
    private bool canAttack;
    private float attackTimer = 0;
    private int attackRange = 40;
    private Damageable damageable;
    private PlayerController playerController;
    Enemies enemy;
    void Awake()
    {
        damageable = GetComponent<Damageable>();
        enemy = GetComponent<Enemies>();
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        player = FindObjectOfType<PlayerController>().gameObject.transform;
        playerController = player.GetComponent<PlayerController>();
        canAttack = false;
        IsSpecial = true;
        attackTimer = 5;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
            wanderTimer = Random.Range(5, 10);
        }

        if(attackTimer <= 0)
        {
            canAttack = true;
            attackTimer = 3;
        }
        else if(!canAttack)
        {
            attackTimer -= Time.deltaTime;
        }

        CheckCurrentState();

    }

    private void CheckCurrentState()
    {
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < attackRange && canAttack)
        {
            Attack();
        }
        int areaDifference = playerController.currentArea - enemy.CurrentEnemyArea;
        if(areaDifference >= 2 || areaDifference <= -4)
        {
            damageable.TakeDamage(Health, true);
        }
    
    }

    private void Attack()
    {
        int layermask1 = 1 << 7;
        int layermask2 = 1 << 8;
        int finalmask = layermask1 | layermask2;
        finalmask = ~finalmask;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, attackRange, finalmask))
        {
            if(hit.collider.CompareTag("Player"))
            {
                canAttack = false;
                var projectile = Instantiate(Resources.Load("Projectiles/Projectile", typeof(GameObject)) as GameObject, transform.position, transform.rotation);
                var rb = projectile.GetComponent<Rigidbody>();
                Vector3 fireDirection = player.transform.position - transform.position;
                fireDirection = new Vector3(fireDirection.x += Random.Range(-0.5f, 0.5f), fireDirection.y, fireDirection.z).normalized;
                rb.AddForce(fireDirection * 2500);
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}

