using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SuperJump : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("SuperJump")]
    public float SuperJumpSpeed;
    public bool IsSuperJumpAllowed = true;
    public float RecharageTime;
    public ParticleSystem _SuperJumpVFX;
    public ParticleSystem _SuperJumpBaseCloudPufft;
    public ParticleSystem _SuperJumpTwist;
    public ParticleSystem _SuperJumpTwist1;
    public void Launch()
    {
        if(IsSuperJumpAllowed)
        {
            _WM._BPM._MH.AppliedMovement.y = SuperJumpSpeed;
            _WM._BPM._MH.MovementVector.y = SuperJumpSpeed;

            _WM._BPM._MH.IsJumping = true;
            _WM._BPM._AM._fna.SetTrigger("SuperJump");
            _WM._BPM._NNBody._Animator.SetBool("Landed", false);
            _WM._BPM._NNBody._Animator.SetBool("Falling", true);

            //Instantiate(_SuperJumpBaseCloudPufft, transform.position, Quaternion.identity);
            _SuperJumpVFX.Play();
            //_SuperJumpTwist.Play();
            //_SuperJumpTwist1.Play();

            _WM._BPM._VFX.CMD_VFX_SjumpStart();

            IsSuperJumpAllowed = false;
            StartCoroutine(_WaitUntil());
        }

    }

    IEnumerator _WaitUntil()
    {
        DOVirtual.Float(0, 1, RecharageTime, UpdateLegUI);
        yield return new WaitForSeconds(RecharageTime);
        Debug.Log(_WM._BPM._MH._CC.isGrounded);
        IsSuperJumpAllowed = true;
    }

    void UpdateLegUI(float value)
    {
        _WM._BPM._UIM.SuperJump.fillAmount = value;
    }
}
