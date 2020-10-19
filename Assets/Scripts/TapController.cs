using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Rigidbody всегда добавлялся к одному и тому же обекту  автоматически 

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;



    public float tapForce = 10;
    //Наклон
    public float tiltSmooth = 5;
    //начальная позиция
    public Vector3 startPos;
    private new Rigidbody2D rigidbody;
    //наклон впере
    private Quaternion downRotation;
    //наклон вверх
    private Quaternion forwardRotation;
    private GameManager game;

	public AudioSource tapAudio;
	public AudioSource pointAudio;
	public AudioSource deadAudio;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
		rigidbody.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    void OnGameStarted()
    {
         //сброс скорости в начале игры 
         rigidbody.velocity = Vector3.zero;
         rigidbody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }
   
    
    
    // Update is called once per frame
    void Update()
    {
        if (game.GameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
			tapAudio.Play();
            transform.rotation = forwardRotation;
            //обновляется скорость
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        
    }
    
    //проверка в какую зону попал player
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone")
        {
            //подсчет очков
            OnPlayerScored();//событие отпралено GameManager
			pointAudio.Play();
        }

        if (col.gameObject.tag == "DeadZone")
        {
            //заморозка при остановке 
            rigidbody.simulated = false;
            OnPlayerDied();//событие отпралено GameManager
			deadAudio.Play();
        }
    }
}
