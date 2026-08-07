using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalLevelManager : MonoBehaviour
{
    public Transform RedSpawnParent, BlueSpawnParent, BodySpawnParent, SpectatorSpawnParent;

    [HideInInspector] public Transform[] RedSpawns;
    [HideInInspector] public Transform[] BlueSpawns;
    [HideInInspector] public Transform[] BodySpawns;
    [HideInInspector] public Transform[] SpectatorSpawns;

    public float OceanFactoryMaxHeight = 23f;
    public float SkyscraperMaxHeight = 2;
    public float JunkYardMaxHeight = 2;

    int RedIndex;
    int BlueIndex;
    int BodyIndex;
    int SpectatorIndex;

    public static LocalLevelManager Inst;
    private void Awake()
    {
        Inst = this;
    }

    public void DeleteNMGC(GameObject NMGC)
    {
        Destroy(NMGC);
        SceneManager.LoadScene(0);
    }

    public void PopulateListValues()
    {
        RedSpawns = new Transform[RedSpawnParent.childCount];
        BlueSpawns = new Transform[BlueSpawnParent.childCount];
        BodySpawns = new Transform[BodySpawnParent.childCount];
        SpectatorSpawns = new Transform[SpectatorSpawnParent.childCount];

        for (int i = 0; i < RedSpawnParent.childCount; i++)
        {
            RedSpawns[i] = RedSpawnParent.GetChild(i);
        }

        for (int i = 0; i < BlueSpawnParent.childCount; i++)
        {
            BlueSpawns[i] = BlueSpawnParent.GetChild(i);
        }

        for (int i = 0; i < BodySpawnParent.childCount; i++)
        {
            BodySpawns[i] = BodySpawnParent.GetChild(i);
        }

        for (int i = 0; i < SpectatorSpawnParent.childCount; i++)
        {
            SpectatorSpawns[i] = SpectatorSpawnParent.GetChild(i);
        }
    }
}
