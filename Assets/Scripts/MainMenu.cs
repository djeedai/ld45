﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioClip OpenMenuSfx;
    public AudioClip MoveSfx;
    public AudioClip SelectSfx;
    public AudioClip CancelSfx;
    public float KeyRepeatRate = 1f;

    private GameObject _menuArrow;
    private int _selectedIndex = 0;
    private float _timeSinceLastArrow = 0f;

    void Start()
    {
        _menuArrow = transform.Find("SelectArrow").gameObject;
    }

    void OnEnable()
    {
        Director.Instance.PlaySfx(OpenMenuSfx);
    }

    void Update()
    {
        // OpenMenu button closes the menu if already open.
        // If inside Update() it means the menu is open.
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("OpenMenu"))
        {
            var dir = Director.Instance;
            dir.PlaySfx(CancelSfx);
            dir.CloseMenu();
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

        var y = Input.GetAxisRaw("Vertical");
        bool inputDown = false;
        bool inputUp = false;
        float curTime = Time.time;
        if ((y != 0f) && ((curTime - _timeSinceLastArrow) >= KeyRepeatRate))
        {
            _timeSinceLastArrow = curTime;
            if (y > 0f)
                inputUp = true;
            else
                inputDown = true;
        }
        else if (y == 0f)
        {
            _timeSinceLastArrow = 0f;
        }

        const int NumEntries = 2;
        bool changed = false;
        if (inputDown || Input.GetButtonDown("MenuDown"))
        {
            changed = true;
            _selectedIndex = (_selectedIndex + 1) % NumEntries;
        }
        else if (inputUp || Input.GetButtonDown("MenuUp"))
        {
            changed = true;
            _selectedIndex = (_selectedIndex + NumEntries - 1) % NumEntries;
        }
        if (changed)
        {
            Director.Instance.PlaySfx(MoveSfx);
            Vector3 pos = _menuArrow.transform.localPosition;
            pos.y = -0.06f - 0.13f * _selectedIndex;
            _menuArrow.transform.localPosition = pos;
        }
    }
}
