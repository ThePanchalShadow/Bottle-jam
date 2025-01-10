using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    private LevelDataSO levelDataSO;
    public int currentLevelIndex;

    [Space]
    [Header("UI Elements")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button resetButton;

    private GameManager gameManager;

    public static event System.Action<int> OnLevelChanged;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0); // Load saved level
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        levelDataSO = gameManager.levelDataSo;
        LoadLevel(currentLevelIndex);

        retryButton.onClick.AddListener(RetryLevel);
        continueButton.onClick.AddListener(LoadNextLevel);
        resetButton.onClick.AddListener(ResetLevels);
    }

    private void LoadLevel(int levelIndex)
    {
        // Ensure index is within bounds
        if (levelIndex >= 0 && levelIndex < levelDataSO.levelData.Count)
        {
            currentLevelIndex = levelIndex;
            PlayerPrefs.SetInt("CurrentLevel", levelIndex); // Save current level
            PlayerPrefs.Save();
            gameManager.SetLevelData(levelDataSO.levelData[currentLevelIndex]);

            OnLevelChanged?.Invoke(currentLevelIndex);
        }
        else
        {
            Debug.LogWarning("Level index out of bounds.");
        }
    }

    private void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levelDataSO.levelData.Count)
        {
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("All levels completed!");
            GameManager.GameCompleted();
        }
    }

    private void RetryLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    private void ResetLevels()
    {
        currentLevelIndex = 19;
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex); // Reset level to 0
        PlayerPrefs.Save();
        LoadLevel(currentLevelIndex);
    }
}
