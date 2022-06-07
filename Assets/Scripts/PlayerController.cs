using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private PlayerShooting playerShooting;
    public Camera fpsCam;
    private PlayerMovement playerMovement;
    private GameObject dir;
    private Director director;
    private LevelController levelController;
    private GameObject gunHolder;
    private List<GameObject> fpsGuns = new List<GameObject>();
    private CSVWriter cSVWriter;


    private float maxHealth = 100;
    public float currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }
    private float _currentHealth;
    public int currentArea
    {
        get { return _currentArea; }
        set { 
            if (value > _currentArea)
            {
                if(!director.Areas[value - 1].HasEntered)
                {
                    director.Areas[value - 1].HasEntered = true;
                    playerShooting.ResetAccuracy();
                }
                _currentArea = value;
                           
            }
            else
            {
                _currentArea = value;
            }
            }
    }
    private int _currentArea;

    public int medKitCount;
    public int stimCount;

    private GameObject medkit;
    private GameObject stim;

    private int currentItem;

    private bool dead;
    // Start is called before the first frame update
    void Start()
    {
        cSVWriter = GameObject.FindGameObjectWithTag("Saferoom").GetComponent<CSVWriter>();
        dir = GameObject.FindGameObjectWithTag("Director");
        if(dir != null)
        {
            director = dir.GetComponent<Director>();
        }
        else
        {
            levelController = FindObjectOfType<LevelController>();
        }

        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        medkit = transform.GetChild(2).gameObject;
        stim = transform.GetChild(3).gameObject;
        currentHealth = maxHealth;
        currentItem = 1;

        gunHolder = transform.GetChild(4).gameObject;
        for (int i = 0; i < gunHolder.transform.childCount; i++)
        {
            fpsGuns.Add(gunHolder.transform.GetChild(i).gameObject);
        }
        playerShooting.currentFpsGun = fpsGuns[0];
        playerShooting.MuzzleFlashChange();
    }

    // Update is called once per frame
    void Update()
    {
        ItemSelect();
        PickUp();
        MouseInput();

        if (currentHealth <= 0 && !dead)
        {
            dead = true;
            StartCoroutine(EndGame());
        }

        playerShooting.currentFpsGun.transform.eulerAngles = new Vector3(0, playerShooting.currentFpsGun.transform.eulerAngles.y, -fpsCam.transform.eulerAngles.x);
    }



    private void MouseInput()
    {
        if (Input.GetButtonDown("Fire1") && currentItem == 3 && medKitCount > 0 && currentHealth < 100)
        {
            currentHealth += 50;
            medKitCount -= 1;
            StartCoroutine(ItemDelay());

        }
        else if (Input.GetButtonDown("Fire1") && currentItem == 4 && stimCount > 0 && currentHealth < 100)
        {
            currentHealth += 25;
            stimCount -= 1;
            StartCoroutine(ItemDelay());
        }
    }

    private void ItemSelect()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(ItemDelay());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentItem = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && medKitCount > 0)
        {
            medkit.SetActive(true);
            stim.SetActive(false);
            playerShooting.currentFpsGun.SetActive(false);
            playerShooting.CanShoot = false;
            currentItem = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && stimCount > 0)
        {
            stim.SetActive(true);
            medkit.SetActive(false);
            playerShooting.currentFpsGun.SetActive(false);
            playerShooting.CanShoot = false;
            currentItem = 4;
        }


    }

    public void TakeDamage(int damage)
    {
        if(director != null)
        {
            director.PlayerHit++;
        }
        else
        {
            levelController.PlayerHit++;
        }
        playerMovement.speed = 2;
        playerMovement.sprintSpeed = 2;
        playerMovement.jumpSpeed = 0.4f;
        currentHealth -= damage;
        StartCoroutine(Stumble());

    }

    IEnumerator EndGame()
    {
        cSVWriter.Write();
        yield return new WaitForSeconds(0.5f);
        Destroy(this);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    IEnumerator Stumble()
    {
        yield return new WaitForSeconds(2f);
        playerMovement.speed = 4;
        playerMovement.sprintSpeed = 5;
        playerMovement.jumpSpeed = 0.6f;

    }


    void PickUp()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            int layermask1 = 1 << 6;
            int layermask2 = 1 << 8;
            int finalmask = layermask1 | layermask2;
            finalmask = ~finalmask;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 2f, finalmask))
            {
                
                if (hit.collider.CompareTag("Pickable"))
                {
                    
                    var name = hit.collider.gameObject.transform.GetChild(0).gameObject.name;
                    
                    if (name == "Medkit")
                    {
                        if (medKitCount == 0)
                        {
                            medKitCount++;
                            if(director != null)
                            {
                                director.CurrentMedkits--;
                            }                           
                            Destroy(hit.collider.gameObject);
                        }

      
                    }
                    else if (name == "Stim")
                    {
                        if (stimCount == 0)
                        {
                            if (director != null)
                            {
                                director.CurrentStims--;
                            }
                            stimCount++;
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else if (name == "AmmoPile")
                    {
                        if (director != null)
                        {
                            director.CurrentAmmoPiles--;
                        }
                        playerShooting.MaxAmmo += 150;
                        Destroy(hit.collider.gameObject);
                    }

                    else if (name != playerShooting.CurrentGun.weaponName)
                    {
                        var pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1, hit.collider.transform.position.z);

                        foreach (var gun in playerShooting.Guns)
                        {
                            if (gun.weaponName == name)
                            {
                                Instantiate(Resources.Load("Guns/" + playerShooting.CurrentGun.weaponName, typeof(GameObject)), pos, hit.collider.transform.rotation);
                                playerShooting.CurrentGun = gun;
                                
                            }
                        }

                        WeaponChange(name);

                        Destroy(hit.collider.gameObject);
                    }

                    
                }

                else if(hit.collider.CompareTag("Door"))
                {
                    var door = hit.collider.GetComponentInParent<Door>();                      
                    door.Move();
                }
            }
        }
    }

    private void WeaponChange(string name)
    {
        foreach (var item in fpsGuns)
        {
            if (item.name == name)
            {
                playerShooting.currentFpsGun = item;
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }

        playerShooting.MuzzleFlashChange();
    }

    IEnumerator ItemDelay()
    {
        yield return new WaitForSeconds(1f);
        currentItem = 1;
        medkit.SetActive(false);
        playerShooting.CanShoot = true;
        playerShooting.currentFpsGun.SetActive(true);
        stim.SetActive(false);
    }

}
