using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PackageManager : MonoBehaviour
{
    [Header("UI objects")]
    [SerializeField] private GameObject EndGameMenu;
    [SerializeField] private GameObject PackageUI;
    [SerializeField] private GameObject ScoreUI;
    [Header("Player things")]
    [SerializeField] private Waypoint playerPointArrow;
    [Header("Delivery")]
    [SerializeField] private Sprite[] packageImages;
    [SerializeField] private GameObject[] itemInHands;
    [SerializeField] private GameObject[] houses;
    [SerializeField] private Image currentImage;
    [SerializeField] private Image coolDownBar;
    [Header("Text fields")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    
    private int score;
    private int previousPackageRand = 0;
    private int previousHouseRand = 0;
    private float timer;
    private float overAllTime;

    private string minutes, seconds;

    public GameObject currentObjectInHands;

    private void Start()
    {
        Restart();
    }

    private void Update()
    {
        MinutesAndSeconds();
        
        scoreText.text = "Score: " + score;
        timeText.text = minutes + seconds;
        finalScoreText.text = "Your score: " + score;

        timer += Time.deltaTime;
        overAllTime += Time.deltaTime;
        
        if (timer >= 10f)
            Restart();
        
        if(overAllTime > 180f)
            GameEnd();

        coolDownBar.fillAmount = (10 - timer) / 10f;
    }

    private void PackageRoll()
    {
        int current = Random.Range(0, Mathf.Min(packageImages.Length, itemInHands.Length));;
        if (previousPackageRand == current)
        {
            PackageRoll();
            return;
        }
        
        previousPackageRand = current;

        currentImage.sprite = packageImages[current];

        for (int i = 0; i < itemInHands.Length; i++)
            itemInHands[i].SetActive(false);
        
        itemInHands[current].SetActive(true);

        currentObjectInHands = itemInHands[current];
        
        Debug.Log(currentObjectInHands.GetComponent<Item>().itemToThrow);
    }

    private void HouseRoll()
    {
        int current = Random.Range(0, houses.Length);
        if (previousHouseRand == current)
        {
            HouseRoll();
            return;
        }
        
        previousHouseRand = current;
        
        houses[current].GetComponent<Delivery>().Selected();
        playerPointArrow.SetNewPoint(houses[current].transform);
    }

    private void Restart()
    {
        timer = 0f;
        PackageRoll();
        HouseRoll();
    }

    private void MinutesAndSeconds()
    {
        int min = (int) overAllTime / 60;

        int sec = overAllTime % 60 > 60 ? (int) overAllTime - 60 * min : (int) overAllTime % 60;

        if (sec >= 0 && sec < 1)
            seconds = "00";
        else if (sec < 10)
            seconds = "0" + sec;
        else
            seconds = sec.ToString();
        minutes = min + ":";
    }

    private void GameEnd()
    {
        Time.timeScale = 0f;
        ScoreUI.SetActive(false);
        PackageUI.SetActive(false);
        EndGameMenu.SetActive(true);
        if (score > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", score);
    }

    public void DeliverySucceed()
    {
        int bonus = (int) (10 - timer) % 10;
        score += 1 + bonus;

        Restart();
    }
}
