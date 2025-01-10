// using System;
// using System.Collections.Generic;
// using System.Linq;
// using DG.Tweening;
// using Game_Play;
// using UnityEngine;
//
// public class LevelBuilder : MonoBehaviour
// {
//     [Header("BoxHolder Data")]
//     [SerializeField] private BoxHolder boxHolderPrefab;
//     [SerializeField] private Transform boxHolderPosition;
//
//     [Header("CanBox Data")]
//     [SerializeField] private List<CanBox> canBoxPrefabs;
//
//     [Header("Can Data")]
//     [SerializeField] private List<Can> canPrefabs;
//
//     [Header("Can Ramp Data")]
//     [SerializeField] private CanRamp canRampPrefab;
//     [SerializeField] private Transform canRampPosition;
//
//     [Header("Can Box Ramp Data")]
//     [SerializeField] private CanBoxRamp canBoxRampPrefab;
//     [SerializeField] private Transform canBoxRampPosition;
//
//     private LevelData currentLevelData;
//
//     private List<BoxHolder> spawnedBoxHolders = new();
//     private List<CanBox> spawnedCanBoxes = new();
//     private List<CanRamp> spawnedCanRamps = new();
//     private List<CanBoxRamp> spawnedCanBoxRamps = new();
//
//     public void SetLevelData(LevelData levelData)
//     {
//         currentLevelData = levelData;
//     }
//
//     public void InitializeLevel()
//     {
//         ClearScene();
//         CreateBoxHolders();
//         CreateCanBoxRamps();
//     }
//
//     public void ClearScene()
//     {
//         foreach (var holder in spawnedBoxHolders) DestroyObject(holder);
//         foreach (var box in spawnedCanBoxes) DestroyObject(box);
//         foreach (var ramp in spawnedCanRamps) DestroyObject(ramp);
//         foreach (var boxRamp in spawnedCanBoxRamps) DestroyObject(boxRamp);
//
//         spawnedBoxHolders.Clear();
//         spawnedCanBoxes.Clear();
//         spawnedCanRamps.Clear();
//         spawnedCanBoxRamps.Clear();
//     }
//
//     private void DestroyObject(Component obj)
//     {
//         if (obj == null) return;
//         obj.transform.DOKill();
//         Destroy(obj.gameObject);
//     }
//
//     private void CreateBoxHolders()
//     {
//         float offset = CalculateOffset(currentLevelData.numberOfBoxHolderToSpawn);
//
//         for (int i = 0; i < currentLevelData.numberOfBoxHolderToSpawn; i++)
//         {
//             var holder = Instantiate(boxHolderPrefab,
//                 boxHolderPosition.position + new Vector3(offset, 0, 0),
//                 Quaternion.identity,
//                 boxHolderPosition);
//             offset += 2;
//             holder.transform.localScale = Vector3.zero;
//             spawnedBoxHolders.Add(holder);
//         }
//
//         Vector3 originalScale = boxHolderPrefab.transform.localScale;
//         AnimateBoxHolders(spawnedBoxHolders, originalScale);
//     }
//
//     private async static void AnimateBoxHolders(List<BoxHolder> holders, Vector3 scale)
//     {
//         foreach (var holder in holders)
//         {
//             holder.transform.localScale = Vector3.zero;
//             await holder.transform.DOScale(scale, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
//         }
//     }
//
//     private void CreateCanBoxRamps()
//     {
//         float offset = CalculateOffset(currentLevelData.numberOfCanBoxRamp);
//
//         for (int i = 0; i < currentLevelData.numberOfCanBoxRamp; i++)
//         {
//             var ramp = Instantiate(canBoxRampPrefab,
//                 canBoxRampPosition.position + new Vector3(offset, 0, 0),
//                 Quaternion.identity,
//                 canBoxRampPosition);
//             offset += 2;
//             spawnedCanBoxRamps.Add(ramp);
//         }
//
//         CreateCanRamps();
//     }
//
//     private void CreateCanRamps()
//     {
//         float offset = CalculateOffset(currentLevelData.numberOfCanRamp);
//
//         for (int i = 0; i < currentLevelData.numberOfCanRamp; i++)
//         {
//             var ramp = Instantiate(canRampPrefab,
//                 canRampPosition.position + new Vector3(offset, 0, 0),
//                 Quaternion.identity,
//                 canRampPosition);
//             offset += 2;
//             spawnedCanRamps.Add(ramp);
//         }
//
//         CreateCanBoxes();
//     }
//
//     private void CreateCanBoxes()
//     {
//         var colorList = Enum.GetValues(typeof(ColorType)).Cast<ColorType>().ToList();
//         var selectedColors = new List<ColorType>();
//
//         for (int i = 0; i < currentLevelData.amountOfColors; i++)
//         {
//             if (colorList.Count == 0) break;
//
//             int randomIndex = UnityEngine.Random.Range(0, colorList.Count);
//             selectedColors.Add(colorList[randomIndex]);
//             colorList.RemoveAt(randomIndex);
//         }
//
//         var filteredCanBoxes = canBoxPrefabs
//             .Where(prefab => selectedColors.Contains(prefab.color))
//             .ToList();
//
//         for (int i = 0; i < currentLevelData.numberOfCanBoxToSpawn; i++)
//         {
//             var boxPrefab = filteredCanBoxes[UnityEngine.Random.Range(0, filteredCanBoxes.Count)];
//             var box = Instantiate(boxPrefab, Vector3.zero, Quaternion.identity);
//             spawnedCanBoxes.Add(box);
//
//             var rampIndex = i % spawnedCanBoxRamps.Count;
//             var assignedRamp = spawnedCanBoxRamps[rampIndex];
//             assignedRamp.assignedBoxCans.Add(box);
//             box.transform.SetParent(assignedRamp.transform);
//         }
//     }
//
//     private float CalculateOffset(int count)
//     {
//         return count % 2 == 0
//             ? -Mathf.Ceil(count / 2f) * 2 + 1
//             : -Mathf.Floor(count / 2f) * 2;
//     }
//
//     public bool AssignBoxToHolder(CanBox box)
//     {
//         foreach (var holder in spawnedBoxHolders)
//         {
//             if (!holder.IsOccupied())
//             {
//                 holder.AttachCanBox(box);
//                 box.AssignTheHolder(holder);
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     public bool CheckAllRampColor()
//     {
//         foreach (var ramp in spawnedCanRamps)
//         {
//             if (ramp.currentAvailableCan && ramp.currentAvailableCan.CheckColorMatch(out CanBox canBox))
//             {
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     public bool CheckWinCondition()
//     {
//         return spawnedCanRamps.All(ramp => ramp.assignedCans.Count == 0);
//     }
// }
