﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //UI显示的内容
    public Text timeText;
    private float gameTime;
    public static bool success;
    public static bool newStart = false;
    public static int score;
    private int currentScore;
    private float addScoreTime;
    public Text playerScore;
    public Button btn_back;
    public GameObject panel;
    public Text txt_tip;
    public GameObject img_tip;
    static public int level = 2;
    public AudioClip nextAudio;
    public AudioClip timeAudio;

    void initGameManager()
    {
        gameTime = 60;
        score = 0;
        success = false;
        currentScore = 0;
        addScoreTime = 0;
        playerScore.text = "0";
    }

    // Start is called before the first frame update
    internal void Start()
    {
        panel.SetActive(false);
        initGameManager();
    }

    /// <summary>
    /// 回到主菜单
    /// </summary>
    public void ToMenu()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void ReStartGame()
    {
        SceneManager.LoadScene("Scene1");
    }
    public IEnumerator ReStartGame2()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Scene1");
    }
    
    public void ButtonOverFunc()
    {
        if (success)
        {
            level++;
        }
        //gameObject.GetComponent<GameController>().InitGameController();
        //initGameManager();
        //panel.SetActive(false);
        AudioSource.PlayClipAtPoint(nextAudio, transform.position);
        StartCoroutine(ReStartGame2());
    }

    public void ChangeGameSpeed(float targetSpeed)
    {
        Time.timeScale = targetSpeed;
    }

    public void ShowTip()
    {
        if (txt_tip.text.Length == 0)
        {
            switch(level)
            {
                case 0:
                    txt_tip.text = "level 1\n生存是文明的第一要义：收集五个不同星球的图标各6个，每消去一个地球生命值减一，生命值为零则游戏失败。";break;
                case 1:
                    txt_tip.text = "level 2\n黑暗森林法则：在60秒内消尽可能多的星球，分数达到500即可通关，每消去一个地球生命值减一，生命值为零则游戏失败。"; break;
                case 2:
                    txt_tip.text = "level 3\n收集10个太阳周围的能量站"; break;
                case 3:
                    txt_tip.text = "level 4\n消除10个质子探测器，不允许消除能量站"; break;
                default:
                    break;
            }
            
            img_tip.SetActive(true);
        }
        else
        {
            txt_tip.text = "";
            img_tip.SetActive(false);
        }
    }

    // Update is called once per frame
    internal void Update()
    {
        if (panel.activeSelf)
        {
            return;
        }
        gameTime -= Time.deltaTime;
        if(gameTime<=0)
        {
            gameTime = 0;
            MakeGameOver();
            return;
        }
        int count = 10;
        timeText.text = gameTime.ToString("0");
        if((int)(gameTime) - count ==0)
        {
            count--;
            AudioSource.PlayClipAtPoint(timeAudio, transform.position);
        }
        if (addScoreTime <= 0.05)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {
            if (currentScore < score)
            {
                currentScore+=10;
                playerScore.text = currentScore.ToString();
                addScoreTime = 0;
            }
        }
    }

    public void MakeGameOver()
    {
        panel.SetActive(true);
    }
}
