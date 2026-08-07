using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KillBlockUI : MonoBehaviour
{
    public TMP_Text _KillersName;
    public TMP_Text _KilledsName;
    public Image _DeathImage;

    public Sprite BodyDeath;
    public Sprite HeadDeath;

    public float AliveTime = 4f;

    [Header("Data")]
    public string KillersNameData;
    public string KilledsNameData;
    public bool DidaHeadDieData;

    public void CreateNewKillBlockUI(string KillersName, string KilledsName, bool diddyakillHead)
    {
        KillersNameData = KillersName;
        KilledsNameData = KilledsName;
        DidaHeadDieData = diddyakillHead;

        _KillersName.text = KillersName;
        _KilledsName.text = KilledsName;

        _DeathImage.sprite = diddyakillHead ? HeadDeath : BodyDeath;

        Destroy(this.gameObject, AliveTime);
    }
}
