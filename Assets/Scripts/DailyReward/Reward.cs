using UnityEngine;
using System;

[Serializable]
public class Reward
{
    public string name;
    public int reward;
    public Sprite sprite;
    [Range(1,3)]
    public int icons;
}