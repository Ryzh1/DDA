using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : Enemies
{
    MoveTo moveTo;
    GameObject director;
    Director dir;
    GameObject levelControllerObject;
    LevelController levelController;
    


    private void Awake()
    {
        moveTo = GetComponent<MoveTo>();
        director = GameObject.FindGameObjectWithTag("Director");
        levelControllerObject = GameObject.FindGameObjectWithTag("GameController");
        audioManager = FindObjectOfType<AudioManager>();
        if (director != null)
        {
            dir = director.GetComponent<Director>();
        }
        else
        {
            levelController = levelControllerObject.GetComponent<LevelController>();
        }

        
                          
        
    }


    public void TakeDamage(int damage, bool areaDeath)
    {
        if (!IsSpecial)
        {
            if (!moveTo.IsChasing)
            {
                audioManager.PlayAudio("alert", transform.position, 1f);
                moveTo.IsChasing = true;
            }
        }
        else
        {
            GetComponent<Lobber>().wanderTimer = 0;
        }
        Collider[] radius = Physics.OverlapSphere(transform.position, 10f);
        foreach (var item in radius)
        {
            MoveTo objMove = item.GetComponent<MoveTo>();
            if (objMove != null)
            {
                objMove.IsChasing = true;
            }
        }


        Health -= damage;
        
        if(Health <= 0)
        {
            if(dir != null)
            {
                if (areaDeath)
                {
                    dir.EnemyDied(CurrentEnemyArea, transform, IsSpecial, true);
                }
                else
                {
                    dir.EnemyDied(CurrentEnemyArea, transform, IsSpecial, false);
                    audioManager.PlayAudio("death", transform.position, 1f);
                }
                
            }
            
            else
            {
                levelController.EnemyDied(IsSpecial);
                audioManager.PlayAudio("death", transform.position, 1f);
            }
            
            Destroy(this.gameObject);
        }

    }
}
