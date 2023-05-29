using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For now, a parent for UI to tell localization to "flip this" if possible
public class UIScript : MonoBehaviour
{
    public bool supportsRightToLeftUI;
    [Tooltip("Don't add ArabicFixer to the component. For change translation section.")]
    public bool dontFixArabic;
}
