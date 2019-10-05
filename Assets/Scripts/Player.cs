using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            var dialog = new TextDialog();
            dialog.Snippets.Add(new TextSnippet { text = "This is the first line." });
            dialog.Snippets.Add(new TextSnippet { text = "This is the second line." });
            dialog.Snippets.Add(new TextSnippet { text = "This is the third line." });
            Director.Instance.StartDialog(dialog);
        }
    }

    public void OnPickUp(PickUp pickUp)
    {
        switch (pickUp.Type)
        {
        case PickUpType.ColorChange:
            GetComponent<SpriteRenderer>().color = pickUp.NewColor;
            break;
        }
    }
}
