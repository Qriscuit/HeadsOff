using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Globals
{
    [SerializeField]
    public struct BodyMeshStruct
    {
        [SerializeField]
        public List<Mesh> Meshes;
        [SerializeField]
        public List<Material> Materials;
    }

    [System.Serializable]
    public enum PlayerNumber
    {
        P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20
    }

    public enum MultiplayerMode
    {
        twoVtwo, threeVthree, fourVfour, fiveVfive, sixVsix
    }

    public enum TeamName
    {
        _null, RedTeam, BlueTeam, Spectator, TBot
    }
}
