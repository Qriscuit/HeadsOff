using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashLegs : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;
    public MasterInputManager _IM;

    [Header("DashLegsVar")]
    public float DashMoveSpeed;
    public float DashTime;
    public float DashCoolDown;
    public bool IsDashAllowed = true;
    bool IsDashing;
    char Direction;

 //ek dash use krne pe meter bhrna chalu ho jata hai, waise tiin(three) dash honge  
 


    private void OnEnable()
    {
        _IM._MovementDir += MovementButtonDL;
    }

    private void OnDisable()
    {
        _IM._MovementDir -= MovementButtonDL;
    }

    Vector2 MovementDir;

    void MovementButtonDL(Vector2 _MovementDir)
    {
        MovementDir = _MovementDir;
    }

    public void Launch()
    {
        if (IsDashAllowed)
        {
            _WM._BPM._MH.IsMovementAllowed = false;

            //if the player press shift and after that he pressed movement keys the script will not register inputs as launch()
            //is called in one frame and in that particular frame player have to press movement keys
            //it only works while player is moving in a direction and pressing movement keys

            //if (MovementDir.y > 0)//W  
            //{
            //    Direction = 'W';

            //    //ValuesForAnimator += new Vector3(0, 0, 1);
            //}
            //if (MovementDir.x < 0)//A
            //{
            //    Direction = 'W';

            //    //ValuesForAnimator += new Vector3(-1, 0, 0);
            //}
            //if (MovementDir.y < 0)//S
            //{
            //    Direction = 'W';

            //    //ValuesForAnimator += new Vector3(0, 0, -1);
            //}
            //if (MovementDir.x > 0)//D
            //{
            //    Direction = 'W';

            //    //ValuesForAnimator += new Vector3(1, 0, 0);
            //}
            
            IsDashAllowed = false;
            IsDashing = true;
            _WM._BPM._AM._fna.SetTrigger("Dash");
            _WM._BPM._VFX.CMD_VFX_LDashStart();
            _WM._BPM._VFX.VFX_DashActive(DashTime,true);
            StartCoroutine(DashCoolcheck());
        }
    }

    private void Update()
    {
        if(IsDashing)
        {
            _WM._BPM._MH._CC.Move(transform.forward * DashMoveSpeed * Time.deltaTime);

            //switch (Direction)
            //{
            //    case 'W':
                  
            //        break;

            //    case 'A':
            //        _WM._BPM._MH._CC.Move(-transform.right*DashMoveSpeed*Time.deltaTime);
            //        break;

            //    case 'S':
            //        _WM._BPM._MH._CC.Move(-transform.forward * DashMoveSpeed * Time.deltaTime);
            //        break;

            //    case 'D':
            //        _WM._BPM._MH._CC.Move(transform.right * DashMoveSpeed * Time.deltaTime);
            //        break;
            //}

        }
    }

    IEnumerator DashCoolcheck()
    {
        yield return new WaitForSeconds(DashTime);
        _WM._BPM._AM._fna.SetTrigger("Dash Halt");
        _WM._BPM._VFX.CMD_VFX_LDashStop();

        IsDashing = false;
        _WM._BPM._MH.IsMovementAllowed = true;
        DOVirtual.Float(0, 1, DashCoolDown, UpdateLegUI);
        yield return new WaitForSeconds(DashCoolDown);
        IsDashAllowed = true;
    }

    void UpdateLegUI(float value)
    {
        _WM._BPM._UIM.Dash.fillAmount = value;
    }
}
