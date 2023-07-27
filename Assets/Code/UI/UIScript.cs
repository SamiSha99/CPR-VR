using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For now, a parent for UI to tell localization to "flip this" if possible
public class UIScript : MonoBehaviour
{
    public bool supportsRightToLeftUI;
    [Tooltip("Don't add ArabicFixer to the component. For change translation section.")]
    public bool dontFixArabic;
    [HideInInspector] public bool isRight;
    void Awake()
    {
        bool rtl = LocalizationHelper.UsingRightToLeftLanguage();
        if(rtl == isRight) return;
        if(!gameObject.HasComponent<RectTransform>(out RectTransform rt)) return;
        LocalizationHelper.FlipComponent(rt);
        isRight = LocalizationHelper.UsingRightToLeftLanguage();
    }
}
