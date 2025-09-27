using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BallLayoutData
{
    public Ball Ball;
    public Vector3 Position;
    public Quaternion Rotation;
}

[CreateAssetMenu(fileName = "BallLayout", menuName = "ScriptableObjects/BallLayout", order = 2)]
public class BallLayout : ScriptableObject
{
    public List<BallLayoutData> Balls;
}
