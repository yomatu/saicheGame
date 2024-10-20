using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public Text scoreLabel;
    
    public Text timeLabel;

    private float time;

    private int score;

    private bool gameOver;

    public Car car;
    public Animator UIAnimator;

    public Text gameOverScoreLabel;
    public Text gameOverBestLabel;

    public Animator gameOverAnimator;
    public AudioSource gameOverAudio;
    public GameObject scoreLight;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(0);
        time = 60;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        
        if(gameOver && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))){
            UIAnimator.SetTrigger("Start");
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
        }
    }



    void UpdateTimer()
    {
        time -= Time.deltaTime; // 减少时间


        int timer = (int)time;
        int seconds = timer % 60;
        int minutes = timer / 60;

        string secondRounded = ((seconds < 10) ? "0" : "") + seconds;
        string minutesRounded = ((seconds < 10) ? "0" : "") + minutes;
        timeLabel.text = minutesRounded + ":" + secondRounded;

        if (time <= 0)
        {
            // 倒计时结束后的逻辑
            time = 0;
            GameOver();
        }
    }

    // void UpdateTimer()
    // {
    //     time += Time.deltaTime;
    //
    //     int timer = (int)time;
    //
    //     int seconds = timer % 60;
    //
    //     int minutes = timer / 60;
    //     
    //     string secondRounded =  ((seconds<10)? "0" : "") +seconds;
    //     string minutesRounded =  ((seconds<10)? "0" : "") +minutes;
    //
    //     timeLabel.text = minutesRounded + ":" + secondRounded;
    // }

    public void UpdateScore(int points)
    {
        
        //增加分数
        score += points;

        
        Debug.Log("score: "+score);
        //更新分数信息
        scoreLabel.text = " "  +score;

        
        // 增加时间
        time += 15;

        StartCoroutine(ScoreLight());


    }

    public void GameOver()
    {
        if (gameOver)
        {
            return;
        }

        SetScore(); 
        
        gameOverAnimator.SetTrigger("Game Over");
        gameOverAudio.Play();
        
        car.FallApart();
        
        gameOver = true;

        foreach (BasicMovement basicMovement in GameObject.FindObjectsOfType<BasicMovement>())
        {
            basicMovement.movespeed = 0;
            basicMovement.rotatespeed = 0;
        }
        
    }

    void SetScore()
    {
        if (score>PlayerPrefs.GetInt("best"))
        {
            PlayerPrefs.SetInt("best",score);
        }

        gameOverScoreLabel.text = "score: " + score;
        gameOverBestLabel.text = "best: " + PlayerPrefs.GetInt("best");
        
        
        


    }	
    //wait less than a second and load the given scene
    IEnumerator LoadScene(string scene){
        yield return new WaitForSeconds(0.6f);
		
        SceneManager.LoadScene(scene);
    }
    IEnumerator ScoreLight(){
        
        scoreLight.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        scoreLight.SetActive(false);

		
  
    }

    
    public void BackToMenu()
    {
  
        SceneManager.LoadScene("Menu");
    }
}
