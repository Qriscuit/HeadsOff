using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

[CreateAssetMenu]
public class SO_Legs : ScriptableObject
{
    public LegType Name;
    public Mesh Mesh;
    public List<Material> RedMaterials;
    public List<Material> BlueMaterials;
    public List<Material> GreyMaterials;
    public Animation FunctionalAnimation;
    public Animation AccentAnimation;
}
