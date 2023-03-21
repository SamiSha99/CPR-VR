using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartingPoint : MonoBehaviour
{
    void Start() => SetPlayerToPoint();
    public void SetPlayerToPoint(GameObject plyr = null)
    {
        if(plyr == null) plyr = Util.GetPlayer();
        if(plyr == null) return;
        plyr.transform.position = transform.position;
        plyr.transform.rotation = transform.rotation;       
    }
}
