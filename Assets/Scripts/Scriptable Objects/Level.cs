using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Word", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimension")]
    public int width;
    public int height;

    [Header("Starting tiles")]
    public TileType[] boardLayout;

    [Header("Avaliable Dots")]
    public GameObject[] dots;

    [Header("Score Goals")]
    public int[] scoreGoals;
}
