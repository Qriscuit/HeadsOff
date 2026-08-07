using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HeadsOffGlobals;
using Mirror;
using DG.Tweening;

public class UI_Manager : NetworkBehaviour
{
    [Header("Health")]
    public Image HeadHealthUI;
    public Image BodyHealthUI;
    public Image ShieldHealthUI;

    public TMP_Text BodyHealthText;
    public TMP_Text HeadHealthText;
    public TMP_Text ShieldHealthText;
    
    [Header("CrossHair Stuff")]
    public Image Crosshair;
    public CanvasGroup DamageCross;
    public CanvasGroup DeathCross;

    [Header("Misc")]
    public Canvas _Canvas;
    public BasePlayerManager _BPM;
    int health;
    bool BodyOccupied;

    [Header("Weapon UI")]
    public GameObject WeaponUI;

    [Header("Chest")]
    public Image JetPack;
    public Image Shield;
    public Image PumpUp;

    [Header("Left Hand")]
    public Image L_FeviTop;
    public Image L_BigShot;
    public Image L_BubbleGun;
    public Image L_ExplodingMine;
    public Image L_FlameThrower;
    public Image L_PA;
    public Image L_Portal;
    public Image L_PunchGlove;
    public Image L_ElectroBall;

    [Header("Right Hand")]
    public Image R_FeviTop;
    public Image R_BigShot;
    public Image R_BubbleGun;
    public Image R_ExplodingMine;
    public Image R_FlameThrower;
    public Image R_PA;
    public Image R_Portal;
    public Image R_PunchGlove;
    public Image R_ElectroBall;

    [Header("Legs")]
    public Image SuperJump;
    public Image Dash;
    public Image Stomp;
    public Image BullRush;
    
    [Space]
    public ChestType CurrentChestType;
    public HandType CurrentLHType;
    public HandType CurrentRHType;
    public LegType CurrentLegType;

    [Header("HealthCanvas")]
    public Canvas HealthCanvas;
    public Image EnemyHealth;
    public Image FriendHealth;
    public Image HealthCanvasBackground;


    float FillAmount;

    int MyPlayerIndex;
    [HideInInspector] [SyncVar] public bool UpdateGMHealthUI;
    public void UpdateHealthUI()
    {
        if (!isLocalPlayer || isServer) return;

        if (BodyOccupied)
        {
            if (_BPM._NNBody.chestType == ChestType.Shield && _BPM._WM._Shield._ShieldHealth > 1)
            {
                ShieldHealthUI.fillAmount = _BPM._WM._Shield._ShieldHealth / 50f;
                FillAmount = _BPM._WM._Shield._ShieldHealth / 50f;
                health = (int)_BPM._WM._Shield._ShieldHealth;
                BodyHealthText.text = health.ToString();
            }
            else
            {
                ShieldHealthUI.fillAmount = 0;
                BodyHealthUI.fillAmount = _BPM._BodyInPossession._BodyHealth / 150f;
                FillAmount = _BPM._BodyInPossession._BodyHealth / 150f;
                health = (int)_BPM._BodyInPossession._BodyHealth;
                BodyHealthText.text = health.ToString();
            }
        }
        else
        {
            HeadHealthUI.fillAmount = _BPM._HeadHealth / 100f;
            FillAmount = _BPM._HeadHealth / 100f;
            health = (int)_BPM._HeadHealth;
            HeadHealthText.text = health.ToString();
        }

        
        if(UpdateGMHealthUI) CMD_UpdateAllPlayersHealth(FillAmount);
    }
    
    [Command]
    private void CMD_UpdateAllPlayersHealth(float fillAmount)
    {
        CLNT_UpdateThisPlayersHealthUI(fillAmount);
    }

    [ClientRpc]
    private void CLNT_UpdateThisPlayersHealthUI(float fillAmount)
    {
        if (_BPM._Team == Team.Red)
        {
            MyPlayerIndex = Room.RedGamePlayers.IndexOf(_BPM);
            if (MyPlayerIndex == 0)
                GameManager.Inst.Red1Health.fillAmount = fillAmount;
            else
                GameManager.Inst.Red2Health.fillAmount = fillAmount;
        }
        else
        {
            MyPlayerIndex = Room.RedGamePlayers.IndexOf(_BPM);
            if (MyPlayerIndex == 0)
                GameManager.Inst.Blue1Health.fillAmount = fillAmount;
            else
                GameManager.Inst.Blue2Health.fillAmount = fillAmount;
        }
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    public void UpdatePlayerUI(bool Attached)
    {
        WeaponUI.SetActive(Attached);
        BodyOccupied = Attached;

        if (Attached)
        {
            HeadHealthUI.transform.parent.gameObject.SetActive(false);
            BodyHealthUI.transform.parent.gameObject.SetActive(true);

            HeadHealthText.gameObject.SetActive(false);
            BodyHealthText.gameObject.SetActive(true);

            Crosshair.gameObject.SetActive(true);

            if(_BPM.BodyInPossession.chestType == ChestType.Shield)
            {
                ShieldHealthUI.gameObject.SetActive(true);
            }
            else
            {
                ShieldHealthUI.gameObject.SetActive(false);
            }
            
            UpdateWeaponUI();
        }
        else
        {
            Crosshair.gameObject.SetActive(false);

            HeadHealthUI.transform.parent.gameObject.SetActive(true);
            BodyHealthUI.transform.parent.gameObject.SetActive(false);

            ShieldHealthUI.gameObject.SetActive(false);

            HeadHealthText.gameObject.SetActive(true);
            BodyHealthText.gameObject.SetActive(false);
        }
    } 

    void UpdateWeaponUI()
    {
        UpdateChestWeaponUI();
        UpdateLeftHandWeaponUI();
        UpdateRightHandWeaponUI();
        UpdateLegWeaponUI();
    }

    private void UpdateChestWeaponUI()
    {
        switch (CurrentChestType)
        {
            case HeadsOffGlobals.ChestType.JetPack:
                JetPack.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.ChestType.PumpUp:
                PumpUp.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.ChestType.Shield:
                Shield.transform.parent.gameObject.SetActive(false);
                break;
        }

        switch (_BPM._NNBody.chestType)
        {
            case HeadsOffGlobals.ChestType.JetPack:
                JetPack.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.ChestType.PumpUp:
                PumpUp.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.ChestType.Shield:
                Shield.transform.parent.gameObject.SetActive(true);
                break;
        }

        CurrentChestType = _BPM._NNBody.chestType;
    }

    private void UpdateLegWeaponUI()
    {
        switch (CurrentLegType)
        {
            case HeadsOffGlobals.LegType.BullRush:
                BullRush.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.LegType.Dash:
                Dash.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.LegType.Stomp:
                Stomp.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.LegType.SuperJump:
                SuperJump.transform.parent.gameObject.SetActive(false);
                break;
        }

        switch (_BPM._NNBody.legsType)
        {
            case HeadsOffGlobals.LegType.BullRush:
                BullRush.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.LegType.Dash:
                Dash.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.LegType.Stomp:
                Stomp.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.LegType.SuperJump:
                SuperJump.transform.parent.gameObject.SetActive(true);
                break;
        }

        CurrentLegType = _BPM._NNBody.legsType;
    }

    private void UpdateRightHandWeaponUI()
    {
        switch (CurrentRHType)
        {
            case HeadsOffGlobals.HandType.BigShot:
                R_BigShot.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.BubbleGun:
                R_BubbleGun.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.ElectroBall:
                R_ElectroBall.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.ExplodingMines:
                R_ExplodingMine.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.FeviTop:
                R_FeviTop.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.FlameThrower:
                R_FlameThrower.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.ParticleAccelerator:
                R_PA.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.PortalGun:
                R_Portal.transform.parent.gameObject.SetActive(false);
                break;

            case HeadsOffGlobals.HandType.PunchGlove:
                R_PunchGlove.transform.parent.gameObject.SetActive(false);
                break;
        }

        switch (_BPM._NNBody.rightHandType)
        {
            case HeadsOffGlobals.HandType.BigShot:
                R_BigShot.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.BubbleGun:
                R_BubbleGun.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ElectroBall:
                R_ElectroBall.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ExplodingMines:
                R_ExplodingMine.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.FeviTop:
                R_FeviTop.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.FlameThrower:
                R_FlameThrower.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ParticleAccelerator:
                R_PA.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.PortalGun:
                R_Portal.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.PunchGlove:
                R_PunchGlove.transform.parent.gameObject.SetActive(true);
                break;
        }

        CurrentRHType = _BPM._NNBody.rightHandType;
    }


    private void UpdateLeftHandWeaponUI()
    {
        switch (CurrentLHType)
        {
            case HeadsOffGlobals.HandType.BigShot:
                L_BigShot.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.BubbleGun:               
                L_BubbleGun.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.ElectroBall:             
                L_ElectroBall.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.ExplodingMines:          
                L_ExplodingMine.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.FeviTop:                 
                L_FeviTop.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.FlameThrower:            
                L_FlameThrower.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.ParticleAccelerator:     
                L_PA.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.PortalGun:               
                L_Portal.transform.parent.gameObject.SetActive(false);
                break;                                             
                                                                   
            case HeadsOffGlobals.HandType.PunchGlove:              
                L_PunchGlove.transform.parent.gameObject.SetActive(false);
                break;
        }

        switch (_BPM._NNBody.leftHandType)
        {
            case HeadsOffGlobals.HandType.BigShot:
                L_BigShot.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.BubbleGun:
                L_BubbleGun.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ElectroBall:
                L_ElectroBall.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ExplodingMines:
                L_ExplodingMine.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.FeviTop:
                L_FeviTop.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.FlameThrower:
                L_FlameThrower.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.ParticleAccelerator:
                L_PA.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.PortalGun:
                L_Portal.transform.parent.gameObject.SetActive(true);
                break;

            case HeadsOffGlobals.HandType.PunchGlove:
                L_PunchGlove.transform.parent.gameObject.SetActive(true);
                break;
        }

        CurrentLHType = _BPM._NNBody.leftHandType;
    }

    private void OnEnable()
    {
        _BPM._bodyAttached += UpdatePlayerUI;
        _BPM._NNBody._ChestChanged += UpdateChestWeaponUI;
        _BPM._NNBody._LHChanged += UpdateLeftHandWeaponUI;
        _BPM._NNBody._RHChanged += UpdateRightHandWeaponUI;
        _BPM._NNBody._LegsChanged += UpdateLegWeaponUI;
    }

    private void OnDisable()
    {
        _BPM._bodyAttached -= UpdatePlayerUI;
        _BPM._NNBody._ChestChanged -= UpdateChestWeaponUI;
        _BPM._NNBody._LHChanged -= UpdateLeftHandWeaponUI;
        _BPM._NNBody._RHChanged -= UpdateRightHandWeaponUI;
        _BPM._NNBody._LegsChanged -= UpdateLegWeaponUI;
    }

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    [ClientRpc]
    public void MakeDamageCrossHairAppear()
    {
        if (!isLocalPlayer) return;

        DamageCross.alpha = 0;
        DOTween.To(() => DamageCross.alpha, x => DamageCross.alpha = x, 1, 0.2f).OnComplete(MakeDamageCrossHairDisappear);
    }

    public void MakeDamageCrossHairDisappear()
    {
        DOTween.To(() => DamageCross.alpha, x => DamageCross.alpha = x, 0, 0.2f);
    }

    [ClientRpc]
    public void MakeKillCrossHairAppear()
    {
        if (!isLocalPlayer) return;

        DeathCross.alpha = 0;
        DOTween.To(() => DeathCross.alpha, x => DeathCross.alpha = x, 1f, 0.2f).OnComplete(() => Invoke("MakeKillCrossHairDisappear", 2f));
    }

    public void MakeKillCrossHairDisappear()
    {
        DOTween.To(() => DeathCross.alpha, x => DeathCross.alpha = x, 0, 0.2f);
    }
}
