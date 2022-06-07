using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class CSVWriter : MonoBehaviour
{

    private string fileName;

    private GameObject director;
    private Director dir;
    LevelController levelController;
    private PlayerShooting playerShooting;


    public TMP_Text MaxAmmo;
    
    void Start()
    {
        playerShooting = FindObjectOfType<PlayerShooting>();
        director = GameObject.FindGameObjectWithTag("Director");
        if (director != null)
        {
            dir = director.GetComponent<Director>();
            fileName = Directory.GetCurrentDirectory() +"/Data Collected/DynamicData.csv";
        }
        else
        {
            fileName = Directory.GetCurrentDirectory() + "/Data Collected/LinearData.csv";
            levelController = FindObjectOfType<LevelController>();
            
        }
        
    }


    public void Write()
    {
        TextWriter tw = new StreamWriter(fileName, false);
        if (dir != null)
        {
            tw.WriteLine("Enemies Killed, Specials Killed, Ammo Used");
            tw.Close();

            tw = new StreamWriter(fileName, true);

            tw.WriteLine(dir.EnemiesKilled + "," + dir.SpecialsKilled + "," + playerShooting.MaxShots);
            tw.WriteLine("\n" + "Area" + "," + "Area Accuracy" + "," + "Enemies Killed" + "," + "Specials Killed" + "," + "PlayerHits");
            foreach (var item in dir.Areas)
            {
                tw.WriteLine(item.Area + "," + item.AreaAccuracy + "," + item.EnemiesKilled + "," + item.SpecialsKilled + "," + item.playerHit);
            }

        }
        else
        {
            tw.WriteLine("Enemies Killed, Specials Killed, Ammo Used");
            tw.Close();

            tw = new StreamWriter(fileName, true);
            tw.WriteLine(levelController.EnemiesKilled + "," + levelController.SpecialsKilled + "," + playerShooting.MaxShots);
            tw.WriteLine("\n" + "Area" + "," + "Area Accuracy" + "," + "PlayerHits");
            foreach (var item in levelController.Triggers)
            {
                tw.WriteLine(item.triggerIndex + "," + item.AreaAccuracy + "," + item.playerHit);
            }
        }
        tw.Close();

    }
}
