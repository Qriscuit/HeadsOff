using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations.Rigging;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using DG.Tweening;
using HeadsOffGlobals;
public class AnimationHandler : NetworkBehaviour
{
    BasePlayerManager _BPM;

    public FlexNetworkAnimator _fna;

    public Vector2 PlayerInputs;
    public void SetPlayerInputsForAnimations(Vector2 Inputs)
    {
        PlayerInputs = new Vector2(Inputs.x, Inputs.y);
    }

    private void Awake()
    {
        _BPM = GetComponent<BasePlayerManager>();

        DOVirtual.Float(0.99f, 0f, 0.1f, setLeftAimRigWeight);
        DOVirtual.Float(0.99f, 0f, 0.1f, setRightAimRigWeight);//because we can't keep weight 0 from inspector for some weired unity reason
                                                               //DOVirtual.Float(0.99f, 0f, 0.3f, setRightRecoilWeight);
    }

    private void OnEnable()
    {
        _BPM._IM._MovementDir += SetPlayerInputsForAnimations;
    }

    private void OnDisable()
    {
        _BPM._IM._MovementDir -= SetPlayerInputsForAnimations;
    }


    float RightLerp = 0;
    float ForwardLerp = 0;
    private void Update()
    {
        if (_BPM.BodyInPossession != null && isLocalPlayer)
        {
            Debug.Log("My isLocalPlayer is = " + isLocalPlayer + " and my connectionId is = " + netId);

            RightLerp = Mathf.Lerp(_BPM._NNBody._Animator.GetFloat("Right"), PlayerInputs.x, 0.1f);
            ForwardLerp = Mathf.Lerp(_BPM._NNBody._Animator.GetFloat("Forward"), PlayerInputs.y, 0.1f);

            _BPM._NNBody._Animator.SetFloat("Right", RightLerp);
            _BPM._NNBody._Animator.SetFloat("Forward", ForwardLerp);
        }
    }

    public void SetJumpingTrue()
    {
        _BPM._NNBody._Animator.SetBool("Jumping", true);
    }

    public void SetDoubleJumpTrue()
    {
        _BPM._NNBody._Animator.SetBool("Jumping", true);
    }

    public void SetFallingTrue()
    {
        _BPM._NNBody._Animator.SetBool("Falling", true);
    }


    public void SetLanded(bool Value)
    {
        _BPM._NNBody._Animator.SetBool("Landed", Value);
    }

    [Command]
    public void CMD_setlANDED(bool Value)
    {
        _BPM._NNBody._Animator.SetBool("Landed", Value); ;
    }

    [ClientRpc]
    void CLNT_setlANDED(bool Value)
    {
        _BPM._NNBody._Animator.SetBool("Landed", Value); ;
    }

    public void ResetJumpBooleans()
    {
        _BPM._NNBody._Animator.SetBool("Jumping", false);
        _BPM._NNBody._Animator.SetBool("Falling", false);
        _BPM._NNBody._Animator.SetBool("Landed", false);
    }

    public void ActivateAnimRiggingRightHand()
    {
        DOVirtual.Float(0, 0.99f, 0.1f, setRightAimRigWeight);//.OnComplete(setRightRecoil);
    }

    public void DeActivateAnimRiggingRightHand()
    {
        DOVirtual.Float(0.99f, 0f, 0.3f, setRightAimRigWeight);
    }

    public bool IsLeftArmInPositionToShoot()
    {
        if (_BPM._NNBody.LeftHandRig.weight > 0.9f)
            return true;
        else
            return false;
    }

    public bool IsRightArmInPositionToShoot()
    {
        if (_BPM._NNBody.RightHandRig.weight > 0.9f)
            return true;
        else
            return false;
    }


    void setRightAimRigWeight(float weight)
    {
        _BPM._NNBody.RightHandRig.data.targetPositionWeight = weight;
    }

    /*
    void setRightRecoil()
    {
        if (_BPM.BodyInPossession._rightHandType == HandType.FlameThrower)
            return;
        Debug.Log("log");
        //DOVirtual.Float(0f, 0.99f, 0.3f, setRightRecoilWeight).OnComplete(resetRightRecoil);
    }
    void resetRightRecoil()
    {
        DOVirtual.Float(0.99f, 0f, 0.3f, setRightRecoilWeight);//.OnComplete(DeActivateAnimRiggingRightHand);
    }
    void setRightRecoilWeight(float weight)
    {
        _BPM._Body.RHRecoilRig.data.targetPositionWeight = weight;
    }
    */

    public void ActivateAnimRiggingLefttHand()
    {
        DOVirtual.Float(0, 0.99f, 0.1f, setLeftAimRigWeight);
    }

    public void DeActivateAnimRiggingLeftHand()
    {
        DOVirtual.Float(0.99f, 0f, 0.3f, setLeftAimRigWeight);
    }

    void setLeftAimRigWeight(float weight)
    {
        _BPM._NNBody.LeftHandRig.data.targetPositionWeight = weight;
    }

    float FadeValue;
    [ClientRpc] public void LaunchBattery()
    {
        _BPM._NNBody._ShieldAccentsAnimator.Play("Shield Pop Off");
        FadeValue = _BPM._NNBody._ShieldAccentsRenderer.materials[1].GetFloat("EdgeWidth");
        DOTween.To(() => FadeValue, x => _BPM._NNBody._ShieldAccentsRenderer.materials[1].SetFloat("EdgeWidth", x), 0, 0.6f).OnComplete(()=> { _BPM._NNBody._ShieldAccents.SetActive(false); });
    }

    [ClientRpc] public void ReAttachBattery()
    {
        _BPM._NNBody._ShieldAccents.SetActive(true);
        _BPM._NNBody._ShieldAccentsAnimator.Play("Shield Pop In");
        FadeValue = _BPM._NNBody._ShieldAccentsRenderer.materials[1].GetFloat("EdgeWidth");
        DOTween.To(() => FadeValue, x => _BPM._NNBody._ShieldAccentsRenderer.materials[1].SetFloat("EdgeWidth", x), 1, 0.6f);
    }
}
