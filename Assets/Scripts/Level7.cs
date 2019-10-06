using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level7 : Level
{
    public override IEnumerator StartLevel()
    {
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "Finally some color appeared." });
            yield return Director.Instance.WaitForDialog(dialog, block: false);
        }

        yield return new WaitForSeconds(1.0f);
    }
}
