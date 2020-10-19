using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();

    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;
     //состояние стрвницы
     enum PageState
        {
          None,
          Start,
          GameOver,
          Countdown
        }

     private int  score = 0;
     private bool gameOver = true;

     public bool GameOver
     {
         get
         {
             return gameOver;
         }
     }

     public int Score
     {
         get
         {
             return score;
         }
     }

     void Awake()
     {
         Instance = this;
     }
     
     //подписка на обратный отсчет 
     void OnEnable()
     {
         CountdownText.OnCountdownFinished += OnCountdownFinished;
         TapController.OnPlayerDied += OnPlayerDied;
         TapController.OnPlayerScored += OnPlayerScored;
     }

     void OnDisable()
     {
         CountdownText.OnCountdownFinished -= OnCountdownFinished;
         TapController.OnPlayerDied -= OnPlayerDied;
         TapController.OnPlayerScored -= OnPlayerScored;
     }

     void OnCountdownFinished()
     {
         SetPageState(PageState.None);
         OnGameStarted();//событие отправленно в тап контроллер
         score = 0;
         gameOver = false;
     }

     void OnPlayerDied()
     {
         gameOver = true;
         //сохранение лучшего результата 
         int savedScore = PlayerPrefs.GetInt("HighScore");
         if (score > savedScore)
         {
             PlayerPrefs.SetInt("HighScore", score);
         }

         SetPageState(PageState.GameOver);
     }

     void OnPlayerScored()
     {
         score++;
         scoreText.text = score.ToString();
     }

     // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
   //прием параметра состояния страницы
   //аактивация и деоктивация страниц в зависимости от состояния
   void SetPageState(PageState state)
   {
       switch (state)
       {
           case PageState.None:
               startPage.SetActive(false);
               gameOverPage.SetActive(false);
               countdownPage.SetActive(false);
               break;
           case PageState.Start:
               startPage.SetActive(true);
               gameOverPage.SetActive(false);
               countdownPage.SetActive(false);
               break;
           case PageState.GameOver:
               startPage.SetActive(false);
               gameOverPage.SetActive(true);
               countdownPage.SetActive(false);
               break;
           case PageState.Countdown:
               startPage.SetActive(false);
               gameOverPage.SetActive(false);
               countdownPage.SetActive(true);
               break;
       }
   }
   //метод для кнопрки replay
   public void ConfirmGameOver()
   {
   //зброс игровых обектов 
        OnGameOverConfirmed();//событие отправленно в тап контроллер 
	//зброс счета 
		scoreText.text = "0";
	//установка состояние страницы
		SetPageState(PageState.Start);
   }
   //метод для книпки play
   public void StartGame()
   {
       SetPageState(PageState.Countdown);
   }
}
