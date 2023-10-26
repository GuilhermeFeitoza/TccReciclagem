using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time

}
[System.Serializable]
public class EndGameRequirements
{

    public GameType gameType;
    public int counterValue;

}


public class EndGameManager : MonoBehaviour
{

    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    public float timerSeconds;
    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();

    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else if (requirements.gameType == GameType.Time)
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);


        }
        counter.text = "" + currentCounterValue;
    }

    public void SetGameType()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }

    }
    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {

            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue == 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {

        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        PanelController Fade = FindObjectOfType<PanelController>();
        Fade.GameOver();
    }
    public void LoseGame()
    {

        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        PanelController Fade = FindObjectOfType<PanelController>();
        Fade.GameOver();

    }









    void Update()
    {

        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {

                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
