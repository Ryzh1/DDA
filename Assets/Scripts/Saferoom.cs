using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Saferoom : MonoBehaviour
{

    private Door door;
    private bool inRoom;
    private CSVWriter cSVWriter;
    private bool end;
    
    // Start is called before the first frame update
    void Start()
    {
        inRoom = false;
        end = false;
        cSVWriter = GetComponent<CSVWriter>();
        door = GetComponentInChildren<Door>();
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            inRoom = true;
            if (!door.open && !end)
            {
                end = true;
                StartCoroutine(LoadMenu());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRoom = false;
        }
    }


    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(1f);
        if (inRoom)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cSVWriter.Write();
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("Menu");
            
            
        }
        
    }
}
