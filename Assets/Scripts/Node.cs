﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;


    public Node(bool _walkable, Vector3 _wp)
    {
        walkable = _walkable;
        worldPosition = _wp;
    }
}
