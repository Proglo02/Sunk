using System;
using UnityEngine;

[Serializable]
public enum BallType
{
    Solid,
    Striped,
    Cue,
    Ball8,
    None
}

[Serializable]
public struct BallData
{
    public int Number;
    public BallType BallType;
    public Material Material;
}

[Serializable]
[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObjects/Ball", order = 1)]
public class Ball : ScriptableObject
{
    public BallData Data;
}
