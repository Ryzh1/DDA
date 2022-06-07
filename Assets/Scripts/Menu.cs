using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private GameObject menu;
    private GameObject info;
    private GameObject info1;



    private void Start()
    {
        menu = transform.GetChild(0).transform.GetChild(0).gameObject;
        info = transform.GetChild(0).transform.GetChild(1).gameObject;
        info1 = transform.GetChild(0).transform.GetChild(2).gameObject;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void ShowInfo()
    {
        info.SetActive(true);
        menu.SetActive(false);
    }

    public void InfoPage2()
    {
        info.SetActive(false);
        info1.SetActive(true);
        menu.SetActive(false);
    }

    public void ShowMenu()
    {
        info1.SetActive(false);
        menu.SetActive(true);
    }

}
