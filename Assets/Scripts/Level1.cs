using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    private PlayerController _playerController;
    private GameObject _player;
    private SpriteRenderer _playerRenderer;
    private GameObject _grid;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _playerController.enabled = false;
        _playerRenderer = _player.GetComponent<SpriteRenderer>();
        _playerRenderer.enabled = false;
        _grid = GameObject.Find("Walls");
        _grid.SetActive(false);
    }

    void Start()
    {
        var co = StartLevel();
        StartCoroutine(co);
    }

    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1.0f);

        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "In the beginning, there was nothing." });
            yield return Director.Instance.StartDialogImpl(dialog, block: true);
        }

        yield return new WaitForSeconds(1.0f);

        {
            var co = FadePlayerIn();
            StartCoroutine(co);
        }
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "Then came life." });
            yield return Director.Instance.StartDialogImpl(dialog, block: true);
        }

        yield return new WaitForSeconds(1.0f);

        {
            var co = FadeLevelIn();
            StartCoroutine(co);
        }
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "And walls." });
            dialog.Snippets.Add(new TextSnippet { text = "Because nothing is easy.", append = true });
            yield return Director.Instance.StartDialogImpl(dialog, block: true);
        }

        _playerController.enabled = true;

        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "But admitedly that was boring." });
            yield return Director.Instance.StartDialogImpl(dialog, block: false);
        }

        yield return new WaitForSeconds(1.0f);

        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "So ." });
            yield return Director.Instance.StartDialogImpl(dialog, block: false);
        }
    }

    IEnumerator FadePlayerIn()
    {
        _playerRenderer.color = Color.black;
        _playerRenderer.enabled = true;
        float t = 0.0f;
        const int numSteps = 150;
        for (int i = 0; i < numSteps; ++i)
        {
            t += (1.0f / numSteps);
            _playerRenderer.color = Color.white * t;
            yield return null;
        }
    }

    IEnumerator FadeLevelIn()
    {
        yield return new WaitForSeconds(1.0f);
        _grid.SetActive(true);
    }
}
