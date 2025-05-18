using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.TransformLogic;

public class InventoryAdder : MonoBehaviour
{
    public Camera arCamera;
    public PlaceableObjectSODatabase database;
    public GameObject placeableObjectTemplatePrefab;

    private void Update()
    {
        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began) return;

        Touch touch = Input.GetTouch(0);
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            InventoryAddable addable = hit.collider.GetComponent<InventoryAddable>();
            if (addable != null)
            {
                AddToInventory(addable.itemData);
            }
        }
    }

    public void AddToInventory(AddableItemData data)
    {
        //Prevent duplicates
        foreach (var item in database.PlacementObjectSos)
        {
            if (item.nameOfObject == data.objectName)
            {
                Debug.Log("Item already in database.");
                return;
            }
        }

        // Create the placeable object at runtime
        GameObject placeableInstance = Instantiate(placeableObjectTemplatePrefab);
        GameObject visual = Instantiate(data.prefab, placeableInstance.transform);
        visual.transform.localPosition = Vector3.zero;

        TransformableObject transformable = placeableInstance.GetComponentInChildren<TransformableObject>();

        PlacementObjectSO obj = ScriptableObject.CreateInstance<PlacementObjectSO>();
        obj.nameOfObject = data.objectName;
        obj.uiSprite = data.uiSprite;
        obj.placementObject = transformable;

        // Add to database
        database.PlacementObjectSos.Add(obj);

        Debug.Log("Added to inventory: " + data.objectName);

        //placeableInstance.SetActive(false);


        // Refresh UI at runtime
        PlacementBarLogic.Instance?.RefreshInventoryUI();
    }

    public bool HasItem(string objectName)
    {
        return database.PlacementObjectSos.Exists(item => item.nameOfObject == objectName);
    }

    public bool HasItems(GameObject[] prefabs, int requiredCount)
    {
        int count = 0;
        foreach (var prefab in prefabs)
        {
            if (database.PlacementObjectSos.Exists(p => p.nameOfObject == prefab.name))
                count++;
        }
        return count >= requiredCount;
    }

    public void RemoveItemByName(string objectName)
    {
        var item = database.PlacementObjectSos.Find(p => p.nameOfObject == objectName);
        if (item != null)
        {
            database.PlacementObjectSos.Remove(item);
            PlacementBarLogic.Instance?.RefreshInventoryUI();
        }
    }
}
