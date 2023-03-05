using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
public class ChestCompressionTrial : MonoBehaviour
{
    public LiveLineDrawer _GraphScript;
    public GameObject chest;
    public float rate = 2;
    private bool _CompressionPressed;
    [Range(0, 1)]
    public float compressionSize;
    public float chestCompressionAmountScale = 0.25f;
    public float _TrialDuration = 20.0f;
    public const float DEFAULT_TIME_TRIAL = 20.0f;
    private float currentCompressionAmount; 
    [SerializeField] AudioClip _CompressionSound;
    [Header("Hands")]
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject _CPRHand;
    public bool handsInRange;
    public void BeginTrial()
    {
        _GraphScript.gameObject.SetActive(true);
        _GraphScript.rate = rate;
        _GraphScript.StartGraphs(this);
        GameObject plyr = Util.GetPlayer();
        leftHand = plyr.GetPlayerHandObject();
        rightHand = plyr.GetPlayerHandObject(true);
        ToggleCPRHand(true);
        currentCompressionAmount = 1.0f;
        enabled = true;
    }

    public void OnTrialFinish()
    {
        QuestManager qm = QuestManager._Instance;
        if (qm.IsQuestType("Do Chest Compression")) qm.ForceCompleteQuest();
        ToggleCPRHand(false);
        enabled = false;
    }
    
    void Update()
    {
        if(!_GraphScript.enabled) return;
        if(leftHand == null || rightHand == null) 
        {
            Debug.LogWarning("Hands are undefined or missing, this is bad!!!");
            return;
        }
        float ccValue = CalculatePlayerHandPosition();
        currentCompressionAmount = Mathf.Lerp(currentCompressionAmount, ccValue, Time.deltaTime * 20);
        _GraphScript.OnCompressionGraphInfo(ccValue);
        if(currentCompressionAmount <= 0.4f && !_CompressionPressed)
        {
            _GraphScript.OnCompressionRecieved();
            _CompressionPressed = true;
            AudioSource.PlayClipAtPoint(_CompressionSound, transform.position, 3.0f);
        }
        else if(currentCompressionAmount >= 0.7f && _CompressionPressed)
        {
            _CompressionPressed = false;
        }
        chest.transform.localScale = new Vector3 (chest.transform.localScale.x, chest.transform.localScale.y, Mathf.Lerp(1 - chestCompressionAmountScale, 1, ccValue));
    }

    float CalculatePlayerHandPosition()
    {
        // Make sure they are close to each other
        if (Util.Vector3_Distance2D(leftHand.transform.position, rightHand.transform.position) > compressionSize)
        {
            if(handsInRange) ToggleCPRHand(false);
            return 1.0f;
        }
        // How far is the center?
        Vector3 centerPoint = (leftHand.transform.position + rightHand.transform.position)/2;
        
        if (!handsInRange) ToggleCPRHand(true);
        SetCPRHandPosition(centerPoint);
        
        if(Vector3.Distance(centerPoint, transform.position) > compressionSize) return 1.0f;
        
        // Get distance between the compressor and the hand
        float yDistance = centerPoint.y - transform.position.y;
        // Clamp between Negative and Positive of Half of the Y scale size
        yDistance = Mathf.Clamp(yDistance, transform.localScale.y/-2, transform.localScale.y/2);
        // Offset by half and lerp between 0 - 1, depending on the actual scale
        return Mathf.Lerp(0,1, (transform.localScale.y/2 + yDistance) / transform.localScale.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.1f, 0.1f, 1, 0.35f);
        Gizmos.DrawCube(transform.position, new Vector3(compressionSize, compressionSize, compressionSize));
    }

    void SetCPRHandPosition(Vector3 pos) => _CPRHand.transform.position = pos;
    void ToggleCPRHand(bool _enabled)
    {
        leftHand.transform.ToggleHidden(_enabled);
        rightHand.transform.ToggleHidden(_enabled);
        _CPRHand.transform.ToggleHidden(!_enabled);
        handsInRange = _enabled;
    }
}
