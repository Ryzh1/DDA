using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 20;
    public float sprintSpeed;
    private float currentSpeed;
    private CharacterController controller;
    private Animation bob;

    public float jumpSpeed = 8;
    public float gravity = -9.81f;
    Vector3 velocity;


    private void Start()
    {
        bob = GetComponentInChildren<Animation>();
        controller = GetComponent<CharacterController>();
        
    }

    void Update()
    {
        Movement();
        Jump(); 
       
    }

    private void Jump()
    {
        velocity.y += gravity * Time.deltaTime;

        if (Physics.Raycast(transform.position, -transform.up, 1.2f) && velocity.y < 0)
        {
            velocity.y = -2f;
            if (Input.GetKeyDown("space"))
            {
                velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }
            
        }

        controller.Move(velocity * Time.deltaTime);
        
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(z > 0.1)
            {
                currentSpeed = sprintSpeed;
                bob.Play();
            }
        }
        else
        {
            currentSpeed = speed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);
    }



}
