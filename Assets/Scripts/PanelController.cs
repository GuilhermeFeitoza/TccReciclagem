using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator panelAnim;
    public Animator gameInfoAnim;

    public void Ok()
    {
        if(panelAnim != null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);

        }
       

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
