using UnityEngine;

public class CubeModel
{
    public bool GameOver { get; private set; }
    public float StartTime { get; private set; }
    public float HighScore { get; private set; }

    const string HighScoreKey = "HighScore";

    public CubeModel()
    {
        HighScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
    }

    public void StartGame()
    {
        GameOver = false;
        StartTime = Time.time;
    }

    public float EndGame()
    {
        GameOver = true;
        float timeInAir = Time.time - StartTime;
        if (timeInAir > HighScore)
        {
            HighScore = timeInAir;
            PlayerPrefs.SetFloat(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
        return timeInAir;
    }
}
