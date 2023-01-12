using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    public GameObject checkmark;

    public void CheckmarkBox(bool _checked)
    {
        if(checkmark != null)
        {
            checkmark.SetActive(_checked);
        }
    }

}
