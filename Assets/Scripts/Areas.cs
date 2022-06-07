using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Areas : MonoBehaviour
{

    public int CurrentEnemies;
    public int CurrentSpecials;
    public int EnemiesKilled;
    public int SpecialsKilled;
    public int Area;
    private bool wanderersSpawned;
    public List<Transform> ItemSpawns;
    private Director director;
    private bool itemsSpawned;
    public bool HasEntered;
    private bool exitted;
    public float AreaAccuracy;
    public int playerHit;
    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("Director").GetComponent<Director>();
        itemsSpawned = false;
        wanderersSpawned = false;
        HasEntered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemies enemy = other.gameObject.GetComponent<Damageable>();
            if (enemy.IsSpecial)
            {
                CurrentSpecials += 1;
            }
            else
            {
                CurrentEnemies += 1;
            }
            enemy.CurrentEnemyArea = Area;
        }
        else if (other.CompareTag("Player"))
        {





            if (!wanderersSpawned)
            {
                if (Area <= 2)
                {
                    wanderersSpawned = true;
                }
                else
                {
                    wanderersSpawned = true;
                    director.WandererSpawn(Area, false);
                }
            }
            if (!itemsSpawned)
            {
                itemsSpawned = true;
                foreach (var item in ItemSpawns)
                {
                    director.ItemSpawn(item);
                }
            }
        }
        else if (other.CompareTag("ItemSpawn"))
        {
            ItemSpawns.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<Enemies>().IsSpecial)
            {
                CurrentSpecials -= 1;
            }
            else
            {
                CurrentEnemies -= 1;
            }
        }
        else if (other.CompareTag("Player") && director.Areas[Area].HasEntered == false)
        {

            Invoke("SetAreaStats", 1f);

        }



    }



    void SetAreaStats()
    {
        if (director.CurrentArea < Area)
        {

        }
        else
        {
            AreaAccuracy = director.playerShooting.Accuracy;
            playerHit = director.PlayerHit;
            director.PlayerHit = 0;
        }
        director.CurrentArea = Area + 1;

    }
}
