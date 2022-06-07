using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    PlayerController target;
    public int damage;

    private void Awake()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player.GetComponent<PlayerController>();
        StartCoroutine(DestroyTime());
    }


    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(10);

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<SphereCollider>().enabled = false;
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject.GetComponent<PlayerController>();
            target.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}
