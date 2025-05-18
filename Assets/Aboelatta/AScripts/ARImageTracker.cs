using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARImageTracker : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    private AudioSource audioSource;

    public AudioClip drummerClip;
    public AudioClip guitarClip;
    public AudioClip trumpetClip;
    public AudioClip successClip;
    public AudioClip errorClip;

    private List<string> currentSequence = new List<string>();
    private string[] expectedPattern = new string[] { "Drummer", "Guitarist", "Drummer", "Trumpet" };

    private Queue<string> imageQueue = new Queue<string>();
    private bool isProcessing = false;
    public bool isPuzzleDone = false;
    private string lastProcessedImage = "";


    void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage image in eventArgs.added)
            EnqueueImage(image);

        foreach (ARTrackedImage image in eventArgs.updated)
            if (image.trackingState == TrackingState.Tracking)
                EnqueueImage(image);
    }

    void EnqueueImage(ARTrackedImage image)
    {

        string imageName = image.referenceImage.name;

        // Skip unknown images (not part of the pattern)
        if (!IsInstrument(imageName))
            return;

        // Prevent repeating the same image repeatedly due to continuous tracking
        if (imageName == lastProcessedImage)
            return;

        // Prevent repeating the same image repeatedly due to continuous tracking
        if (imageName == lastProcessedImage)
            return;

        lastProcessedImage = imageName;

        imageQueue.Enqueue(imageName);

        if (!isProcessing)
            StartCoroutine(ProcessQueue());
    }


    System.Collections.IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (imageQueue.Count > 0)
        {
            string imageName = imageQueue.Dequeue();

            // Play instrument sound
            yield return StartCoroutine(PlayInstrumentSound(imageName));

            // Add to sequence
            currentSequence.Add(imageName);

            // Check if it still matches the expected pattern so far
            if (!IsPartialMatch(currentSequence))
            {
                yield return PlayClip(errorClip);
                currentSequence.Clear();
                continue;
            }

            // Full match
            if (currentSequence.Count == expectedPattern.Length && IsFullMatch(currentSequence))
            {
                yield return PlayClip(successClip);
                isPuzzleDone = true;
                currentSequence.Clear();
            }
        }

        isProcessing = false;
    }

    System.Collections.IEnumerator PlayInstrumentSound(string imageName)
    {
        AudioClip clip = GetClipForImage(imageName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }

    System.Collections.IEnumerator PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }

    AudioClip GetClipForImage(string imageName)
    {
        switch (imageName)
        {
            case "Drummer": return drummerClip;
            case "Guitarist": return guitarClip;
            case "Trumpet": return trumpetClip;
            default: return null;
        }
    }

    bool IsPartialMatch(List<string> sequence)
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if ((i >= expectedPattern.Length || sequence[i] != expectedPattern[i]) && (expectedPattern[i] == "Drummer" || expectedPattern[i] == "Guitarist" || expectedPattern[i] == "Trumpet"))
                return false;
        }
        return true;
    }

    bool IsFullMatch(List<string> sequence)
    {
        if (sequence.Count != expectedPattern.Length)
            return false;

        for (int i = 0; i < expectedPattern.Length; i++)
        {
            if (sequence[i] != expectedPattern[i])
                return false;
        }

        return true;
    }

    bool IsInstrument(string imageName)
    {
        return imageName == "Drummer" || imageName == "Guitarist" || imageName == "Trumpet";
    }
}
