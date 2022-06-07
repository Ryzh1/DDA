using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject[] enemies;
    public Transform[] spawnPoints;
    public TriggerController[] Triggers;
    public int EnemiesKilled;
    public int SpecialsKilled;
    public int PlayerHit;
    private PlayerController player;
    private PlayerShooting playerShooting;
    public AudioSource hordeScreech;
    public AudioSource hordeMusic;

    private void Start()
    {
        playerShooting = FindObjectOfType<PlayerShooting>();

    }


    public IEnumerator SpawnHorde(int index)
    {
        hordeScreech.Play();
        hordeMusic.Play();
        Triggers[index - 1].AreaAccuracy = playerShooting.Accuracy;
        Triggers[index - 1].playerHit = PlayerHit;
        PlayerHit = 0;

        playerShooting.ResetAccuracy();
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2));
            GameObject spawned = Instantiate(enemies[0], spawnPoints[index - 1].transform.position, transform.rotation);
            spawned.GetComponent<MoveTo>().IsChasing = true;


            if (spawnPoints[index] != null)
            {
                GameObject spawned2 = Instantiate(enemies[0], spawnPoints[index].transform.position, transform.rotation);
                spawned2.GetComponent<MoveTo>().IsChasing = true;


            }


        }
    }


    public void EnemyDied(bool special)
    {
        if (special)
        {

            SpecialsKilled++;
        }
        else
        {
            EnemiesKilled++;
        }


    }
}