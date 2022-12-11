using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTeleportationAnchor : MonoBehaviour
{
    public GameObject m_Cylinder;
    void Start()
    {
        if (m_Cylinder != null)
        {
            m_Cylinder.SetActive(false);
            ActivateTeleportationRay.onTeleportRayActivated += ToggleTeleportationAnchor;
        }
    }
    
    private void ToggleTeleportationAnchor(bool b)
    {
        if (m_Cylinder == null) return;
        m_Cylinder.SetActive(b);
    }
}
