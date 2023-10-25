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
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    public float timerSeconds;
    // Start is called before the first frame update
    void Start()
    {

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

    public void DecreaseCounterValue()
    {
        currentCounterValue--;
        counter.text = "" + currentCounterValue;
        if(currentCounterValue == 0)
        {
            currentCounterValue = 0;
            counter.text = "" + currentCounterValue;

        }
   }





    



    void Update()
    {

        if(requirements.gameType == GameType.Time && currentCounterValue > 0 )
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0) {
                
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
