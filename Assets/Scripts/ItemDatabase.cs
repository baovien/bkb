﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public List<Item> items = new List<Item>();

	void Start()
	{
		items.Add(new Item("Stone", 0, "Just stone", Item.ItemType.Resource, true));
		items.Add(new Item("Wood", 1, "Just wood", Item.ItemType.Resource, true));
		items.Add(new Item("Meat", 2, "Just meat", Item.ItemType.Consumable, true));
	}
}
