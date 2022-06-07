using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public bool open;
    private Quaternion endRotation;
    float rotationProgress = -1;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
    }


    private void Update()
    {
        if (rotationProgress < 1 && rotationProgress >= 0)
        {
            rotationProgress += Time.deltaTime * 0.2f;
            transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, rotationProgress);
        }
        
    }
    public void Move()
    {
        rotationProgress = 0;
        if (open)
        {
            open = false;
            endRotation.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            
        }
        else if (!open)
        {
            open = true;
            endRotation.eulerAngles = new Vector3(transform.rotation.x, -90, transform.rotation.z);
        }
    }
}
