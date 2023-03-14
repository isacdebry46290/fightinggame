using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController_Script : MonoBehaviour
{
    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float curAttackTime;
    public float slowTime;
    public float maxSpeed;
    public float max_charge_dmg;




    [Header("Cur Values")]
    public int curHp;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public int diecount;
    public float lastHit;
    public float lastHitice;
    public bool isSlowed;
    public float charge_dmg;
    public float charge_rate;
    public bool ischarging;
    public GameObject deathEfectprefab;


    [Header("Attacking")]
    public PlayerController_Script curAttacker;
    public float attackDmg;
    public float attackSpeed;
    public float iceAttackSpeed;
    public float attackRate;
    public float lastAttackTime;
    public GameObject[] attackPrefabs;


    [Header("MODS")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Audion clips")]
    //jump snd 0
    //land snd 1
    //taunt_1 2
    //shhot snd 3
    public AudioClip[] playerfx_list;


    [Header("Componhenets")]
    private Rigidbody2D rig;
    private Animator anim;
    private AudioSource audio;
    private Transform muzzle;
    private GameManager gameManager;
    private PlayerContainerUI playercont;
    




    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        rig = GetComponent<Rigidbody2D>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();
        gameManager = GameObject.FindObjectOfType<GameManager>(); 
    }

    public void setUI(PlayerContainerUI playerUI)
    {
        this.playercont = playerUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
        curJumps = maxJumps;
        score = 0;
        moveSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10 || curHp <= 0)
        {
            die();
        }  
        if(diecount >= 10)
        {
            die();
            diecount = 0;
        }

        if (curAttacker)
        {
            if(Time.time - lastHit > curAttackTime)
            {
                curAttacker = null;
            }
        }
        if (isSlowed)
        {
            if (Time.time - lastHitice > slowTime)
            {
                isSlowed = false;
                moveSpeed = maxSpeed;
            }
        }

        if (ischarging)
        {
            charge_dmg += charge_rate;
            if(charge_dmg > max_charge_dmg)
            {
                charge_dmg = max_charge_dmg;
            }
        }
    }
    
    private void FixedUpdate()
    {
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (ContactPoint2D hit in collision.contacts)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    if(hit.point.y < transform.position.y)
                    {
                        audio.PlayOneShot(playerfx_list[1]);
                        curJumps = maxJumps;
                    }
                }
                if((hit.point.x > transform.position.x|| hit.point.x < transform.position.x) && hit.point.y < transform.position.y);
                {
                    if(maxJumps == 0)
                    {
                        curJumps++;
                    }
               
                }
            }
        }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void move()
    {

        rig.velocity = new Vector2(curMoveInput * moveSpeed, rig.velocity.y);

        //player direction 
        if(curMoveInput != 0)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }

    }
    private void jump()
    {
        //play jump sound
        audio.PlayOneShot(playerfx_list[0]);
        rig.velocity = new Vector2(rig.velocity.x, 0);
        //add force up
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void die()
    {
        Destroy(Instantiate(deathEfectprefab, transform.position, Quaternion.identity));
        print("player has died");
        // play die snd
        audio.PlayOneShot(playerfx_list[4]);
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            if(score > 0)
            {
                score = 0;
            }
        }
        diecount++;
        respawn();

    }
    public void drop_out()
    {
        Destroy(playercont.gameObject);
        Destroy(gameObject);
    }

    public void addScore()
    {
        score++;
        playercont.updateScoreText(score);
    }

    public void takeDamage(int ammount, PlayerController_Script attacker)
    {
        curHp -= ammount;
        curAttacker = attacker;
        if (ischarging)
        {
            charge_dmg /= 2;
        }

    }

    public void takeDamage(float ammount)
    {
        curHp -= (int)ammount;
        curAttacker = curAttacker;
        lastHit = Time.time;
        if (ischarging)
        {
            charge_dmg /= 2;
        }
    }
    public void takeIceDamage(float ammount)
    {
        curHp -= (int)ammount;
        curAttacker = curAttacker;
        lastHit = Time.time;
        isSlowed = true;
        lastHit = Time.time;
        lastHitice = Time.time;
        moveSpeed /= 2;
        if (ischarging)
        {
            charge_dmg /= 2;
        }
        playercont.updateHealthBar(curHp, maxHp);
    }
    private void respawn()
    {
        curHp = maxHp;
        curJumps = maxJumps;
        curAttacker = null;
        rig.velocity = Vector2.zero;
        moveSpeed = moveSpeed;
        transform.position = gameManager.spawn_points[Random.Range(0,gameManager. spawn_points.Length)].position;
        playercont.updateHealthBar(curHp, maxHp);
    }
    // input Action map methods
    // move input method
    public void onMoveInput(InputAction.CallbackContext context)
        {
        
            float x = context.ReadValue<float>();
            if(x > 0)
            {
                curMoveInput = 1;
            }
            else if (x < 0)
            {
                curMoveInput = -1;
            }
            else
            {
                curMoveInput = 0;
            }
        }
        public void onJumpInput(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                if(curJumps > 0)
                {
                    curJumps--;
                    jump();
                }
            

            }
        
        }
    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }

    }
    public void onAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed&& Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_attack_attack();
        }
        if (ischarging)
        {
            ischarging = false;
            charge_dmg = 0;
        }

    }

    public void spawn_attack_attack()
    {
        GameObject fireBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireBall.GetComponent<Fireball>().onSpawn(attackDmg, attackSpeed, this, transform.localScale.x);
    }

    public void spawn_ice_attack()
    {
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<Fireball>().onSpawn(attackDmg, iceAttackSpeed, this, transform.localScale.x);
    }

    public void spawn_Charge_attack()
    {
        GameObject chargeBall = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chargeBall.GetComponent<Fireball>().onSpawn(charge_dmg, attackSpeed, this, transform.localScale.x);
    }

    public void oniceAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_ice_attack();
        }
        if (ischarging)
        {
            ischarging = false;
            charge_dmg = 0;
        }

    }
    public void onChargeAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            ischarging = true;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            // toggle off charging bool
            ischarging = false;
            // spawn fire ball
            spawn_Charge_attack();
            // set dmg vallue back to 0
            charge_dmg = 0;
        }

    }
    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            audio.PlayOneShot(playerfx_list[2]);
        }

    }
    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt2 button");
        }

    }
    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt3 button");
        }

    }
    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt4 button");
        }

    }
    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
        }

    }










}
