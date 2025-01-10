using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Game Panels")]
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletedPanel;

    [Header("Level UI Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI levelNumberText;

    private void OnEnable()
    {
        GameManager.OnWin += ShowYouWinPanel;
        GameManager.OnGameOver += ShowGameOverPanel;
        GameManager.OnGameCompleted += ShowGameCompletedPanel;

        LevelManager.OnLevelChanged += UpdateLevelDisplay; // Listen to level change events
    }

    private void OnDisable()
    {
        GameManager.OnWin -= ShowYouWinPanel;
        GameManager.OnGameOver -= ShowGameOverPanel;
        GameManager.OnGameCompleted -= ShowGameCompletedPanel;

        LevelManager.OnLevelChanged -= UpdateLevelDisplay; // Unsubscribe to prevent memory leaks
    }

    private void ShowYouWinPanel()
    {
        HideAllPanels();
        ActivatePanel(youWinPanel);
    }

    private void ShowGameOverPanel()
    {
        HideAllPanels();
        ActivatePanel(gameOverPanel);
    }

    private void ShowGameCompletedPanel()
    {
        HideAllPanels();
        ActivatePanel(gameCompletedPanel);
    }

    private void HideAllPanels()
    {
        youWinPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameCompletedPanel.SetActive(false);
    }

    private void UpdateLevelDisplay(int levelIndex)
    {
        HideAllPanels();
        levelNumberText.text = $"Level {levelIndex + 1}";
    }

    private static void ActivatePanel(GameObject gameObject)
    {
        var transform = gameObject.transform;
        transform.localScale = Vector3.zero;
        transform.gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
}
