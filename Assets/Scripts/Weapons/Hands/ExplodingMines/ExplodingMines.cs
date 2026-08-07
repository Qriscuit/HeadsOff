using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingMines : MonoBehaviour
{
    WeaponManager _WM;
    public GameObject _Mine;

    public float MineRespawnTime;

    public int MinesLaunchedL = 0;
    public int MinesLaunchedR = 0;
    
    [SerializeField] bool isAllowedL;
    [SerializeField] bool isAllowedR;

    private void Awake()
    {
        _WM = transform.parent.GetComponentInParent<WeaponManager>();
    }

    Coroutine MineRespawningL;
    Coroutine MineRespawningR;

    public void Launch(Vector3 _Dir, Vector3 SpawnPoint, int LeftRight)
    {
        if(MinesLaunchedL < 3 && LeftRight==0)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingL();
            _WM.CMD_SpawnMine(SpawnPoint, _Dir, true);
            MinesLaunchedL++;

            if (MineRespawningL == null) MineRespawningL = StartCoroutine(RespawnMinesL());
         
        }
        
        
        
        if (MinesLaunchedR < 3 && LeftRight == 1)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingR();
            _WM.CMD_SpawnMine(SpawnPoint, _Dir, false);
            MinesLaunchedR++;

            if (MineRespawningR == null) MineRespawningR = StartCoroutine(RespawnMinesR());
      
        }
    }

    void Update()
    {
        //if (MineRespawningL == null)
        //{
        //    Debug.Log("is the LEFT mine respawning : NO");
        //}
        //else
        //{
        //    Debug.Log("is the LEFT mine respawning : YES");
        //}

        //if (MineRespawningR == null)
        //{
        //    Debug.Log("is the RIGHT mine respawning : NO");
        //}
        //else
        //{
        //    Debug.Log("is the RIGHT mine respawning : YES");
        //}

    }

    IEnumerator RespawnMinesL()
    {
        Debug.Log("Entered the Left Respawn Corouine");
        while(MinesLaunchedL > 0)
        {
            Debug.Log("Entered the Loop Respawn Corouine current MinesLaunchedL are " + MinesLaunchedL);
            Debug.Log("About to start waiting for " + MineRespawnTime +" seconds");
            yield return new WaitForSeconds(MineRespawnTime);
            Debug.Log("waited for some time now mineslaunced is about decrease its current value is " + MinesLaunchedL);
            MinesLaunchedL--;
            Debug.Log("after decrement the value is " + MinesLaunchedL);
        }

        MineRespawningL = null;
    }

    IEnumerator RespawnMinesR()
    {
        while (MinesLaunchedR > 0)
        {
            yield return new WaitForSeconds(MineRespawnTime);
            MinesLaunchedR--;
        }

        MineRespawningR = null;
    }
}

//_Launched[MinesLaunched] = PhotonNetwork.Instantiate("Prefabs/Hands/Exploding Mines/Mine", SpawnPoint, Quaternion.identity).GetComponent<Mine>();
//_Launched[MinesLaunched].LaunchMineFromHand(_Dir);