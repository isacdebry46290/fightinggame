using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    
    public AudioSource audio;
    public AudioClip[] clips;
    public float damage;
    public float lifeTime;
    public PlayerController_Script owner;
    public float speed;
    public Rigidbody2D rig;


    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audio.PlayOneShot(clips[0]);
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        audio.PlayOneShot(clips[1]);
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController_Script>().takeDamage(damage);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        Destroy(gameObject, 1f);
    }
    
    public void onSpawn(float damage, float speed, PlayerController_Script owner, float dir)
    {
        setDamage(damage);
        setOwner(owner);
        setSpeed(speed);
        rig.velocity = new Vector2(dir * speed, 0);
        
    }

    public void setOwner(PlayerController_Script owner)
    {
        this.owner = owner;
    }
    public void setDamage(float damage)
    {
        this.damage = damage;
    }
    public void setDamage(int damage)
    {
        this.damage = damage;
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
}