using UnityEngine.EventSystems;
using UnityEngine;

public class ChefTapDetector : MonoBehaviour
{
    private Camera arCamera;
    private ChefInteractionManager chefInteractionManager;

    private void Start()
    {
        arCamera = Camera.main;
        chefInteractionManager = FindObjectOfType<ChefInteractionManager>(); // or assign via inspector
    }

    void Update()
    {
        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        Touch touch = Input.GetTouch(0);
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Chef"))
            {
                if (chefInteractionManager != null)
                {
                    chefInteractionManager.OnTapped();
                }
            }
        }
    }
}
