using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Lists")]
    public Color[] player_colors;
    public List<PlayerController_Script> players_list = new List<PlayerController_Script>();
    public Transform[] spawn_points;

    [Header("prefab refs")]
    
    public GameObject playerContPrefab;


    [Header("Components")]
    private AudioSource Audio;
    public AudioClip[] game_fix;
    public Transform containerGroup;
    public TextMeshProUGUI timeText;

    [Header("level vars")]
    public float startTime;
    public float curTime;
    List<PlayerController_Script> winningplayers;
    public bool canJoin;


    public static GameManager instance;

    private void Awake()
    {
        canJoin = true;
        instance = this;
        Audio = GetComponent<AudioSource>();
        containerGroup = GameObject.FindGameObjectWithTag("UIContainer").GetComponent<Transform>();
        startTime = PlayerPrefs.GetFloat("roundTimer", 100);
        winningplayers = new List<PlayerController_Script>();
    }

    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        timeText.text = ((int)curTime).ToString();
    }
    public void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        timeText.text = ((int)curTime).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(curTime <= 0)
        {

            int highscore = 0;
            int index = 0;
            
           
            foreach ( PlayerController_Script player in players_list)
            {
                
                if (player.score > highscore)
                {
                    winningplayers.Clear();
                    highscore = player.score;
                    index = players_list.IndexOf(player);
                    winningplayers.Add(player);
                }
                else if (player.score == highscore)
                {
                    winningplayers.Add(player);
                }
            }

            if (winningplayers.Count > 1)
            {
                canJoin = false;
                foreach(PlayerController_Script player in players_list)
                {
                    if (!winningplayers.Contains(player))
                    {
                        player.drop_out();
                    }
                }
                curTime = 35;
            }
            else
            {
                PlayerPrefs.SetInt("colorIndex", index);

                SceneManager.LoadScene("winScreen");
            }

            

        }   
    }

    public void onPlayerJoined(PlayerInput player)
    {
        if (canJoin)
        {
            Audio.PlayOneShot(game_fix[0]);
            //set player color when joined
            player.GetComponentInChildren<SpriteRenderer>().color = player_colors[players_list.Count];

            //create a ui container
            PlayerContainerUI cont = Instantiate(playerContPrefab, containerGroup).GetComponent<PlayerContainerUI>();
            // asigne cont to a player
            player.GetComponent<PlayerController_Script>().setUI(cont);
            cont.initialize(player_colors[players_list.Count]);

            players_list.Add(player.GetComponent<PlayerController_Script>());
            player.transform.position = spawn_points[Random.Range(0, spawn_points.Length)].position;
        }
     
    }

}
