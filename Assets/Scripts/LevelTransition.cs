using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    public string NextLevelName;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player")
        {
            return;
        }
        var co = Director.Instance.LoadLevelAsync(NextLevelName);
        StartCoroutine(co);
    }
}
