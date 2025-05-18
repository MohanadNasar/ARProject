using UnityEngine;

public class MaidTouchInteraction : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private Camera arCamera;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform)
                {
                    TriggerTalk();
                }
            }
        }
    }

    private void TriggerTalk()
    {
        if (animator != null)
        {
            animator.SetTrigger("Talk");
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
