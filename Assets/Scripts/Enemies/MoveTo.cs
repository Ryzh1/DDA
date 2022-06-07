using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : Enemies
{
    

    public bool IsChasing;
    private bool isWanderer;
    public bool canMove;

    void Awake()
    {

        player = FindObjectOfType<PlayerController>().gameObject.transform;
        audioManager = FindObjectOfType<AudioManager>();
        agent = GetComponent<NavMeshAgent>();
        
        isWanderer = true;
        canMove = true;
        
    }

    private void Update()
    {
        Chase();
    }

    private void Chase()
    {
        if(player == null)
        {
            return;
        }
        else
        {
            
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            if (!IsChasing)
            {
                if (distance < 15 || !isWanderer)
                {
                    int layermask1 = 1 << 8;
                    layermask1 = ~layermask1;

                    RaycastHit hit;
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    if (Physics.Raycast(transform.position, direction, out hit, 16, layermask1))
                    {
                        if(hit.collider.tag == "Player")
                        {
                            audioManager.PlayAudio("alert", transform.position, 1f);
                            canMove = true;
                            IsChasing = true;
                        }
                    }                   
                }
            }
            else if (distance > 2f && canMove)
            {
                agent.destination = player.position;
            }
        }
     
    }
}

