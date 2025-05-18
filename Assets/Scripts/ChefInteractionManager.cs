using UnityEngine;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.PlacementBar;

public class ChefInteractionManager : MonoBehaviour
{
    public enum PuzzlePhase { Intro, DishPhase, IngredientPhase, Complete }
    public PuzzlePhase currentPhase = PuzzlePhase.Intro;

    public AddableItemData[] dirtyPlateData;
    public GameObject[] cleanPlatePrefabs;
    public GameObject breakfastPrefab;

    public AddableItemData cheeseData;
    public AddableItemData eggData;
    public AddableItemData breadData;
    public AddableItemData breakfastData;

    public InventoryAdder inventoryManager;

    [Header("Animation")]
    private Animator chefAnimator; // ?? assign this in the Inspector

    [Header("Voice Lines")]
    public AudioSource audioSource;

    public AudioClip line_Intro;
    public AudioClip line_NotCleaned;
    public AudioClip line_GatherIngredients;
    public AudioClip line_MissingIngredients;
    public AudioClip line_BreakfastReady;
    public AudioClip line_Done;

    public bool isComplete;


    public void SetChefAnimator(Animator animator)
    {
        chefAnimator = animator;
    }

    public void OnTapped()
    {
        switch (currentPhase)
        {
            case PuzzlePhase.Intro:
                TriggerAnimation("Talk");
                Say("Ah! My dishes are dirty. Please clean them for me.");
                AddDirtyPlatesToInventory();
                currentPhase = PuzzlePhase.DishPhase;
                break;

            case PuzzlePhase.DishPhase:
                if (inventoryManager.HasItems(cleanPlatePrefabs, 1))
                {
                    TriggerAnimation("Talk");
                    Say("Wonderful! Now, collect my ingredients: cheese, egg, and bread.");
                    currentPhase = PuzzlePhase.IngredientPhase;
                }
                else
                {
                    Say("You haven't cleaned all my plates yet!");
                    TriggerAnimation("Wrong");
                }
                break;

            case PuzzlePhase.IngredientPhase:
                if (inventoryManager.HasItem(cheeseData.objectName) &&
                    inventoryManager.HasItem(eggData.objectName) &&
                    inventoryManager.HasItem(breadData.objectName))
                {
                    TriggerAnimation("Win");
                    Say("Perfect! Here's your breakfast.");
                    RemoveIngredients();
                    inventoryManager.AddToInventory(breakfastData);
                    currentPhase = PuzzlePhase.Complete;
                    isComplete = true;
                }
                else
                {
                    TriggerAnimation("Wrong");
                    Say("Some ingredients are still missing...");
                }
                break;

            case PuzzlePhase.Complete:
                TriggerAnimation("Talk");
                Say("Bon appétit! My job here is done.");
                isComplete = true;
                break;
        }
    }

    void Say(string text)
    {
        Debug.Log("[Chef]: " + text);

        if (audioSource == null)
        {
            Debug.LogWarning("? AudioSource not assigned.");
            return;
        }

        // Play the corresponding audio
        if (text.Contains("Ah! My dishes are dirty"))
        {
            audioSource.PlayOneShot(line_Intro);
        }
        else if (text.Contains("You haven't cleaned all my plates"))
        {
            audioSource.PlayOneShot(line_NotCleaned);
        }
        else if (text.Contains("collect my ingredients"))
        {
            audioSource.PlayOneShot(line_GatherIngredients);
        }
        else if (text.Contains("Some ingredients are still missing"))
        {
            audioSource.PlayOneShot(line_MissingIngredients);
        }
        else if (text.Contains("Perfect! Here's your breakfast"))
        {
            audioSource.PlayOneShot(line_BreakfastReady);
        }
        else if (text.Contains("Bon appétit"))
        {
            audioSource.PlayOneShot(line_Done);
        }
    }


    void TriggerAnimation(string triggerName)
    {
        if (chefAnimator != null)
        {
            chefAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("Chef Animator not assigned!");
        }
    }

    void AddDirtyPlatesToInventory()
    {
        foreach (var plateData in dirtyPlateData)
        {
            inventoryManager.AddToInventory(plateData);
        }
    }

    void RemoveIngredients()
    {
        inventoryManager.RemoveItemByName(cheeseData.objectName);
        inventoryManager.RemoveItemByName(eggData.objectName);
        inventoryManager.RemoveItemByName(breadData.objectName);
    }
}
