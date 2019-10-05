using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType
{
    ColorChange
}

public class PickUp : MonoBehaviour
{
    public PickUpType Type = PickUpType.ColorChange;
    public LogicColor NewColor = LogicColor.White;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = NewColor.ToColor();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            collider.gameObject.GetComponent<Player>().OnPickUp(this);
            Destroy(gameObject);
        }
    }
}
