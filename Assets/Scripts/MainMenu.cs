using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameObject _menuArrow;
    private int _selectedIndex = 0;

    void Start()
    {
        _menuArrow = transform.Find("SelectArrow").gameObject;
    }

    void Update()
    {
        float y = Input.GetAxis("Vertical");
        if (y != 0)
        {
            const int NumEntries = 2;
            if (y > 0f)
            {
                _selectedIndex = (_selectedIndex + 1) % NumEntries;
            }
            else
            {
                _selectedIndex = (_selectedIndex + NumEntries - 1) % NumEntries;
            }
            Vector3 pos = _menuArrow.transform.localPosition;
            pos.y = -0.06f - 0.13f * _selectedIndex;
            _menuArrow.transform.localPosition = pos;
        }
    }
}
