using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProgectile : Fireball
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        audio.PlayOneShot(clips[1]);
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController_Script>().takeIceDamage(damage);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        Destroy(gameObject, 1f);
    }


}
