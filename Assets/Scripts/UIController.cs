using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerShooting playerShooting;
    private MouseLook mouseLook;
    private TMP_Text MaxAmmo;
    private TMP_Text CurrentAmmo;
    private TMP_Text Health;
    private GameObject pauseMenu;
    private GameObject optionsMenu;
    private GameObject player;
    bool paused;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        playerController = player.GetComponent<PlayerController>();
        playerShooting = player.GetComponent<PlayerShooting>();
        mouseLook = FindObjectOfType<MouseLook>();
        MaxAmmo = transform.GetChild(0).GetComponent<TMP_Text>();
        CurrentAmmo = transform.GetChild(1).GetComponent<TMP_Text>();
        Health = transform.GetChild(2).GetComponent<TMP_Text>();
        pauseMenu = transform.GetChild(4).gameObject;
        optionsMenu = transform.GetChild(5).gameObject;

        Health.text = "Health: " + playerController.currentHealth.ToString();
        MaxAmmo.text = playerShooting.MaxAmmo.ToString();
        CurrentAmmo.text = playerShooting.CurrentMagSize.ToString();
        paused = false;


    }

    // Update is called once per frame
    void Update()
    {
        Health.text = "Health: " + playerController.currentHealth.ToString();
        MaxAmmo.text = playerShooting.MaxAmmo.ToString();
        CurrentAmmo.text = playerShooting.CurrentMagSize.ToString() + "/";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
                
            }
            else
            {
                PauseMenu();
            }
            
        }
    }

    public void PauseMenu()
    {

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        paused = true;

    }
    public void Resume()
    {

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        paused = false;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SliderChange()
    {
        float sens = optionsMenu.GetComponentInChildren<Slider>().value;
        mouseLook.mouseSens = sens;
    }
}
