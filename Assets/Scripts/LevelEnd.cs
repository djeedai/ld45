using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : Level
{
    GameObject _outro;

    void Awake()
    {
        _outro = GameObject.Find("Outro");
        _outro.SetActive(false);
    }

    public override IEnumerator StartLevel()
    {
        Director.Instance.PlayerController.enabled = false;

        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "And that was it." });
            yield return Director.Instance.WaitForDialog(dialog, block: true);
        }

        yield return new WaitForSeconds(1.0f);

        var sr = GameObject.Find("Fader").GetComponent<SpriteRenderer>();
        const int numSteps = 180;
        float t = 0.3f;
        float dt = 0.7f / numSteps;
        for (int i = 0; i < numSteps; ++i)
        {
            t += dt;
            sr.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        _outro.SetActive(true);
        while (!Input.GetButtonDown("Use"))
            yield return null;

        Director.Instance.ExitGame();
    }
}
