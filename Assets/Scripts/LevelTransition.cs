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
        Director.Instance.ChangeLevelTo(NextLevelName);
    }
}
