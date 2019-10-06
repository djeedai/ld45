using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    public float Speed = 1.0f;
    public UnityEvent OnLever = new UnityEvent();

    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.Stop();
        _audioSource.loop = false;
        _audioSource.volume = 1f;
    }

    void Update()
    {
        float t = Time.time;
        if ((t * Speed) % 2 <= 1.0f)
        {
            _spriteRenderer.color = Color.gray;
        }
        else
        {
            _spriteRenderer.color = Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        _audioSource.Play();
        OnLever.Invoke();
    }
}
