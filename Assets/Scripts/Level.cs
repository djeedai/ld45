using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : MonoBehaviour
{
    public AudioClip BackgroundMusicIntro;
    public AudioClip BackgroundMusicLoop;

    public abstract IEnumerator StartLevel();
}
