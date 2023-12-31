using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    private GoalManager goalManager;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        if(hitPoints <= 0)
        {
            if (goalManager != null) {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    void Initialize()
    {
       
    }
    public void TakeDamage(int damage)
    {


        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
