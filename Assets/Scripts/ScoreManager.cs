using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public GameplayManager gameplayManager;

    public TMP_Text scoreText;
    public TMP_Text highscoreText;

    public int tinyScore;
    public int tinyHighscore;

    public int classicScore;
    public int classicHighscore;

    public int bigScore;
    public int bigHighscore;

    public int biggerScore;
    public int biggerHighscore;

    public int hugeScore;
    public int hugeHighscore;


    // Start is called before the first frame update
    void Start()
    {
        tinyScore = PlayerPrefs.GetInt("tinyScore", 0);
        tinyHighscore = PlayerPrefs.GetInt("tinyHighscore", 0);

        classicScore = PlayerPrefs.GetInt("classicScore", 0);
        classicHighscore = PlayerPrefs.GetInt("classicHighscore", 0);

        bigScore = PlayerPrefs.GetInt("bigScore", 0);
        bigHighscore = PlayerPrefs.GetInt("bigHighscore", 0);

        biggerScore = PlayerPrefs.GetInt("biggerScore", 0);
        biggerHighscore = PlayerPrefs.GetInt("biggerHighscore", 0);

        hugeScore = PlayerPrefs.GetInt("hugeScore", 0);
        hugeHighscore = PlayerPrefs.GetInt("hugeHighscore", 0);

        UpdateScoresText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToScore(int val)
    {
        switch (gameplayManager.boardSize)
        {
            case GameplayManager.BoardSize.Tiny:
                tinyScore += val;
                PlayerPrefs.SetInt("tinyScore", tinyScore);
                if (tinyScore > tinyHighscore)
                {
                    tinyHighscore = tinyScore;
                    PlayerPrefs.SetInt("tinyHighscore", tinyHighscore);
                }

                break;

            case GameplayManager.BoardSize.Classic:
                classicScore += val;
                PlayerPrefs.SetInt("classicScore", classicScore);
                if (classicScore > classicHighscore)
                {
                    classicHighscore = classicScore;
                    PlayerPrefs.SetInt("classicHighscore", classicHighscore);
                }

                break;

            case GameplayManager.BoardSize.Big:
                bigScore += val;
                PlayerPrefs.SetInt("bigScore", bigScore);
                if (bigScore > bigHighscore)
                {
                    bigHighscore = bigScore;
                    PlayerPrefs.SetInt("bigHighscore", bigHighscore);
                }

                break;

            case GameplayManager.BoardSize.Bigger:
                biggerScore += val;
                PlayerPrefs.SetInt("biggerScore", biggerScore);
                if (biggerScore > biggerHighscore)
                {
                    biggerHighscore = biggerScore;
                    PlayerPrefs.SetInt("biggerHighscore", biggerHighscore);
                }

                break;

            case GameplayManager.BoardSize.Huge:
                hugeScore += val;
                PlayerPrefs.SetInt("hugeScore", hugeScore);
                if (hugeScore > hugeHighscore)
                {
                    hugeHighscore = hugeScore;
                    PlayerPrefs.SetInt("hugeHighscore", hugeHighscore);
                }

                break;
        }

        PlayerPrefs.Save();

        UpdateScoresText();
    }

    public int GetScore()
    {
        switch (gameplayManager.boardSize)
        {
            case GameplayManager.BoardSize.Tiny:
                return tinyScore;

            case GameplayManager.BoardSize.Classic:
                return classicScore;

            case GameplayManager.BoardSize.Big:
                return bigScore;

            case GameplayManager.BoardSize.Bigger:
                return biggerScore;

            case GameplayManager.BoardSize.Huge:
                return hugeScore;
        }

        return 0;
    }

    public void SetScore(int score)
    {
        switch (gameplayManager.boardSize)
        {
            case GameplayManager.BoardSize.Tiny:
                tinyScore = score;
                break; 

            case GameplayManager.BoardSize.Classic:
                classicScore = score;
                break;

            case GameplayManager.BoardSize.Big:
                bigScore = score;
                break;

            case GameplayManager.BoardSize.Bigger:
                biggerScore = score;
                break;

            case GameplayManager.BoardSize.Huge:
                hugeScore = score;
                break;
        }

        UpdateScoresText();
    }


    public void ResetScore()
    {
        switch (gameplayManager.boardSize)
        {
            case GameplayManager.BoardSize.Tiny:
                tinyScore = 0;
                PlayerPrefs.SetInt("tinyScore", tinyScore);
                break;

            case GameplayManager.BoardSize.Classic:
                classicScore = 0;
                PlayerPrefs.SetInt("classicScore", classicScore);
                break;

            case GameplayManager.BoardSize.Big:
                bigScore = 0;
                PlayerPrefs.SetInt("bigScore", bigScore);
                break;

            case GameplayManager.BoardSize.Bigger:
                biggerScore = 0;
                PlayerPrefs.SetInt("biggerScore", biggerScore);
                break;

            case GameplayManager.BoardSize.Huge:
                hugeScore = 0;
                PlayerPrefs.SetInt("hugeScore", hugeScore);
                break;
        }

        PlayerPrefs.Save();

        UpdateScoresText();
    }

    public void UpdateScoresText()
    {
        switch (gameplayManager.boardSize)
        {
            case GameplayManager.BoardSize.Tiny:
                scoreText.text = tinyScore.ToString();
                highscoreText.text = tinyHighscore.ToString();
                break;

            case GameplayManager.BoardSize.Classic:
                scoreText.text = classicScore.ToString();
                highscoreText.text = classicHighscore.ToString();
                break;

            case GameplayManager.BoardSize.Big:
                scoreText.text = bigScore.ToString();
                highscoreText.text = bigHighscore.ToString();
                break;

            case GameplayManager.BoardSize.Bigger:
                scoreText.text = biggerScore.ToString();
                highscoreText.text = biggerHighscore.ToString();
                break;

            case GameplayManager.BoardSize.Huge:
                scoreText.text = hugeScore.ToString();
                highscoreText.text = hugeHighscore.ToString();
                break;
        }
    }
}
