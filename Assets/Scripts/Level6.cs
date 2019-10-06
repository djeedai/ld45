using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6 : Level
{
    public override IEnumerator StartLevel()
    {
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "Soon some shapes started to appear." });
            yield return Director.Instance.WaitForDialog(dialog, block: true);
        }

        yield return new WaitForSeconds(1.0f);
    }
}
