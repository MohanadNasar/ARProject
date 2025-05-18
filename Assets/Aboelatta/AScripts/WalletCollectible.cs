using UnityEngine;

public class WalletCollectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    //[SerializeField] private GameObject collectEffect;

    public bool isCollected = false;

    void Update()
    {
        if (isCollected) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    Collect();
                }
            }
        }
    }

    private void Collect()
    {
        isCollected = true;

        // Optional: Play collection sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }


        // Optional: Spawn particle effect
        //if (collectEffect != null)
        //{
        //    Instantiate(collectEffect, transform.position, Quaternion.identity);
        //}

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScoreFromOutside(100); // or any amount
        }
        else
        {
            Debug.LogWarning("?? ScoreManager not found in scene!");
        }

        // Disable or destroy the wallet
        gameObject.SetActive(false);
        // or Destroy(gameObject);
    }
}
