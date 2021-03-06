﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Animator anim;
    private Rigidbody2D myRigidBody2D;
    private Inventory inventory;
    private static bool playerExists;
    private float attackTimeCounter;
    private static float moveSpeedDefault;

    public float moveSpeed;
    public float moveSpeedMultiplier;
    public float attackTime;
    public string startPoint;

    // Use this for initialization
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        anim = GetComponent<Animator>();
        myRigidBody2D = GetComponent<Rigidbody2D>();
        moveSpeedDefault = moveSpeed;

        if (!playerExists)
        {
            playerExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
       
        //Player movement
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (moveInput != Vector2.zero)
        {
            myRigidBody2D.velocity = new Vector2(moveInput.x * moveSpeedDefault, moveInput.y * moveSpeedDefault);
        }
        else
        {
            myRigidBody2D.velocity = Vector2.zero;
        }
        
        //Attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            attackTimeCounter = attackTime;
            anim.SetBool("Attack", true);
        }
        
        //Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeedDefault = moveSpeed * moveSpeedMultiplier;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeedDefault = moveSpeed;
        }
        
        //Attack time counter
        if (attackTimeCounter > 0)
        {
            attackTimeCounter -= Time.deltaTime;
        }
        if (attackTimeCounter <= 0)
        {
            anim.SetBool("Attack", false);
        }
    }
    
//TODO: Rewrite this to a general solution, additionally layers
    /// <summary>
    /// Pick up item and add to inventory
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerHand"))
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Wood"))
        {
            inventory.AddItem(2);
            Destroy(other.gameObject);

        } else if (other.gameObject.CompareTag("Stone"))
        {
            inventory.AddItem(1);
            Destroy(other.gameObject);

        } else if (other.gameObject.CompareTag("Pickup"))
        {
            inventory.AddItem(3);
            Destroy(other.gameObject);
            
        } else if (other.gameObject.CompareTag("Meat"))
        {
            inventory.AddItem(0);
            Destroy(other.gameObject);
        }
    }
}
