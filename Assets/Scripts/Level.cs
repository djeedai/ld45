using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public AudioClip BackgroundMusicIntro;
    public AudioClip BackgroundMusicLoop;

    public virtual IEnumerator StartLevel()
    {
        // Default implementation does nothing, immediately return
        yield break;
    }
}
