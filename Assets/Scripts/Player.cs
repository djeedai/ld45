using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogicColor
{
    Disabled,
    White,
    Gray
}

public class Player : MonoBehaviour
{
    public LogicColor LogicColor { get; private set; } = LogicColor.Disabled;

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

    public void SetColor(LogicColor color)
    {
        if (color == LogicColor)
        {
            return;
        }

        LogicColor = color;
        Director.Instance.DisableLayer(LogicColor);
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
