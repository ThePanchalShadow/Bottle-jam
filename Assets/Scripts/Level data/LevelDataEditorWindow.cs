using UnityEngine;
using UnityEditor;

public class LevelDataEditorWindow : EditorWindow
{
    // Reference to the LevelDataSO scriptable object
    private LevelDataSO levelDataSO;

    // Fields to input level information
    private bool clearLevelData;
    private int numberOfLevelsToCreate = 5;

    private int minAmountOfColors = 2, maxAmountOfColors = 3;
    private int minNumberOfBoxHolderToSpawn = 4, maxNumberOfBoxHolderToSpawn = 4;
    private int minNumberOfCanBoxToSpawn = 4, maxNumberOfCanBoxToSpawn = 8;
    private int minNumberOfCanBoxRamp = 2, maxNumberOfCanBoxRamp = 4;
    private int minNumberOfCanRamp = 2, maxNumberOfCanRamp = 3;

    [MenuItem("Tools/Level Data Editor")]
    public static void ShowWindow()
    {
        // Create the window
        var window = GetWindow<LevelDataEditorWindow>("Level Data Editor");
        window.Show();
    }

    private void OnGUI()
    {
        // Field for selecting the LevelDataSO object
        levelDataSO = (LevelDataSO)EditorGUILayout.ObjectField("Level Data SO", levelDataSO, typeof(LevelDataSO), false);

        if (levelDataSO != null)
        {
            // Input fields for the level generation settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add Multiple Level", EditorStyles.boldLabel);

            clearLevelData = EditorGUILayout.Toggle("Clear Level Data?", clearLevelData);
            numberOfLevelsToCreate = EditorGUILayout.IntField("Levels to Create", numberOfLevelsToCreate);

            // Ranges for each parameter
            EditorGUILayout.Space();
            minAmountOfColors = EditorGUILayout.IntField("Min Colors - Min", minAmountOfColors);
            maxAmountOfColors = EditorGUILayout.IntField("Max Colors", maxAmountOfColors);

            //            minNumberOfBoxHolderToSpawn = EditorGUILayout.IntField("Min Box Holders", minNumberOfBoxHolderToSpawn);
            //            maxNumberOfBoxHolderToSpawn = EditorGUILayout.IntField("Max Box Holderas", maxNumberOfBoxHolderToSpawn);/

            EditorGUILayout.Space();
            minNumberOfCanBoxToSpawn = EditorGUILayout.IntField("Min Can Boxes", minNumberOfCanBoxToSpawn);
            maxNumberOfCanBoxToSpawn = EditorGUILayout.IntField("Max Can Boxes", maxNumberOfCanBoxToSpawn);

            EditorGUILayout.Space();
            minNumberOfCanRamp = EditorGUILayout.IntSlider("Min Can Ramps", minNumberOfCanRamp, 0, 5);
            maxNumberOfCanRamp = EditorGUILayout.IntSlider("Max Can Ramps", maxNumberOfCanRamp, 0, 5);

            EditorGUILayout.Space();
            minNumberOfCanBoxRamp = EditorGUILayout.IntSlider("Min Number of Can Box Ramps", minNumberOfCanBoxRamp, 0, 5);
            maxNumberOfCanBoxRamp = EditorGUILayout.IntSlider("Max Can Box Ramps", maxNumberOfCanBoxRamp, 0, 5);

            // Button to add levels to the list
            if (GUILayout.Button("Add Levels"))
            {
                AddLevelsToList();
            }

            // Display input fields for new level data values
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add Single Level", EditorStyles.boldLabel);

            // Input field for amount of colors
            minAmountOfColors = EditorGUILayout.IntField("Amount of Colors", minAmountOfColors);
            minNumberOfCanBoxToSpawn = EditorGUILayout.IntField("Number of Can Boxes", minNumberOfCanBoxToSpawn);

            // Using IntSlider to restrict input range for specific values
            minNumberOfCanRamp = EditorGUILayout.IntSlider("Number of Can Ramps", minNumberOfCanRamp, 0, 5);
            minNumberOfCanBoxRamp = EditorGUILayout.IntSlider("Number of Can Box Ramps", minNumberOfCanBoxRamp, 0, 5);

            // Add new level data when the button is clicked
            if (GUILayout.Button("Add New Level"))
            {
                AddNewLevelData();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a LevelDataSO ScriptableObject.", MessageType.Warning);
        }
    }

    private void AddLevelsToList()
    {
        // Clear existing levels before adding new ones
        if (clearLevelData) levelDataSO.levelData.Clear();

        // Calculate increments for each range
        float incrementAmountOfColors = (float)(maxAmountOfColors - minAmountOfColors) / (numberOfLevelsToCreate - 1);
        float incrementNumberOfBoxHolderToSpawn = (float)(maxNumberOfBoxHolderToSpawn - minNumberOfBoxHolderToSpawn) / (numberOfLevelsToCreate - 1);
        float incrementNumberOfCanBoxToSpawn = (float)(maxNumberOfCanBoxToSpawn - minNumberOfCanBoxToSpawn) / (numberOfLevelsToCreate - 1);
        float incrementNumberOfCanRamp = (float)(maxNumberOfCanRamp - minNumberOfCanRamp) / (numberOfLevelsToCreate - 1);
        float incrementNumberOfCanBoxRamp = (float)(maxNumberOfCanBoxRamp - minNumberOfCanBoxRamp) / (numberOfLevelsToCreate - 1);

        // Add levels gradually from min to max for each parameter
        for (int level = 0; level < numberOfLevelsToCreate; level++)
        {
            LevelData newLevelData = new()
            {
                amountOfColors = Mathf.RoundToInt(minAmountOfColors + incrementAmountOfColors * level),
                numberOfBoxHolderToSpawn = Mathf.RoundToInt(minNumberOfBoxHolderToSpawn + incrementNumberOfBoxHolderToSpawn * level),
                numberOfCanBoxToSpawn = Mathf.RoundToInt(minNumberOfCanBoxToSpawn + incrementNumberOfCanBoxToSpawn * level),
                numberOfCanRamp = Mathf.RoundToInt(minNumberOfCanRamp + incrementNumberOfCanRamp * level),
                numberOfCanBoxRamp = Mathf.RoundToInt(minNumberOfCanBoxRamp + incrementNumberOfCanBoxRamp * level),
            };

            levelDataSO.levelData.Add(newLevelData);
        }

        // Mark the scriptable object as dirty to save changes
        EditorUtility.SetDirty(levelDataSO);
    }

    // Method to add new level data using the input values
    private void AddNewLevelData()
    {
        LevelData newLevelData = new()
        {
            amountOfColors = Mathf.RoundToInt(minAmountOfColors),
            numberOfBoxHolderToSpawn = Mathf.RoundToInt(minNumberOfBoxHolderToSpawn),
            numberOfCanBoxToSpawn = Mathf.RoundToInt(minNumberOfCanBoxToSpawn),
            numberOfCanRamp = Mathf.RoundToInt(minNumberOfCanRamp),
            numberOfCanBoxRamp = Mathf.RoundToInt(minNumberOfCanBoxRamp),
        };

        levelDataSO.levelData.Add(newLevelData);

        EditorUtility.SetDirty(levelDataSO);
    }
}