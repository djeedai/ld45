using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioClip MoveSfx;
    public AudioClip SelectSfx;
    public AudioClip CancelSfx;

    private GameObject _menuArrow;
    private int _selectedIndex = 0;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.Stop();
        _audioSource.loop = false;
        _audioSource.volume = 1f;
    }

    void Start()
    {
        _menuArrow = transform.Find("SelectArrow").gameObject;
    }

    void Update()
    {
        // OpenMenu button closes the menu if already open.
        // If inside Update() it means the menu is open.
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("OpenMenu"))
        {
            Director.Instance.PlaySfx(CancelSfx);
            gameObject.SetActive(false);
            return;
        }

        if (Input.GetButtonDown("Use"))
        {
            Director.Instance.PlaySfx(SelectSfx);
            switch (_selectedIndex)
            {
            case 0:
                Director.Instance.RestartLevel();
                break;
            case 1:
                Director.Instance.ExitGame();
                break;
            }
            return;
        }

        const int NumEntries = 2;
        bool changed = false;
        if (Input.GetButtonDown("MenuDown"))
        {
            changed = true;
            _selectedIndex = (_selectedIndex + 1) % NumEntries;
        }
        else if (Input.GetButtonDown("MenuUp"))
        {
            changed = true;
            _selectedIndex = (_selectedIndex + NumEntries - 1) % NumEntries;
        }
        if (changed)
        {
            _audioSource.clip = MoveSfx;
            _audioSource.Play();
            Vector3 pos = _menuArrow.transform.localPosition;
            pos.y = -0.06f - 0.13f * _selectedIndex;
            _menuArrow.transform.localPosition = pos;
        }
    }
}
