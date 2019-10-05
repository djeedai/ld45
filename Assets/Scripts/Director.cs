using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSnippet
{
    public string text;
    public bool append = false;
}

public class TextDialog
{
    public List<TextSnippet> Snippets = new List<TextSnippet>();
}

public class Director : MonoBehaviour
{
    public static Director Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("Director").GetComponent<Director>();
            }
            return _instance;
        }
    }

    private static Director _instance;

    private PlayerController _playerController;
    private TMPro.TextMeshPro _text;
    private GameObject _frame;
    private GameObject _button;
    private IEnumerator _coroutine;

    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _text = GetComponentInChildren<TMPro.TextMeshPro>();
        _frame = GameObject.Find("Frame");
        _button = GameObject.Find("SkipButton");

        _text.enabled = false;
        _frame.SetActive(false);
        _button.SetActive(false);
    }

    public void StartDialog(TextDialog dialog, bool block = true)
    {
        _coroutine = StartDialogImpl(dialog, block);
        StartCoroutine(_coroutine);
    }

    public IEnumerator StartDialogImpl(TextDialog dialog, bool block)
    {
        bool wasActive = _playerController.enabled;
        if (block)
        {
            _playerController.enabled = false;
        }
        _text.enabled = true;
        _frame.SetActive(true);

        foreach (var snippet in dialog.Snippets)
        {
            if (snippet.append)
            {
                _text.text += "\n" + snippet.text;
            }
            else
            {
                _text.text = snippet.text;
            }
            var co_bt = ShowButtonWithDelay(2.0f);
            StartCoroutine(co_bt);
            yield return null; // wait for next loop so the GetButtonDown() reset
            yield return StartCoroutine(WaitForUse());
            StopCoroutine(co_bt);
            _button.SetActive(false);
        }

        _text.enabled = false;
        _frame.SetActive(false);
        if (block)
        {
            _playerController.enabled = wasActive;
        }
    }

    IEnumerator WaitForUse()
    {
        while (!Input.GetButtonDown("Use"))
            yield return null;
    }

    IEnumerator ShowButtonWithDelay(float dt)
    {
        yield return new WaitForSeconds(dt);
        _button.SetActive(true);
    }
}
