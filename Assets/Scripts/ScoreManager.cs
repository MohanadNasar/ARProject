using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;

    // References to puzzle scripts (drag in Inspector)
    private WalletCollectible puzzle1;
    public ARImageTracker puzzle2;
    public ChefInteractionManager puzzle3;

    private int score = 0;
    private bool p2Scored = false;
    private bool p3Scored = false;



    void Update()
    {

        if (puzzle2 != null && puzzle2.isPuzzleDone && !p2Scored)
        {
            AddScore(100);
            p2Scored = true;
        }

        if (puzzle3 != null && puzzle3.isComplete && !p3Scored)
        {
            AddScore(100);
            p3Scored = true;
        }
    }


    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void AddScoreFromOutside(int amount)
    {
        AddScore(amount);
    }
}
