using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType
{
    ColorChange,
    SizeChange
}

public class PickUp : MonoBehaviour
{
    public AudioClip PickUpSfx;
    public PickUpType Type = PickUpType.ColorChange;
    public LogicColor NewColor = LogicColor.White;
    public float DeltaSize = 0f;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = NewColor.ToColor();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Director.Instance.PlaySfx(PickUpSfx);
            collider.gameObject.GetComponent<Player>().OnPickUp(this);
            Destroy(gameObject);
        }
    }
}
