using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Game_Play;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //0.1f for fast gameplay
    //0.3f for good feel to show clearly how cans are moving
    // public const float GameLoopDuration = 0.5f;
    private static bool _isGameComplete;

    [FormerlySerializedAs("levelDataSO")] [Header("Level Settings")]
    public LevelDataSO levelDataSo;

    private int numberOfBoxHolderToSpawn;
    private int numberOfCanBoxToSpawn;
    private int numberOfCanRamp;
    private int numberOfCanBoxRamp;

    [Space] [Header("BoxHolder Data")] [SerializeField]
    private BoxHolder boxHolderPrefab;

    public Transform boxHolderPosition;
    public List<BoxHolder> spawnedBoxHolders = new();

    [Space] [Header("Box Data")] [SerializeField]
    private List<CanBox> canBoxPrefabs; // List of prefabs, one for each color

    public List<CanBox> spawnedCanBoxes = new();

    [Space] [Header("Can Data")] [SerializeField]
    private List<Can> canPrefabs; // List of prefabs, one for each color

    [Space] [Header("Can Ramp Data")] public Transform canRampPosition;
    [SerializeField] private CanRamp canRampPrefab;
    public List<CanRamp> spawnedCanRamps = new();

    [Space] [Header("Can Box Ramp Data")] public Transform canBoxRampPosition;
    [SerializeField] private CanBoxRamp canBoxRampPrefab;
    public List<CanBoxRamp> spawnedCanBoxRamps = new();

    public static event Action OnWin;
    public static event Action OnGameOver;
    public static event Action OnGameCompleted;

    private LevelData currentLevelData;

    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            LogWarning($"Instance of {this} already exists so destroying this object");
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }

    #endregion

    #region Level Setup

    private void Start()
    {
        canBoxPrefabs = levelDataSo.canBoxPrefabs;
        canPrefabs = levelDataSo.canPrefabs;
    }

    public void SetLevelData(LevelData levelData)
    {
        // Assign values from level data to individual variables
        currentLevelData = levelData;

        // Assign individual values to the GameManager's variables
        numberOfBoxHolderToSpawn = currentLevelData.numberOfBoxHolderToSpawn;
        numberOfCanBoxToSpawn = currentLevelData.numberOfCanBoxToSpawn;
        numberOfCanRamp = currentLevelData.numberOfCanRamp;
        numberOfCanBoxRamp = currentLevelData.numberOfCanBoxRamp;

        // Clear previously spawned objects
        ClearScene();
    }

    private void InitializeLevel()
    {
        CreateBoxHolders();
        CreateCanBoxRamps();
        _isGameComplete = false;
    }

    private void ClearScene()
    {
        // Destroy all previously spawned objects
        foreach (var holder in spawnedBoxHolders)
        {
            holder.transform.DOKill();
            Destroy(holder.gameObject);
        }

        spawnedBoxHolders.Clear();

        foreach (var ramp in spawnedCanRamps)
        {
            ramp.transform.DOKill();
            Destroy(ramp.gameObject);
        }

        spawnedCanRamps.Clear();

        foreach (var box in spawnedCanBoxes.Where(box => box))
        {
            box.transform.DOKill();
            Destroy(box.gameObject);
        }

        spawnedCanBoxes.Clear();

        foreach (var canBoxRamp in spawnedCanBoxRamps)
        {
            canBoxRamp.transform.DOKill();
            Destroy(canBoxRamp.gameObject);
        }

        spawnedCanBoxRamps.Clear();

        InitializeLevel();
    }

    #endregion

    #region Creation Methods

    // Create and spawn BoxHolders
    private void CreateBoxHolders()
    {
        var offset = CalculateOffset(numberOfBoxHolderToSpawn);

        for (var i = 0; i < numberOfBoxHolderToSpawn; i++)
        {
            // Instantiate and set boxHolderPosition as the parent
            var holder = Instantiate(
                boxHolderPrefab,
                boxHolderPosition.position + new Vector3(offset, 0, 0),
                Quaternion.identity,
                boxHolderPosition);
            offset += 2;
            holder.transform.localScale = Vector3.zero;
            spawnedBoxHolders.Add(holder);
        }

        var originalScale = boxHolderPrefab.transform.localScale;
        _ = AnimateBoxHolders(spawnedBoxHolders, originalScale);
    }

    private static async Task AnimateBoxHolders(List<BoxHolder> spawnedBoxHolders, Vector3 originalScale)
    {
        foreach (var holder in spawnedBoxHolders)
        {
            holder.transform.localScale = Vector3.zero;
            await holder.transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }
    }

    // Create and spawn CanBoxRamps, then create CanBoxes
    private void CreateCanBoxRamps()
    {
        var offset = CalculateOffset(numberOfCanBoxRamp);

        for (var i = 0; i < numberOfCanBoxRamp; i++)
        {
            // Calculate position and instantiate with canBoxRampPosition as the parent
            var ramp = Instantiate(
                canBoxRampPrefab,
                canBoxRampPosition.position + new Vector3(offset, 0, 0),
                Quaternion.identity,
                canBoxRampPosition
            );
            offset += 2;
            spawnedCanBoxRamps.Add(ramp);
        }

        CreateCanRamps();
    }


    // Create and spawn CanRamps
    private void CreateCanRamps()
    {
        var offset = CalculateOffset(numberOfCanRamp);
        for (var i = 0; i < numberOfCanRamp; i++)
        {
            // Calculate position and instantiate with canRampPosition as the parent
            var ramp = Instantiate(
                canRampPrefab,
                canRampPosition.position + new Vector3(offset, 0, 0),
                Quaternion.identity,
                canRampPosition
            );
            offset += 2;
            spawnedCanRamps.Add(ramp);
        }

        CreateCanBoxes();
    }

    private static float CalculateOffset(int numberOfObjects)
    {
        float offset;
        var objects = numberOfObjects / 2;
        if (numberOfObjects % 2 == 0)
        {
            var temp = Mathf.Ceil(objects);
            offset = -temp * 2 + 1;
        }
        else
        {
            var temp = Mathf.Floor(objects);
            offset = -temp * 2;
        }

        return offset;
    }

    // Create and distribute CanBoxes among CanBoxRamps
    private void CreateCanBoxes()
    {
        var colorList = new List<ColorType>((Enum.GetValues(typeof(ColorType)) as ColorType[])!);

        // Create a list to hold the selected random colors
        var selectedColors = new List<ColorType>();

        // Use a for loop to select the required amount of colors
        for (var i = 0; i < currentLevelData.amountOfColors; i++)
        {
            var randomIndex = Random.Range(0, colorList.Count);
            var selectedColor = colorList[randomIndex];
            selectedColors.Add(selectedColor);

            // Remove the selected color to avoid duplicates
            colorList.RemoveAt(randomIndex);
        }

        // filter CanBox prefabs that match the selected colors
        var filteredCanBoxes = canBoxPrefabs
            .Where(prefab => selectedColors.Contains(prefab.GetComponent<CanBox>().color))
            .ToList();

        CanBox lastSpawnedPrefab = null;

        for (var i = 0; i < numberOfCanBoxToSpawn; i++)
        {
            var availablePrefabs = filteredCanBoxes
                .Where(prefab => prefab != lastSpawnedPrefab)
                .ToList();

            // Randomly pick from available prefabs
            var randomIndex = Random.Range(0, availablePrefabs.Count);
            var boxPrefab = availablePrefabs[randomIndex];

            // Update last spawned prefab
            lastSpawnedPrefab = boxPrefab;

            // Instantiate and assign
            var box = Instantiate(boxPrefab, Vector3.zero, Quaternion.identity);
            spawnedCanBoxes.Add(box);

            // Distribute across ramps
            var rampIndex = i % spawnedCanBoxRamps.Count;
            var spawnedCanBoxRamp = spawnedCanBoxRamps[rampIndex];
            spawnedCanBoxRamp.assignedBoxCans.Add(box);
            box.transform.SetParent(spawnedCanBoxRamp.transform);
        }


        foreach (var ramp in spawnedCanBoxRamps) ramp.FirstPositionAssign();

        CreateCans();
    }


    // Create and distribute Cans among CanRamps
    private void CreateCans()
    {
        foreach (var box in spawnedCanBoxes)
            for (var i = 0; i < 4; i++) // Each box spawns 4 cans
            {
                // Find the prefab that matches the box's color
                var canPrefab = canPrefabs.Find(prefab => prefab.color == box.color);

                if (canPrefab == null)
                {
                    LogError($"No prefab found for color {box.color}");
                    continue;
                }

                // Instantiate the can from the prefab
                var can = Instantiate(canPrefab, Vector3.zero, Quaternion.identity);
                var rampIndex = (spawnedCanBoxes.IndexOf(box) * 4 + i) % spawnedCanRamps.Count;

                // Assign to a ramp
                var spawnedCanRamp = spawnedCanRamps[rampIndex];
                spawnedCanRamp.assignedCans.Add(can);
                can.transform.SetParent(spawnedCanRamp.transform);
            }

        foreach (var ramp in spawnedCanRamps) ramp.FirstPositionsAssign();
    }

    #endregion

    #region MyRegion

    // private void Update()
    // {
    //     Log("<color=blue>--------------------Update Game Over------------------------</color>");
    //
    //     foreach (var holder in spawnedBoxHolders)
    //     {
    //         LogWarning($"No holder found for {holder.name}");
    //         Log($"holder.IsOccupied() {holder.IsOccupied()}\n");
    //         Log($"BoxHolderState {holder.boxHolderState}\n");
    //         
    //         if(!holder.HeldCanBox) return;
    //         Log($"CanBox.canCollected.Count < 4 {holder.HeldCanBox.CanCollected.Count < 4}" +
    //             $"Can Count is {holder.HeldCanBox.CanCollected.Count}\n");
    //     }
    //     foreach (var ramp in spawnedCanRamps)
    //     {
    //         Log($"{ramp} is animating {ramp.IsMovingForward}");
    //     }
    // }

    #endregion
    
    #region Game Logic

    // Assign a CanBox to an available BoxHolder
    public bool AssignBoxToHolder(CanBox box)
    {
        var assigned = false;
        //StopCoroutine(_gameLoopCoroutine); // Pause game loop while assigning
        foreach (var holder in spawnedBoxHolders.Where(holder => !holder.IsOccupied()))
        {
            holder.AttachCanBox(box);
            box.AssignTheHolder(holder);
            assigned = true;
            break;
        }

        //_gameLoopCoroutine = StartCoroutine(GameLoopCoroutine()); // Resume game loop
        return assigned;
    }

    // Check for Win Condition
    public bool CheckWinCondition()
    {
        if(_isGameComplete) return false;
        
        var allRampsEmpty = spawnedCanRamps.TrueForAll(ramp => ramp.assignedCans.Count == 0);
        var isLastBox = spawnedCanBoxes.Count <= 0;
        var areAllBoxesAnimated = spawnedCanBoxes.TrueForAll(canBox => canBox.removeAnimationFinished);
        Log($"allRampsEmpty {allRampsEmpty}\nareAllBoxesAnimated {areAllBoxesAnimated}\nisLastBox {isLastBox}");
        if (!allRampsEmpty || !isLastBox || !areAllBoxesAnimated)
        {
            _ = CheckAllRampColor();
            return false;
        }
        Log("Game Complete and Won");
        
        OnWin?.Invoke();
        _isGameComplete = true;
        
        return true;
    }

    // Trigger Game Over
   // ReSharper disable once AsyncVoidMethod
    private void GameOver()
    {
        if(_isGameComplete) return;

        var allHoldersOccupied = spawnedBoxHolders.TrueForAll(holder =>
            holder.IsOccupied() && holder.BoxHolderState == BoxHolderState.HoldingBox && holder.HeldCanBox.CanCollected.Count < 4)
            && spawnedCanRamps.TrueForAll(ramp => !ramp.IsMovingForward);
            /*!holder.isAssigning && !holder.isDetachingBox*/
        
        Log("<color=blue>--------------------New Game Over------------------------</color>");
        
        foreach (var holder in spawnedBoxHolders)
        {
            if (!holder)
            {
                LogError($"No holder found for {holder.name}");
                continue;
            }


            Log($"allHoldersOccupied {allHoldersOccupied}\n");
            Log($"holder.IsOccupied() {holder.IsOccupied()}\n");
            Log($"BoxHolderState {holder.BoxHolderState}\n");
            
            if(!holder.HeldCanBox) return;
            Log($"CanBox.canCollected.Count < 4 {holder.HeldCanBox.CanCollected.Count < 4}" +
                $"Can Count is {holder.HeldCanBox.CanCollected.Count}\n");
        }

        foreach (var ramp in spawnedCanRamps)
        {
            Log($"{ramp} is animating {ramp.IsMovingForward}");
        }
        if (!allHoldersOccupied) return;
        Log("<b><color=red>Game OVER</color></b>");
        OnGameOver?.Invoke();
        _isGameComplete = true;
    }

    public async Task CheckAllRampColor()
    {
        var matchFound = false;
        var count = 0;
        
        foreach (var firstCan in spawnedCanRamps
                     .Select(ramp => ramp.GetFirstCan())
                     .Where(firstCan => firstCan != null))
        {
            count++;
            Log($"check color of {firstCan} from ramp {count}");
            
            if (!firstCan.CheckColorMatch(out var canBox)) continue;
            
            // canBox.AssignCan(firstCan);
            matchFound = true;
            await Task.Delay(100);
        }
        
        if (!matchFound) GameOver();
    }

    public static void GameCompleted()
    {
        OnGameCompleted?.Invoke();
    }

    #endregion
    
    private void Log(object message)
    {
        Debug.Log($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
    }
    private void LogWarning(object message)
    {
        Debug.LogWarning($"<b><color=yellow>{gameObject.name}</color></b>  {message}", gameObject);
    }
    private void LogError(object message)
    {
        Debug.LogError($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
    }
    
    #region Coroutines

    //bool _matchFound = false;
    //// Main game loop
    //private IEnumerator GameLoopCoroutine()
    //{
    //    Debug.Log("game loop started");
    //    while (true)
    //    {
    //        _matchFound = false;

    //        yield return new WaitForSeconds(gameLoopDuration);

    //        // Loop through all can ramps and check each available can
    //        foreach (CanRamp ramp in spawnedCanRamps)
    //        {
    //            if (ramp.currentAvailableCan && ramp.currentAvailableCan.CheckColorMatch())
    //            {
    //                _matchFound = true;
    //            }
    //        }


    //        if (!_matchFound)
    //        {
    //            bool allHoldersOccupied = spawnedBoxHolders.TrueForAll(holder => holder.IsOccupied() && !holder.isAnimating);
    //            if (allHoldersOccupied)
    //            {
    //                yield return new WaitForSeconds(0.5f);
    //                GameOver();
    //                Debug.Log("Player loose");
    //                _gameLoopCoroutine = null;
    //                yield break;
    //            }
    //        }

    //        if (CheckWinCondition())
    //        {
    //            Debug.Log("Player won");
    //            _gameLoopCoroutine = null;
    //            yield break;
    //        }
    //    }
    //}

    #endregion
}