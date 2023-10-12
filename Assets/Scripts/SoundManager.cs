using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource moveNoise,garbageTruckNoise;
    public void PlayRandomDestroyNoise() {

        int clipToPlay = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToPlay].Play();
    
    
    }

    public void PlayMoveNoise() {

        moveNoise.Play();
    
    }
    public void PlayTruckNoise()
    {

        garbageTruckNoise.Play();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
