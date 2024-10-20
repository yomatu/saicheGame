using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    
    public AudioSource scoreAudio;
    private GameManager manager;

    private bool addedScore;
    
    void Start()
    {

        manager = GameObject.FindObjectOfType<GameManager>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.transform.root.CompareTag("Player")||addedScore)
        {
            return;
        }

        addedScore = true;

        manager.UpdateScore(1);
        scoreAudio.Play();
    }
}
