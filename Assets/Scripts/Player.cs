using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogicColor
{
    Disabled,
    White,
    Gray,
    LightGray
}

public static class LogicColorExtensions
{
    public static Color ToColor(this LogicColor logicColor)
    {
        switch (logicColor)
        {
        case LogicColor.Disabled:
        default:
            return new Color(0, 0, 0, 0);

        case LogicColor.White:
            return new Color(1, 1, 1);

        case LogicColor.Gray:
            return new Color(99f / 255f, 99f / 255f, 99f / 255f);

        case LogicColor.LightGray:
            return new Color(163f / 255f, 163f / 255f, 163f / 255f);
        }
    }
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
        GetComponent<SpriteRenderer>().color = color.ToColor();
        Director.Instance.DisableLayer(LogicColor);
    }

    public void OnPickUp(PickUp pickUp)
    {
        switch (pickUp.Type)
        {
        case PickUpType.ColorChange:
            SetColor(pickUp.NewColor);
            break;
        }
    }
}
