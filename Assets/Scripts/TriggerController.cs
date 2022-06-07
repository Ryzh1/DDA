using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public LevelController level;
    public int triggerIndex;
    private bool triggered;
    public float AreaAccuracy = 100;
    public int playerHit;

    private void Start()
    {
        triggered = false;
    }
    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(level.SpawnHorde(triggerIndex));
            
        }
        
    }
}
