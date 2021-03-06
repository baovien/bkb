﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildSystem : MonoBehaviour
{
    // Variables to hold data (Current block)
    private int currentBlockID = 0;
    private Item currentBlock;

    // The amount of blocks that are selectable by the player
    private int selectableBlocksTotal;

    // Block template
    private GameObject blockTemplate;
    private SpriteRenderer currentRend;

    // Bools to control the building system
    private bool buildModeOn;
    private bool buildBlocked;

    // float to adjust size of blocks 
    [SerializeField]
    private float blockSizedMod;

    // Layer masks
    [SerializeField]
    private LayerMask solidNoBuildLayer;
    [SerializeField]
    private LayerMask backingNoBuildLayer;
    [SerializeField]
    private LayerMask allBlocksLayer;

    // Player object reference
    private GameObject playerObject;
    private Inventory inventory;
    private List<Item> blockList;

    [SerializeField]
    private float buildDistance;


    private void Start()
    {
        // Get player
        playerObject = GameObject.Find("Player");
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        //blockList = database.items;
        blockList = (from item in inventory.inventory where item.itemType == Item.ItemType.Block select item).ToList();
        if (blockList.Count < 1) Debug.Log("Nothing can be placed, Inventory doesn't contain a block");

    }

    private void Update()
    {
        blockList = (from item in inventory.inventory where item.itemType == Item.ItemType.Block select item).ToList();
        // If <key> pressed, toggle build mode
        if (Input.GetKeyDown("e"))
        {
            Debug.Log(blockList.Count);
            buildModeOn = !buildModeOn;

            if (blockTemplate != null)
            {
                Destroy(blockTemplate);
            }

            // No current block type set:
            if (currentBlock == null)
            {
                // ensure allBlocks array is ready
                if (blockList[currentBlockID] != null)
                {
                    // Get a new currentBlock using ID
                    currentBlock = blockList[currentBlockID];
                }
            }

            if (buildModeOn)
            {
                // Create a new object for blockTemplate
                blockTemplate = new GameObject("CurrentBlockTemplate");
                // Add and store reference to a SpriteRenderer on the template objec
                currentRend = blockTemplate.AddComponent<SpriteRenderer>();
                // Set the sprite of the template object to match the current block type
                currentRend.sprite = Sprite.Create(currentBlock.itemIcon, new Rect(0, 0, currentBlock.itemIcon.width, currentBlock.itemIcon.height), new Vector2(0.5f, 0.5f));
            }
        }

        if (buildModeOn && blockTemplate != null)
        {
            float newPosX = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / blockSizedMod) * blockSizedMod;
            float newPosY = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / blockSizedMod) * blockSizedMod;
            blockTemplate.transform.position = new Vector2(newPosX, newPosY);
            currentRend.sortingOrder = 2;

            RaycastHit2D rayHit;
            if (currentBlock.isSolid)
            {
                rayHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, solidNoBuildLayer);
            }
            else
            {
                rayHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, backingNoBuildLayer);
            }

            if (rayHit.collider != null)
            {
                buildBlocked = false;
            }
            if (Vector2.Distance(playerObject.transform.position, blockTemplate.transform.position) > buildDistance || Vector2.Distance(playerObject.transform.position, blockTemplate.transform.position) < playerObject.GetComponent<Renderer>().bounds.size.y)
            {
                buildBlocked = true;
            }

            if (buildBlocked)
            {
                currentRend.color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                currentRend.color = new Color(1f, 1f, 1f, 1f);
            }
            // Using scrollwheel to traverse the list of blocks. 
            float mousewheel = Input.GetAxis("Mouse ScrollWheel");
            if (mousewheel != 0 && blockList.Count > 0)
            {
                selectableBlocksTotal = blockList.Count - 1;

                if (mousewheel > 0)
                {
                    currentBlockID--;
                    if (currentBlockID < 0)
                    {
                        currentBlockID = selectableBlocksTotal;
                    }
                }
                else if (mousewheel < 0)
                {
                    currentBlockID++;

                    if (currentBlockID > selectableBlocksTotal)
                    {
                        currentBlockID = 0;
                    }
                }

                currentBlock = blockList[currentBlockID];
                currentRend.sprite = Sprite.Create(currentBlock.itemIcon, new Rect(0, 0, currentBlock.itemIcon.width, currentBlock.itemIcon.height), new Vector2(0.5f, 0.5f));
                currentBlock.itemQuantity = blockList[currentBlockID].itemQuantity;
            }

            // Placing block
            if (Input.GetMouseButtonDown(0) && buildBlocked == false && currentBlock.itemQuantity > 0)
            {
                GameObject newBlock = new GameObject(currentBlock.itemName);
                newBlock.transform.position = blockTemplate.transform.position;
                SpriteRenderer newRend = newBlock.AddComponent<SpriteRenderer>();
                newRend.sprite = Sprite.Create(currentBlock.itemIcon, new Rect(0, 0, currentBlock.itemIcon.width, currentBlock.itemIcon.height), new Vector2(0.5f, 0.5f));

                if (currentBlock.isSolid == true)
                {
                    // Need to remove it from inventory

                    
                    newBlock.AddComponent<BoxCollider2D>();
                    newBlock.layer = 9;
                    newRend.sortingOrder = 2;
                }
                else
                {
                    newBlock.AddComponent<BoxCollider2D>();
                    newBlock.layer = 10;
                    newRend.sortingOrder = 1;
                }

                for (int i = 0; i < inventory.inventory.Count; i++)
                {
                    if(inventory.inventory[i].itemID == currentBlock.itemID)
                    {
                        inventory.inventory[i].itemQuantity -= 1;
                        blockList.Remove(currentBlock);
                        break;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) && blockTemplate != null)
            {
                RaycastHit2D destroyHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, allBlocksLayer);

                if (destroyHit.collider != null)
                {
                    Destroy(destroyHit.collider.gameObject);

                    GameObject newPickup = new GameObject(blockList[currentBlockID].itemName, typeof(SpriteRenderer));
                    newPickup.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                    newPickup.GetComponent<SpriteRenderer>().sprite = Sprite.Create(blockList[currentBlockID].itemIcon, new Rect(0, 0, blockList[currentBlockID].itemIcon.width, blockList[currentBlockID].itemIcon.height), new Vector2(0.5f, 0.5f));
                    newPickup.AddComponent<BoxCollider2D>().isTrigger = true;
                    newPickup.tag = "Pickup";
                    newPickup.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    newPickup.transform.position = new Vector2(newPosX, newPosY);
                }
            }

        }
    }
}