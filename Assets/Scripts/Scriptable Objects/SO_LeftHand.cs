using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

[CreateAssetMenu]
public class SO_LeftHand : ScriptableObject
{
    public HandType Name;
    [Space]
    public Mesh Mesh;
    [Space]
    public List<Material> RedMaterials;
    public List<Material> BlueMaterials;
    public List<Material> GreyMaterials;
    [Space]
    public bool ExtraAccent;
    public Material RedAccentMaterial;
    public Material BlueAccentMaterial;
    public Material GreyAccentMaterial;
}
