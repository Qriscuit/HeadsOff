using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FeviTop : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;
    public FeviTopParent FeviTopBallPrefab;
    public FeviTopChild FeviTopChildPrefab;
    public GameObject FevitopSplashVFX;

    [Header("FeviTop")]
    public float nreflection;
    public float CoolDownTime;
    public float FTmoveSpeed;
    public float TimeToSpawnGlue;

    public bool isFeviTopAllowedL;
    public bool isFeviTopAllowedR;

    public float SlowedSpeed;
    public float SlowedSpeedDelay;

    private void Start()
    {
        isFeviTopAllowedL = true;
        isFeviTopAllowedR = true;
    }

    public void Launch(Vector3 PositionToSpawnBall, Vector3 direction, int LeftRight)
    {
        if(LeftRight==0 && isFeviTopAllowedL)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingL();
            _WM.CMD_SpawnFeviTop(PositionToSpawnBall, direction, FTmoveSpeed);
            isFeviTopAllowedL = false;
            StartCoroutine(StartCoolDownL());

        }
        if(LeftRight ==1 && isFeviTopAllowedR )
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingR();
            _WM.CMD_SpawnFeviTop(PositionToSpawnBall, direction, FTmoveSpeed);
            isFeviTopAllowedR = false;
            StartCoroutine(StartCoolDownR());

        }
    }

    IEnumerator StartCoolDownL()
    {
        DOVirtual.Float(0, 1, CoolDownTime, updateLeftUI);
        yield return new WaitForSeconds(CoolDownTime);
        isFeviTopAllowedL = true;
    }
    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_FeviTop.fillAmount = value;
    }

    IEnumerator StartCoolDownR()
    {
        DOVirtual.Float(0, 1, CoolDownTime, updateRightUI);
        yield return new WaitForSeconds(CoolDownTime);
        isFeviTopAllowedR = true;
    }
    void updateRightUI(float value)
    {
        _WM._BPM._UIM.L_FeviTop.fillAmount = value;
    }
}

