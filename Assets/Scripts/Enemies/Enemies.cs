using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemies : MonoBehaviour
{
    protected Transform player;
    protected NavMeshAgent agent;
    public bool IsSpecial;
    public int Health = 100;
    public int AttackDamage;
    public bool Linear;
    public int CurrentEnemyArea;
    public AudioManager audioManager;



}
