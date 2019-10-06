using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HoleTrigger : MonoBehaviour
{
    public AudioClip FallClip;

    CompositeCollider2D _selfCollider;
    BoxCollider2D _playerCollider;

    void Awake()
    {
        _selfCollider = GetComponent<CompositeCollider2D>();
        FallClip?.LoadAudioData();
    }

    void Update()
    {
        if (_playerCollider != null)
        {
            // Check if the player box fits completely inside the collider, which
            // means there's nothing left outside the hole, so can fall inside.
            // This allows larger player to walk "over" smaller holes.
            bool allInside = true;
            allInside = allInside && _selfCollider.OverlapPoint(_playerCollider.bounds.min);
            allInside = allInside && _selfCollider.OverlapPoint(new Vector2(_playerCollider.bounds.min.x, _playerCollider.bounds.max.y));
            allInside = allInside && _selfCollider.OverlapPoint(_playerCollider.bounds.max);
            allInside = allInside && _selfCollider.OverlapPoint(new Vector2(_playerCollider.bounds.max.x, _playerCollider.bounds.min.y));
            if (allInside)
            {
                // Fall into the hole and die
                var director = Director.Instance;
                director.PlayerController.enabled = false;
                director.PlaySfx(FallClip);
                var sr = Director.Instance.Player.GetComponent<SpriteRenderer>();
                var co = FadeToHole(Director.Instance.Player.transform, sr);
                StartCoroutine(co);

                // Forget about the player, so avoid repeating all of that
                _playerCollider = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        _playerCollider = (collider as BoxCollider2D);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        _playerCollider = null;
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
