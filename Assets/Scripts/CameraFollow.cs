using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Target;
    public float Speed = 0.1f;

    void Start()
    {
        
    }

    void Update()
    {
        float x = Target.transform.position.x;
        float y = Target.transform.position.y;
        float t = Speed * Time.deltaTime;
        x = Mathf.Lerp(gameObject.transform.position.x, x, t);
        y = Mathf.Lerp(gameObject.transform.position.y, y, t);
        x = Mathf.Round(x * 16.0f) / 16.0f;
        y = Mathf.Round(y * 16.0f) / 16.0f;
        gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
    }
}
