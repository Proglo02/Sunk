using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BallData
{
    public int Number;
    public Material Material;
}

[Serializable]
[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObjects/Ball", order = 1)]
public class Ball : ScriptableObject
{
    public BallData Data;
}
