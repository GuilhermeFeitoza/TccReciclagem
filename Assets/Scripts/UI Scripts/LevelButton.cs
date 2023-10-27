using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Active")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    public Text levelTextSprite;

    public Image[] stars;
    public Text leveText;
    public int level;
    public GameObject confirmPanel;
    private Button myButton;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        ActiveStars();
        ShowLevel();
        DecideSprite();
    }

    void ActiveStars() {


        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    void DecideSprite()
    {

        if (isActive)
        {

            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelTextSprite.enabled = true;

        }
        else {

            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelTextSprite.enabled = false;

        }

    }
    void ShowLevel()
    {
        leveText.text = "" + level;


    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ConfirmPanel(int level) {

        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
