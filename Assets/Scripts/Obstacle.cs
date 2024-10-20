using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private GameManager manager;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
    }

   void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.transform.root.CompareTag("Player"))
        {
            
            Debug.Log("接触到的物体为: " + other.gameObject.name);
            // 检查是否有 null 引用
            if (other == null)
            {
                Debug.LogError("数组的值为空");
                return;
            }
             manager.GameOver();
        }
    }
}
