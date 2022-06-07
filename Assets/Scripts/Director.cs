using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Director : MonoBehaviour
{
    public int EnemiesKilled;
    public int SpecialsKilled;
    public int PlayerHit;
    public int CurrentEnemies = 0;
    public int CurrentSpecials = 0;
    public int PlayerAmmo;
    public float PlayerHealth;
    public int CurrentArea;

    public GameObject[] Enemies;
    public Transform[] SpawnPoints;
    public Areas[] Areas;
    public Transform[] ItemSpawns;





    private GameObject player;
    private PlayerController playerController;
    public PlayerShooting playerShooting;
    public AudioSource hordeScreech;
    public AudioSource hordeMusic;

    private bool canSpawn;
    private float hordeTimer = 30f;
    private float restTimer = 30f;
    private bool horde;
    private float specialSpawnTimer = 0f;
    private bool specialsSpawned = false;

    public float intensity
    {
        get { return _intensity; }
        set { _intensity = Mathf.Clamp(value, 1, 100); }
    }
    private float _intensity;

    public int CurrentMedkits;
    public int CurrentStims;
    public int CurrentAmmoPiles;


    private float spawnAmount;
    private float spawnMultiplier;


    enum Phase
    {
        Buildup,
        Peak,
        Rest
    }
    Phase currentPhase;
    private bool spawnChosen;

    void Start()
    {
        canSpawn = true;
        spawnAmount = 1;
        spawnMultiplier = 1;
        horde = false;
        player = FindObjectOfType <PlayerController>().gameObject;
        playerController = player.GetComponent<PlayerController>();
        playerShooting = player.GetComponent<PlayerShooting>();

        currentPhase = Phase.Buildup;
        PlayerHealth = playerController.currentHealth;
        PlayerAmmo = playerShooting.MaxAmmo;

        WandererSpawn(1, true);
        WandererSpawn(2, true);
        CurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;


        Debug.Log(Application.dataPath);
        
    }


    void Update()
    {
        playerController.currentArea = CurrentArea;
        if (PlayerHealth != playerController.currentHealth)
        {
            PlayerHealth = playerController.currentHealth;
            intensity += 5;
        }
        PlayerAmmo = playerShooting.MaxAmmo;

        if (intensity > 0)
        {
            intensity -= Time.deltaTime;
        }


        CheckPhase();
        SpecialManager();
    }

    float PlayerSkill()
    {
        float inverse = 100 - playerShooting.Accuracy;
        float avg = inverse + intensity / 2;
        return avg;
    }
    private float PlayerState()
    {
        float skill = PlayerSkill();
        float output;
        if (skill <= 10)
        {
            spawnMultiplier += 0.5f;
        }
        else if(skill <= 20 && skill > 10)
        {
            spawnMultiplier += 0.3f;
        }
        else if (skill <= 30 && skill > 20)
        {
            spawnMultiplier += 0.2f;
        }
        else if (skill <= 40 && skill > 30)
        {
            spawnMultiplier += 0.1f;
        }
        else if (skill <= 50 && skill > 40)
        {
            spawnMultiplier -= 0f;
        }
        else if (skill <= 60 && skill > 50)
        {
            spawnMultiplier -= 0.1f;
        }
        else if (skill <= 70 && skill > 60)
        {
            spawnMultiplier -= 0.2f;
        }
        else if (skill <= 80 && skill > 70)
        {
            spawnMultiplier -= 0.3f;
        }
        else if (skill > 80)
        {
            spawnMultiplier -= 0.5f;
        }

        if(PlayerHealth <= 25)
        {
            spawnMultiplier -= 0.5f;
        }
        else if (PlayerHealth <= 50 && PlayerHealth > 25)
        {
            spawnMultiplier -= 0.3f;
        }
        else if (PlayerHealth <= 75 && PlayerHealth > 50)
        {
            spawnMultiplier += 0.3f;
        }
        else if (PlayerHealth > 75)
        {
            spawnMultiplier += 0.5f;
        }

        spawnAmount = spawnAmount * spawnMultiplier;
        if (horde)
        {
            output = 5 + spawnAmount * 10;
            output = output / 2;
        }
        else
        {
            output = 5 + spawnAmount * 10;
        }
        spawnAmount = 1;
        spawnMultiplier = 1;
        Debug.Log(output);
        return output;

    }

    public void WandererSpawn(int area, bool initial)
    {
        int layermask1 = 1 << 8;
        int layermask2 = 1 << 2;
        int finalmask = layermask1 | layermask2;
        finalmask = ~finalmask;
        int amount;

        if (!initial)
        {
            amount = (int)PlayerState();
        }
        else
        {
            amount = 15;
        }

        for (int i = 0; i < amount; i++)
        {
            RaycastHit hit;
            
            while (!spawnChosen)
            {
                var bounds = Areas[area - 1].gameObject.GetComponent<Collider>();
                var _xAxis = Random.Range(bounds.bounds.min.x, bounds.bounds.max.x);
                var _zAxis = Random.Range(bounds.bounds.min.z, bounds.bounds.max.z);

                
                Vector3 pos = new Vector3(_xAxis, 0, _zAxis);
                float distance = Vector3.Distance(pos, player.transform.position);
                pos.y = 50f;

                if (Physics.Raycast(pos, -transform.up, out hit, Mathf.Infinity, finalmask))
                {
                    if(hit.collider.tag == "Terrain")
                    {
                        if (distance > 15)
                        {                           
                            spawnChosen = true;
                            pos.y = 1f;
                            Instantiate(Enemies[0], pos, transform.rotation);
                            CurrentEnemies++;
                        }
                    }
                }                  
              
            }
            spawnChosen = false;
        }

    }


    private void CheckPhase()
    {
        if (currentPhase == Phase.Buildup)
        {
            if (hordeTimer <= 0)
            {
                canSpawn = true;
                hordeTimer = 30f;
                currentPhase = Phase.Peak;
            }
            else
            {
                hordeTimer -= Time.deltaTime;
            }
        }

        else if (currentPhase == Phase.Peak)
        {
            if (CurrentEnemies < 15 && canSpawn && intensity < 30)
            {
                intensity += 25;
                StartCoroutine(SpawnHorde(CurrentArea));
                canSpawn = false;
                horde = true;
            }
            else if (!horde)
            {
                currentPhase = Phase.Buildup;
            }

        }

        else if (currentPhase == Phase.Rest)
        {
            horde = false;
            if (restTimer <= 0)
            {
                restTimer = 30f;
                currentPhase = Phase.Buildup;
            }
            else
            {
                restTimer -= Time.deltaTime;
            }
        }
    }

    public IEnumerator SpawnHorde(int index)
    {
        float currentIntensity = intensity;
        hordeScreech.Play();
        hordeMusic.Play();
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2));
            GameObject spawned;
            if (index < 6)
            {
                spawned = Instantiate(Enemies[0], SpawnPoints[index].transform.position, transform.rotation);
            }
            else
            {
                spawned = Instantiate(Enemies[0], SpawnPoints[index - 1].transform.position, transform.rotation);
            }
            
            spawned.GetComponent<MoveTo>().IsChasing = true;

            CurrentEnemies++;
            GameObject spawned2;
            if (index >= 2 && currentIntensity < 20)
            {
                spawned2 = Instantiate(Enemies[0], SpawnPoints[index - 2].transform.position, transform.rotation);
                CurrentEnemies++;
                spawned2.GetComponent<MoveTo>().IsChasing = true;
            }
            else if(intensity < 20)
            {
                spawned2 = Instantiate(Enemies[0], SpawnPoints[index - 1].transform.position, transform.rotation);
                CurrentEnemies++;
                spawned2.GetComponent<MoveTo>().IsChasing = true;
            }
        }        
        currentPhase = Phase.Rest;        
    }

    public void EnemyDied(int area, Transform location, bool special, bool areaDeath)
    {
        var distance = Vector3.Distance(location.position, player.transform.position);
        if (special)
        {
            if(!areaDeath)
            {
                SpecialsKilled++;
                specialSpawnTimer = 30;
                Areas[CurrentArea - 1].SpecialsKilled++;
                Areas[area - 1].CurrentSpecials--;
            }
            else
            {
                specialSpawnTimer = 0;
            }
            CurrentSpecials--;

        }
        else
        {
            CurrentEnemies--;
            Areas[CurrentArea - 1].EnemiesKilled++;
            Areas[area - 1].CurrentEnemies--;
            EnemiesKilled++;
        }
        

        if (distance < 2)
        {
            intensity += 2;
        }
    }

    public void ItemSpawn(Transform location)
    {

        float itemMultiplier = ItemChooser();

        if (itemMultiplier > 1.1 && CurrentMedkits == 0 && CurrentStims == 0)
        {
            if (itemMultiplier > 1.3)
            {
                Instantiate(Resources.Load("Pickups/Medkit", typeof(GameObject)), location.position, location.rotation);
                CurrentMedkits++;
            }
            else
            {
                Instantiate(Resources.Load("Pickups/Stim", typeof(GameObject)), location.position, location.rotation);
                CurrentStims++;
            }
        }
        else if (itemMultiplier < 0.9 && CurrentAmmoPiles < 1)
        {
            Instantiate(Resources.Load("Pickups/AmmoPile", typeof(GameObject)), location.position, location.rotation);
            CurrentAmmoPiles++;
        }
        else
        {
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    if(CurrentAmmoPiles > 0)
                    {
                        Instantiate(Resources.Load("Guns/" + playerShooting.Guns[Random.Range(0, playerShooting.Guns.Length)].weaponName, typeof(GameObject)), location.position, location.rotation);
                    }
                    else
                    {
                        Instantiate(Resources.Load("Pickups/AmmoPile", typeof(GameObject)), location.position, location.rotation);
                        CurrentAmmoPiles++;
                    }
                    break;
                case 1:
                    Instantiate(Resources.Load("Guns/" + playerShooting.Guns[Random.Range(0,playerShooting.Guns.Length)].weaponName, typeof(GameObject)), location.position, location.rotation);
                    break;
                case 2:
                    if(CurrentStims > 0)
                    {
                        Instantiate(Resources.Load("Guns/" + playerShooting.Guns[Random.Range(0, playerShooting.Guns.Length)].weaponName, typeof(GameObject)), location.position, location.rotation);
                    }
                    else
                    {
                        Instantiate(Resources.Load("Pickups/Stim", typeof(GameObject)), location.position, location.rotation);
                        CurrentStims++;
                    }
                    break;
                default:
                    break;
            }
            
        }

    }

    float ItemChooser()
    {

        float healthItemMultiplier = 1;
        float ammoMultiplier = 1;
        
        if (PlayerHealth <= 10)
        {
            healthItemMultiplier += 0.5f;
            if(playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
            
        }
        else if (PlayerHealth <= 20 && PlayerHealth > 10)
        {
            healthItemMultiplier += 0.4f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 30 && PlayerHealth > 20)
        {
            healthItemMultiplier += 0.3f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 40 && PlayerHealth > 30)
        {
            healthItemMultiplier += 0.2f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 50 && PlayerHealth > 40)
        {
            healthItemMultiplier += 0.1f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 60 && PlayerHealth > 50)
        {
            healthItemMultiplier += 0.05f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 70 && PlayerHealth > 60)
        {
            healthItemMultiplier -= 0.3f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth <= 80 && PlayerHealth > 70)
        {
            healthItemMultiplier -= 0.4f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }
        else if (PlayerHealth > 80)
        {
            healthItemMultiplier -= 0.5f;
            if (playerController.medKitCount > 0 || playerController.stimCount > 0)
            {
                healthItemMultiplier -= 0.2f;
            }
        }

        if (PlayerAmmo <= 100)
        {
            ammoMultiplier += 0.5f;
            if(CurrentAmmoPiles > 0)
            {
                ammoMultiplier -= 0.2f;
            }
        }
        else if (PlayerAmmo <= 200 && PlayerAmmo > 100)
        {
            ammoMultiplier += 0.3f;
            if (CurrentAmmoPiles > 0)
            {
                ammoMultiplier -= 0.2f;
            }
        }
        else if (PlayerAmmo <= 300 && PlayerAmmo > 200)
        {
            ammoMultiplier -= 0.3f;
            if (CurrentAmmoPiles > 0)
            {
                ammoMultiplier -= 0.2f;
            }
        }
        else if (PlayerAmmo > 300)
        {
            ammoMultiplier -= 0.5f;
            if (CurrentAmmoPiles > 0)
            {
                ammoMultiplier -= 0.2f;
            }
        }
        if(ammoMultiplier > healthItemMultiplier)
        {
            return 0.8f;
        }
        else if( healthItemMultiplier > ammoMultiplier)
        {
            if(healthItemMultiplier >= 1.3)
            {
                return 1.5f;
            }
            else
            {
                return 1.2f;
            }
        }
        else if(healthItemMultiplier == 0.5 && ammoMultiplier == 0.5)
        {
            return 1;
        }
        else
        {
            int rand = Random.Range(0, 2);
            switch (rand)
            {
                case 0:
                    return 1.2f;
                case 1:
                    return 0.8f;
                default:
                    break;
            }
        }
        return 1;

    }
   


    void SpecialManager()
    {
        if (CurrentSpecials < 4 && PlayerSkill() < 50 && specialSpawnTimer <= 0f)
        {
            
            Vector3 newPos = RandomNavSphere(transform.position, 100);
            newPos.y = 20;
            RaycastHit hit;
            int layermask1 = 1 << 2;
            layermask1 = ~layermask1;
            if (Physics.Raycast(newPos, -transform.up, out hit, Mathf.Infinity, layermask1))
            {
                newPos.y = 1.4f;
                if (hit.collider.gameObject.layer == 8)
                {
                    float distance = Vector3.Distance(newPos, player.transform.position);
                    int area = hit.collider.gameObject.GetComponent<Areas>().Area;
                    if (playerController.currentArea != area && area > 1 && distance > 30)
                    {
                        if (!specialsSpawned)
                        {

                            CurrentSpecials++;
                            Instantiate(Enemies[1], newPos, transform.rotation);

                            if (CurrentSpecials == 4)
                            {
                                specialsSpawned = true;
                                specialSpawnTimer = 30f;
                            }
                        }
                        else
                        {
                            if (CurrentSpecials == 2)
                            {
                                CurrentSpecials++;
                                Instantiate(Enemies[1], newPos, transform.rotation);
                                specialSpawnTimer = 30f;
                            }
                            else
                            {
                                CurrentSpecials++;
                                Instantiate(Enemies[1], newPos, transform.rotation);
                            }
                        }
                    }

                }             
            }
        }
        else if(specialSpawnTimer > 0 && CurrentSpecials < 4)
        {
            specialSpawnTimer -= Time.deltaTime;
        }
        
    }



    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, -1);
        
        return navHit.position;
    }
}
