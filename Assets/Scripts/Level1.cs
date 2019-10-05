using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : Level
{
    private PlayerController _playerController;
    private Player _player;
    private SpriteRenderer _playerRenderer;
    private GameObject _wallsWhite;
    private GameObject _wallsGray;

    void Start()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        _playerController = playerGameObject.GetComponent<PlayerController>();
        _player = playerGameObject.GetComponent<Player>();
        _playerRenderer = playerGameObject.GetComponent<SpriteRenderer>();

        _player.SetColor(LogicColor.Disabled);
        _playerController.enabled = false;
        _playerRenderer.enabled = false;
    }

    public override IEnumerator StartLevel()
    {
        _wallsWhite = GameObject.Find("WallsWhite");
        _wallsGray = GameObject.Find("WallsGray");
        _wallsWhite.SetActive(false);
        _wallsGray.SetActive(false);

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
            var co = FadeWallsIn(_wallsWhite);
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
            dialog.Snippets.Add(new TextSnippet { text = "But admittedly that was boring." });
            yield return Director.Instance.StartDialogImpl(dialog, block: false);
        }

        yield return new WaitForSeconds(1.0f);

        _player.SetColor(LogicColor.White);
        {
            var co = FadeWallsIn(_wallsGray);
            StartCoroutine(co);
        }
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "So came the shades." });
            dialog.Snippets.Add(new TextSnippet { text = "Some say they were 50 of them.", append = true });
            dialog.Snippets.Add(new TextSnippet { text = "But here we have less.", append = true });
            yield return Director.Instance.StartDialogImpl(dialog, block: true);
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

    IEnumerator FadeWallsIn(GameObject walls)
    {
        yield return new WaitForSeconds(1.0f);
        walls.SetActive(true); //< TODO - fade
    }
}
