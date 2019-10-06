using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        Director.Instance.PlayerController.enabled = false;
        var sr = Director.Instance.Player.GetComponent<SpriteRenderer>();
        var co = FadeToHole(Director.Instance.Player.transform, sr);
        StartCoroutine(co);
    }

    IEnumerator FadeToHole(Transform xform, SpriteRenderer sr)
    {
        const int numSteps = 50;
        float dt = 1.0f / numSteps;
        float t = 1.0f;
        Vector3 originalScale = xform.localScale;
        for (int i = 0; i < numSteps; ++i)
        {
            t -= dt;
            sr.color = Color.white * t;
            xform.localScale = originalScale * t;
            yield return null;
        }

        Director.Instance.RestartLevel();
    }
}
