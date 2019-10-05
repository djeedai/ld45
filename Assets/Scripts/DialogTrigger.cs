using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string Dialog;
    public bool OneShot = true;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        if (OneShot)
        {
            enabled = false;
        }
        var dialog = new TextDialog();
        dialog.Snippets.Add(new TextSnippet { text = Dialog });
        Director.Instance.StartDialog(dialog, block: false);
    }
}
