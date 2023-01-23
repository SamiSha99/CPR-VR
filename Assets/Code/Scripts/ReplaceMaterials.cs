using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceMaterials : MonoBehaviour
{
    public Material[] materials;
    public void SetMaterial()
    {
        Renderer r = GetComponent<Renderer>();
        if(r == null) return;
        r.materials = materials;
    }
}
