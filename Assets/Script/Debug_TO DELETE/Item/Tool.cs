using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Tool : Vegetable_Scriptable
{
    [SerializeField] private int level;
    [SerializeField] private int capacity;
    [SerializeField] private int maxCapacity;
    [SerializeField] private int size;

    public int Level => level;
    public int Capacity => capacity;
    public int MaxCapacity => maxCapacity;
    public int Size => size;
}
