using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
{
    private SerializedProperty levelData;
    private SerializedProperty canBoxPrefabs;
    private SerializedProperty canPrefabs;

    // Foldout states
    private bool levelDataFoldout = true;

    // Input fields for new level data
    private int newAmountOfColors = 0;  // New field for amount of colors
    private int newNumberOfBoxHolderToSpawn = 0;
    private int newNumberOfCanBoxToSpawn = 0;
    private int newNumberOfCanRamp = 0;
    private int newNumberOfCanBoxRamp = 0;

    private void OnEnable()
    {
        // Cache references to the serialized properties
        canBoxPrefabs = serializedObject.FindProperty("canBoxPrefabs");
        canPrefabs = serializedObject.FindProperty("canPrefabs");
        levelData = serializedObject.FindProperty("levelData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Can Box Prefabs Section
        EditorGUILayout.LabelField("Can Box Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canBoxPrefabs, new GUIContent("Can Box Prefabs"));

        // Can Prefabs Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Can Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canPrefabs, new GUIContent("Can Prefabs"));

        // Level Data Section with Foldout
        EditorGUILayout.Space();
        levelDataFoldout = EditorGUILayout.Foldout(levelDataFoldout, "Level Data Settings", true);
        if (levelDataFoldout)
        {
            EditorGUILayout.PropertyField(levelData, new GUIContent("Level Data"));

            // Display input fields for new level data values
            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("New Level Data", EditorStyles.boldLabel);

            //// Input field for amount of colors
            //newAmountOfColors = EditorGUILayout.IntField("Amount of Colors", newAmountOfColors);
            //newNumberOfCanBoxToSpawn = EditorGUILayout.IntField("Number of Can Boxes", newNumberOfCanBoxToSpawn);

            //// Using IntSlider to restrict input range for specific values
            //newNumberOfBoxHolderToSpawn = EditorGUILayout.IntSlider("Number of Box Holders", newNumberOfBoxHolderToSpawn, 0, 5);
            //newNumberOfCanRamp = EditorGUILayout.IntSlider("Number of Can Ramps", newNumberOfCanRamp, 0, 5);
            //newNumberOfCanBoxRamp = EditorGUILayout.IntSlider("Number of Can Box Ramps", newNumberOfCanBoxRamp, 0, 5);

            //// Add new level data when the button is clicked
            //if (GUILayout.Button("Add New Level", GUILayout.Height(30)))
            //{
            //    AddNewLevelData();
            //}
        }

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }


    // Method to add new level data using the input values
    private void AddNewLevelData()
    {
        levelData.arraySize++;
        SerializedProperty newItem = levelData.GetArrayElementAtIndex(levelData.arraySize - 1);

        // Assign values from the input fields to the new element
        newItem.FindPropertyRelative("amountOfColors").intValue = newAmountOfColors;  // Assign amountOfColors
        newItem.FindPropertyRelative("numberOfBoxHolderToSpawn").intValue = newNumberOfBoxHolderToSpawn;
        newItem.FindPropertyRelative("numberOfCanBoxToSpawn").intValue = newNumberOfCanBoxToSpawn;
        newItem.FindPropertyRelative("numberOfCanRamp").intValue = newNumberOfCanRamp;
        newItem.FindPropertyRelative("numberOfCanBoxRamp").intValue = newNumberOfCanBoxRamp;

        // Optionally, reset the input fields if needed after adding the new level
        newAmountOfColors = 0;  // Reset the newAmountOfColors
        newNumberOfBoxHolderToSpawn = 0;
        newNumberOfCanBoxToSpawn = 0;
        newNumberOfCanRamp = 0;
        newNumberOfCanBoxRamp = 0;
    }
}