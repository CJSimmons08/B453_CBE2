using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioClip GirlHitSound;
    public AudioClip PlatformHitSound;

    public Vector3 Velocity;

    
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Velocity * Time.deltaTime, Space.World);
        CheckToErase();
    }

    private void CheckToErase()
    {
        if (transform.position.y < GlobalManager.Instance.GetBottomOfScreen()) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var girl = collision.GetComponent<Girl>();
        if (girl != null)
        {
            // We hit the girl
            if (girl.TakeDamage() != GirlDamageTaken.NoDamage)
            {
                audioSource.PlayOneShot(GirlHitSound);
            }
            return;
        }
        var platform = collision.GetComponent<Platform>();
        if (platform != null)
        {
            // We hit a platform, let's destroy both of us
            // We must use a global audio source becase we will destry ourselves immediately, and
            // and the audio wouldn't play
            GlobalManager.Instance.AudioSource.PlayOneShot(PlatformHitSound);
            Destroy(gameObject);
            Destroy(platform.gameObject);
        }
    }
}
