using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally Set Materials
public class ReplaceMaterials : MonoBehaviour
{
    public Material[] materials;
    public void SetMaterial() => this.SetMaterial(materials);
}
