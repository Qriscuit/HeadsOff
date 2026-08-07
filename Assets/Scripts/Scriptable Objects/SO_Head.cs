using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

[CreateAssetMenu]
public class SO_Head : ScriptableObject
{
    public string Name;
    public Mesh Mesh;
    public List<Material> RedMaterials;
    public List<Material> BlueMaterials;
    public Animation FunctionalAnimation;
    public Animation AccentAnimation;
}
