using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabsToSpawn; // One prefab per image name
    private ARTrackedImageManager trackedImageManager;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private HashSet<string> spawnedOnce = new HashSet<string>();

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            TrySpawnOnce(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            TrySpawnOnce(trackedImage);
        }
    }

    private void TrySpawnOnce(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null)
            return;

        string imageName = trackedImage.referenceImage.name;
        if (spawnedOnce.Contains(imageName)) return;

        GameObject prefab = prefabsToSpawn.Find(p => p != null && p.name == imageName);
        if (prefab == null)
        {
            Debug.LogWarning($"? No prefab found for image: {imageName}");
            return;
        }

        // Get spawn info
        Vector3 spawnPos = trackedImage.transform.position;
        Quaternion spawnRot = trackedImage.transform.rotation;
        Vector3 spawnScale = prefab.transform.localScale;

        // ? Offset: lower object based on prefab's height or manually
        float yOffset = 0.0f;

        Renderer rend = prefab.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            yOffset = rend.bounds.extents.y; // Half the height
        }
        else
        {
            yOffset = 0.05f; // fallback default (5 cm)
        }

        spawnPos.y -= yOffset; // lower the object

        // Spawn and lock
        GameObject instance = Instantiate(prefab, spawnPos, spawnRot);
        instance.name = prefab.name;
        instance.transform.localScale = spawnScale;
        instance.transform.parent = null;

        spawnedObjects[imageName] = instance;
        spawnedOnce.Add(imageName);

        // Lock transform
        var locker = instance.AddComponent<ARTransformLocker>();
        locker.Lock(spawnPos, spawnRot, spawnScale);

        Debug.Log($"? Spawned {imageName} at corrected height. Offset: {yOffset:F3}");

        if (instance.CompareTag("Chef")) // ? Tag-based check
        {
            Animator chefAnimator = instance.GetComponentInChildren<Animator>();
            ChefInteractionManager manager = FindObjectOfType<ChefInteractionManager>();
            if (manager != null && chefAnimator != null)
            {
                manager.SetChefAnimator(chefAnimator);
                Debug.Log("? Chef Animator assigned to ChefInteractionManager");
            }
            else
            {
                Debug.LogWarning("? Could not assign Chef Animator.");
            }
        }


    }
}
