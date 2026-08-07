using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Mirror;
using Cinemachine;
using DG.Tweening;

public class VFXHandler : NetworkBehaviour
{
    [SerializeField] BasePlayerManager _BPM;

    [Header("VFX Prefab ref")]
    public ParticleSystem _LDashLegL;
    public ParticleSystem _LDashLegR;

    [Space]
    public ParticleSystem _LsJumpLegTwistL;
    public ParticleSystem _LsJumpLegTwistR;
    public ParticleSystem _LsJumpLegCloud;

    [Space]
    public ParticleSystem _LBullRush;

    [Space]
    public ParticleSystem _HmuzzleSmokeL;
    public ParticleSystem _HmuzzleSmokeR;

    [Space]
    public ParticleSystem _HmuzzleElectricL;
    public ParticleSystem _HmuzzleElectricR;

    [Space]
    public ParticleSystem _HmuzzleRingL;
    public ParticleSystem _HmuzzleRingR;

    [Space]
    public ParticleSystem _HportalRingL;
    public ParticleSystem _HportalRingR;

    [Space]
    public ParticleSystem _BelectricLineL;
    public ParticleSystem _BelectricLineR;

    [Space]
    public ParticleSystem _CjetPackL;
    public ParticleSystem _CjetPackR;

    [Space]
    public ParticleSystem _HeadLaunch;

    
    [Space]
    public ParticleSystem _HeadLaunchBody;

    [Space]
    public ParticleSystem _HeadFlight;

    [Space]
    public ParticleSystem _SpeedLine;

    [Space]
    public GameObject _BodyDestroyed;

    [Header("FeedBacks")]
    [SerializeField] MMFeedbacks FB_FlameDamage;
    public MMFeedbacks FB_CamShake;
    [SerializeField] public MMFeedbacks FB_DeathGreyScale;
    [SerializeField] MMFeedbacks FB_StickyGlue;
    [SerializeField] MMFeedbacks FB_ShieldDamage;
    [SerializeField] MMFeedbacks FB_GeneralDamageFlash;
    [SerializeField] MMFeedbacks FB_ElectroballStun;
    [SerializeField] MMFeedbacks FB_BullrushStun;

    
    #region HeadLaunch
    [Command]
    public void CMD_VFX_HeadLaunch()
    {
        _HeadLaunch.Play();
        CLNT_VFX_HeadLaunch();
    }
    [ClientRpc]
    public void CLNT_VFX_HeadLaunch()
    {
        _HeadLaunch.Play();
    }

    [Command]
    public void CMD_VFX_HeadLaunchBody()
    {
        _HeadLaunchBody.Play();
        CLNT_VFX_HeadLaunchBody();
    }
    [ClientRpc]
    public void CLNT_VFX_HeadLaunchBody()
    {
        _HeadLaunchBody.Play();
    }
    #endregion

    #region HeadFlight
    [Command]
    public void CMD_VFX_HeadFlight()
    {
        if (_HeadFlight.isPlaying)
            return;
        _HeadFlight.Play();
        CLNT_VFX_HeadFlight();
    }
    [ClientRpc]
    public void CLNT_VFX_HeadFlight()
    {
        _HeadFlight.Play();
    }

    [Command]
    public void CMD_VFX_HeadFlightStop()
    {
        _HeadFlight.Stop();
        CLNT_VFX_HeadFlightStop();
    }
    [ClientRpc]
    public void CLNT_VFX_HeadFlightStop()
    {
        _HeadFlight.Stop();
    }
    #endregion


    #region DashLegs
    [Command]
    public void CMD_VFX_LDashStart()
    {
        _LDashLegL.Play();
        _LDashLegR.Play();
        CLNT_VFX_LDashStart();
    }

    [ClientRpc]
    public void CLNT_VFX_LDashStart()
    {
        _BPM._PAM.Dash.Play();
        Debug.Log("clientstart");
        _LDashLegL.Play();
        _LDashLegR.Play();
        Debug.Log("PS playing in code" + _LDashLegL.isPlaying);

    }

    [Command]
    public void CMD_VFX_LDashStop()
    {
        _LDashLegL.Stop();
        _LDashLegR.Stop();
        CLNT_VFX_LDashStop();
    }

    [ClientRpc]
    public void CLNT_VFX_LDashStop()
    {
        _BPM._PAM.Dash.Stop();
        Debug.Log("clientsop");
        _LDashLegR.Stop();
        _LDashLegL.Stop();
    }
    #endregion

    #region SuperJump
    [Command]
    public void CMD_VFX_SjumpStart()
    {
        _LsJumpLegTwistL.Play();
        _LsJumpLegTwistR.Play();
        _LsJumpLegCloud.Play();
        CLNT_VFX_SjumpStart();
    }

    [ClientRpc]
    public void CLNT_VFX_SjumpStart()
    {
        _BPM._PAM.SuperJump.Play();
        _LsJumpLegTwistL.Play();
        _LsJumpLegTwistR.Play();
        _LsJumpLegCloud.Play();
    }

    [Command]
    public void CMD_VFX_SjumpStop()
    {
        _LsJumpLegTwistL.Stop();
        _LsJumpLegTwistR.Stop();
        _LsJumpLegCloud.Stop();
        CLNT_VFX_SjumpStop();
    }

    [ClientRpc]
    public void CLNT_VFX_SjumpStop()
    {
        _LsJumpLegTwistL.Stop();
        _LsJumpLegTwistR.Stop();
        _LsJumpLegCloud.Stop();
    }
    #endregion

    #region BullRush

    [Command]
    public void CMD_VFX_LbullRushStart()
    {
        _LBullRush.Play();
        Debug.Log("working");
        CLNT_VFX_LbullRushStart();
    }
    [ClientRpc]
    public void CLNT_VFX_LbullRushStart()
    {
        _BPM._PAM.BullRush.Play();
        _LBullRush.Play();
    }

    [ClientRpc]
    public void CLNT_VFX_LbullRushStop()
    {
        _BPM._PAM.BullRush.Stop();
        _LBullRush.Stop();
    }
    public void VFX_LbullRushStop()
    {
        _BPM._PAM.BullRush.Stop();
        _LBullRush.Stop();
    }

    #endregion

    #region muzzleSmoke
    [Command]
    public void CMD_VFX_HmuzzleSmokeL()
    {
        _HmuzzleSmokeL.Play();
        CLNT_VFX_HmuzzleSmokeL();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleSmokeL()
    {
        _BPM._PAM.BigShotShot.Play();
        _HmuzzleSmokeL.Play();
    }

    [Command]
    public void CMD_VFX_HmuzzleSmokeR()
    {
        _HmuzzleSmokeR.Play();
        CLNT_VFX_HmuzzleSmokeR();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleSmokeR()
    {
        _BPM._PAM.BigShotShot.Play();
        _HmuzzleSmokeR.Play();
    }
    #endregion

    #region muzzleElectric
    [Command]
    public void CMD_VFX_HmuzzleElectricL()
    {
        _HmuzzleElectricL.Play();
        CLNT_VFX_HmuzzleElectricL();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleElectricL()
    {
        _BPM._PAM.Electroball.Play();
        _HmuzzleElectricL.Play();
    }

    [Command]
    public void CMD_VFX_HmuzzleElectricR()
    {
        _HmuzzleElectricR.Play();
        CLNT_VFX_HmuzzleElectricR();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleElectricR()
    {
        _BPM._PAM.Electroball.Play();
        _HmuzzleElectricR.Play();
    }
    #endregion

    #region muzzleRing
    [Command]
    public void CMD_VFX_HmuzzleRingL()
    {
        _HmuzzleRingL.Play();
        CLNT_VFX_HmuzzleRingL();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleRingL()
    {
        _HmuzzleRingL.Play();
    }

    [Command]
    public void CMD_VFX_HmuzzleRingR()
    {
        _HmuzzleRingR.Play();
        CLNT_VFX_HmuzzleRingR();
    }
    [ClientRpc]
    public void CLNT_VFX_HmuzzleRingR()
    {
        _HmuzzleRingR.Play();
    }
    #endregion

    #region Portal
    [Command]
    public void CMD_VFX_HportalStartL()
    {
        _HportalRingL.Play();
        CLNT_VFX_HportalStartL();
    }

    [ClientRpc]
    public void CLNT_VFX_HportalStartL()
    {
        _HportalRingL.Play();
    }

    [Command]
    public void CMD_VFX_HportalStopL()
    {
        _HportalRingL.Stop();
        CLNT_VFX_HportalStopL();
    }

    [ClientRpc]
    public void CLNT_VFX_HportalStopL()
    {
        _HportalRingL.Stop();
    }

    [Command]
    public void CMD_VFX_HportalStartR()
    {
        _HportalRingR.Play();
        CLNT_VFX_HportalStartR();
    }

    [ClientRpc]
    public void CLNT_VFX_HportalStartR()
    {
        _HportalRingR.Play();
    }

    [Command]
    public void CMD_VFX_HportalStopR()
    {
        _HportalRingR.Stop();
        CLNT_VFX_HportalStopR();
    }

    [ClientRpc]
    public void CLNT_VFX_HportalStopR()
    {
        _HportalRingR.Stop();
    }
    #endregion

    #region electriclines
    [Command]
    public void CMD_VFX_BelectricLineL()
    {
        _BelectricLineL.Play();
        CLNT_VFX_BelectricLineL();
    }

    [Command]
    public void CMD_VFX_BelectricLineStopL()
    {
        _BelectricLineL.Stop();
        CLNT_VFX_BelectricLineStopL();
    }


    [ClientRpc]
    public void CLNT_VFX_BelectricLineL()
    {
        _BelectricLineL.Play();
    }


    [ClientRpc]
    public void CLNT_VFX_BelectricLineStopL()
    {
        _BelectricLineL.Stop();
    }

    [Command]
    public void CMD_VFX_BelectricLineR()
    {
        _BelectricLineR.Play();
        CLNT_VFX_BelectricLineR();
    }
    [Command]
    public void CMD_VFX_BelectricLineStopR()
    {
        _BelectricLineR.Stop();
        CLNT_VFX_BelectricLineStopR();
    }

    [ClientRpc]
    public void CLNT_VFX_BelectricLineStopR()
    {
        _BelectricLineR.Stop();
    }

    [ClientRpc]
    public void CLNT_VFX_BelectricLineR()
    {
        _BelectricLineR.Play();
    }
    #endregion

    #region JetPack
    [Command]
    public void CMD_VFX_CjetPackStart()
    {
        _CjetPackL.Play();
        _CjetPackR.Play();
        CLNT_VFX_CjetPackStart();
    }

    [ClientRpc]
    public void CLNT_VFX_CjetPackStart()
    {
        _CjetPackL.Play();
        _CjetPackR.Play();
            _BPM._PAM.JetPack.Play();
    }

    [Command]
    public void CMD_VFX_CjetPackStop()
    {
        _CjetPackL.Stop();
        _CjetPackR.Stop();
        CLNT_VFX_CjetPackStop();
    }

    [ClientRpc]
    public void CLNT_VFX_CjetPackStop()
    {
        _CjetPackL.Stop();
        _CjetPackR.Stop();
        _BPM._PAM.JetPack.Stop();
    }
    #endregion

    #region dash
    public void VFX_DashActive(float Time,bool AutoStop)
    {
        _SpeedLine.Play();
        DOVirtual.Float(65, 100, 0.2f, scaleFieldOfView);

        if (AutoStop)
            StartCoroutine(VFX_DashDeActivate(Time));
    }

    IEnumerator VFX_DashDeActivate(float Time)
    {
        yield return new WaitForSeconds(Time);
        VFX_DashActiveStop();
    }

    public void VFX_DashActiveStop()
    {
        _SpeedLine.Stop();
        DOVirtual.Float(100, 65, 0.2f, DescaleFieldOfView);
    }

    void DescaleFieldOfView(float value)
    {
        _BPM._CMHead.m_Lens.FieldOfView = value;
        _BPM._CMBody.m_Lens.FieldOfView = value;
    }

    void scaleFieldOfView(float value)
    {

        _BPM._CMHead.m_Lens.FieldOfView = value;
        _BPM._CMBody.m_Lens.FieldOfView = value;
    }

    #endregion

    //---------------------------------------------------------//

    #region FlameDamage
    [ClientRpc]
    public void FBCall_FlameDamageStart()
    {
        if (!isLocalPlayer)
            return;

        if (!FB_FlameDamage.IsPlaying)
        FB_FlameDamage.PlayFeedbacks();
    }

    [ClientRpc]
    public void FBCall_FlameDamageStop()
    {
        if (!isLocalPlayer)
            return;

        FB_FlameDamage.StopFeedbacks();
    }
    #endregion

    #region CameraShake
    [ClientRpc]
    public void FBCall_CamShakeStart(int Time)
    {
        if (!isLocalPlayer)
            return;

        if (!FB_CamShake.IsPlaying)
        {
            FB_CamShake.PlayFeedbacks();
            //StartCoroutine(StopCamShake(Time));
        }
    }

    IEnumerator StopCamShake(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_CamShakeStop();
    }

    [ClientRpc]
    public void FBCall_CamShakeStop()
    {
        if (!isLocalPlayer)
            return;

        FB_CamShake.StopFeedbacks();
    }
    #endregion

    #region DeathScreen
    [ClientRpc]
    public void FBCall_DeathGreyScaleStart()
    {
        if (!isLocalPlayer)
            return;

        if (!FB_DeathGreyScale.IsPlaying)
        {
            FB_DeathGreyScale.PlayFeedbacks();
            //StartCoroutine(StopDeathGreyScale(Time));
        }
    }

    IEnumerator StopDeathGreyScale(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_DeathGreyScaleStop();
    }
    [ClientRpc]
    public void FBCall_DeathGreyScaleStop()
    {
        if (!isLocalPlayer)
            return;

        FB_DeathGreyScale.StopFeedbacks();
    }
    #endregion

    #region StickyGlue
    [ClientRpc]
    public void FBCall_StickyGlueStart(int Time)
    {
        if (!isLocalPlayer)
            return;

        if (!FB_StickyGlue.IsPlaying)
        {
            FB_StickyGlue.PlayFeedbacks();
            //StartCoroutine(StopStickyGlue(Time));
        }
    }

    IEnumerator StopStickyGlue(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_StickyGlueStop();
    }
    public void FBCall_StickyGlueStop()
    {
        if (!isLocalPlayer)
            return;

        FB_DeathGreyScale.StopFeedbacks();
    }
    #endregion

    #region ShieldDamage
    [ClientRpc]
    public void FBCall_ShieldDamageStart()
    {
        if (!isLocalPlayer)
            return;

        if (!FB_ShieldDamage.IsPlaying)
        {
            FB_ShieldDamage.PlayFeedbacks();
            //StartCoroutine(StopShieldDamage(Time));
        }
    }

    IEnumerator StopShieldDamage(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_ShieldDamageStop();
    }
    [ClientRpc]
    public void FBCall_ShieldDamageStop()
    {
        if (!isLocalPlayer)
            return;

        FB_ShieldDamage.StopFeedbacks();
    }
    #endregion

    #region GeneralDamage
    [ClientRpc]
    public void FBCall_GeneralDamageFlashStart(int Time)
    {
        if (!isLocalPlayer)
            return;

        if (!FB_GeneralDamageFlash.IsPlaying)
        {
            FB_GeneralDamageFlash.PlayFeedbacks();
            //StartCoroutine(StopGeneralDamageFlash(Time));
        }
    }

    IEnumerator StopGeneralDamageFlash(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_GeneralDamageFlashStop();
    }
    [ClientRpc]
    public void FBCall_GeneralDamageFlashStop()
    {
        if (!isLocalPlayer)
            return;

        FB_GeneralDamageFlash.StopFeedbacks();
    }
    #endregion

    #region ElectroballStun
    [ClientRpc]
    public void FBCall_ElectroballStunStart()
    {
        if (!isLocalPlayer)
            return;

        if (!FB_ElectroballStun.IsPlaying)
        {
            FB_ElectroballStun.PlayFeedbacks();
            //StartCoroutine(StopElectroballStun(Time));
        }
    }

    IEnumerator StopElectroballStun(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_ElectroballStunStop();
    }
    [ClientRpc]
    public void FBCall_ElectroballStunStop()
    {
        if (!isLocalPlayer)
            return;

        FB_ElectroballStun.StopFeedbacks();
    }
    #endregion

    #region BullrushStun
    [ClientRpc]
    public void FBCall_BullrushStunStart(int Time)
    {
        if (!isLocalPlayer)
            return;

        if (!FB_BullrushStun.IsPlaying)
        {
            FB_BullrushStun.PlayFeedbacks();
            //StartCoroutine(StopBullrushStun(Time));
        }
    }

    IEnumerator StopBullrushStun(int Time)
    {
        yield return new WaitForSeconds(Time);
        FBCall_BullrushStunStop();
    }
    [ClientRpc]
    public void FBCall_BullrushStunStop()
    {
        if (!isLocalPlayer)
            return;

        FB_BullrushStun.StopFeedbacks();
    }
    #endregion

    public void CMD_DestroyBody()
    {
        GameObject bodyDestroy = Instantiate(_BodyDestroyed, this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(bodyDestroy);
    }


    [ClientRpc]
    public void CLNT_PlayHittingEnemy()
    {
        if (!isLocalPlayer)
            return;

        _BPM._PAM.HittingEnemy.Play();
    }

    [ClientRpc]
    public void CLNT_PlayKillEnemy()
    {
        if (!isLocalPlayer)
            return;

        _BPM._PAM.KillEnemy.Play();
    }
}
