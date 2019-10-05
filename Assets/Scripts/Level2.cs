using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2 : Level
{
    private GameObject _wallsWhite;
    private GameObject _wallsGray;

    public override IEnumerator StartLevel()
    {
        _wallsWhite = GameObject.Find("WallsWhite");
        _wallsGray = GameObject.Find("WallsGray");
        _wallsWhite.SetActive(true);
        _wallsGray.SetActive(true);

        Director.Instance.Player.SetColor(LogicColor.White);

        yield return new WaitForSeconds(1.0f);
    }
}
