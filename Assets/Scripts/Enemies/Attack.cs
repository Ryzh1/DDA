using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Enemies
{

    private bool canAttack;
    PlayerController target;
    MoveTo move;
    private void Start()
    {

        player = FindObjectOfType<PlayerController>().gameObject.transform;
        
        move = GetComponent<MoveTo>();
        canAttack = true;
        target = player.GetComponent<PlayerController>();
    }


    private void FixedUpdate()
    {
        if(player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < 5)
            {
                AttackRay();
            }
        }

    }

    private void AttackRay()
    {       
        int layermask1 = 1 << 8;
        layermask1 = ~layermask1;
        
        RaycastHit hit;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, out hit, 1f, layermask1))
        {
            if(hit.collider.CompareTag("Player") && canAttack)
            {
                move.canMove = true;
                canAttack = false;
                target.TakeDamage(AttackDamage);
                StartCoroutine(AttackTimer());
            }
            else
            {
                StartCoroutine(MoveDelay());
            }

        }
    }
    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(1f);
        move.canMove = true;
    }

    IEnumerator AttackTimer()
    {

        yield return new WaitForSeconds(2f);

        canAttack = true;
    }
}
