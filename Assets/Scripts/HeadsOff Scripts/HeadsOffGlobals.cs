using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeadsOffGlobals
{
    public enum ChestType
    {
        JetPack, Printing, PumpUp, Shield, NA
    }

    public enum LegType
    {
        BullRush, Dash, Stomp, SuperJump, NA
    }
    
    public enum HandType
    {
        BigShot, BubbleGun, ElectroBall, ExplodingMines, FeviTop, FlameThrower, Latch, ParticleAccelerator, PortalGun, PunchGlove, MachineGun, Sniper, ShotGun, NA
    }
    
    public enum BodyPart
    {
        C, L, LH, RH
    }

    public enum Team
    {
        Red, Blue, NA, Spectator
    }

    public struct MeshData
    {
        public ChestType _Chest;
        public LegType _Legs;
        public HandType _Hands;

        public List<SkinnedMeshRenderer> AllSkinnedMeshRenderers;
    }
}