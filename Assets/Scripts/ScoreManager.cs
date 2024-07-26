using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    public PlayerController player1; 
    public PlayerController player2;

    private int player1Score = 0;
    private int player2Score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    public void IncreaseScore(PlayerController player)
    {
        if (player == player1)
        {
            player1Score++;
        }
        else if (player == player2)
        {
            player2Score++;
        }
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        player1ScoreText.text = "Player 1: " + player1Score.ToString();
        player2ScoreText.text = "Player 2: " + player2Score.ToString();
    }
}
