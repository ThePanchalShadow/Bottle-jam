using System.Collections.Generic;
using Game_Play;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData")]
public class LevelDataSO : ScriptableObject
{
    [Tooltip("List of Can Box prefabs (one for each color)")]
    public List<CanBox> canBoxPrefabs;

    [Tooltip("List of Can prefabs (one for each color)")]
    public List<Can> canPrefabs;

    [Space]
    public List<LevelData> levelData;
}

[System.Serializable]
public struct LevelData
{
    [Header("Level Settings")]

    public int amountOfColors;
    [Range(0,5)]
    public int numberOfBoxHolderToSpawn;

    public int numberOfCanBoxToSpawn;

    [Range(0, 5)]
    public int numberOfCanRamp;
    [Range(0, 5)]
    public int numberOfCanBoxRamp;
}
