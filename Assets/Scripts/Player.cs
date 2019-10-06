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

    private Vector3 _baseLocalScale;

    void Awake()
    {
        _baseLocalScale = transform.localScale;
    }

    public void SetScale(float scale)
    {
        _baseLocalScale = new Vector3(scale, scale, scale);
        transform.localScale = _baseLocalScale;
    }

    public void SetColor(LogicColor color, bool force = false)
    {
        if (!force && (color == LogicColor))
        {
            return;
        }

        LogicColor = color;
        GetComponent<SpriteRenderer>().color = color.ToColor();
        Director.Instance.DisableLayer(LogicColor);
    }

    public void SetDeltaSize(float deltaSize)
    {
        _baseLocalScale += new Vector3(deltaSize, deltaSize, deltaSize);
        _baseLocalScale = Vector3.Min(_baseLocalScale, new Vector3(2.0f, 2.0f, 2.0f)); // safety
        _baseLocalScale = Vector3.Max(_baseLocalScale, new Vector3(0.2f, 0.2f, 0.2f)); // safety
        transform.localScale = _baseLocalScale;
    }

    public void OnPickUp(PickUp pickUp)
    {
        switch (pickUp.Type)
        {
        case PickUpType.ColorChange:
            SetColor(pickUp.NewColor);
            break;

        case PickUpType.SizeChange:
            SetDeltaSize(pickUp.DeltaSize);
            break;
        }
    }
}
