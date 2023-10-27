using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;
    public Image[] stars;
    public int level; 
    // Start is called before the first frame update
    void Start()
    {
        ActiveStars();
        
    }

    void ActiveStars()
    {


        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);


    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
  


}

