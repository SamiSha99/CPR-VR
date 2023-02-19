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
    private GameObject leftHand, rightHand;
    [Range(0, 1)]
    public float compressionSize;
    public float chestCompressionAmountScale = 0.25f;
    public float _TrialDuration = 20.0f;
    public const float DEFAULT_TIME_TRIAL = 20.0f;
    private float currentCompressionAmount; 
    [SerializeField] AudioClip _CompressionSound;
    public void BeginTrial()
    {
        _GraphScript.gameObject.SetActive(true);
        _GraphScript.rate = rate;
        _GraphScript.StartGraphs(this);
        leftHand = Util.GetPlayer().GetPlayerHandObject();
        rightHand = Util.GetPlayer().GetPlayerHandObject(true);
        currentCompressionAmount = 1.0f;
        enabled = true;
    }

    public void OnTrialFinish()
    {
        QuestManager qm = QuestManager._Instance;
        if(qm.IsQuestType("Do Chest Compression")) qm.ForceCompleteQuest();
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
        else if(currentCompressionAmount >= 0.8f && _CompressionPressed)
        {
            _CompressionPressed = false;
        }
        chest.transform.localScale = new Vector3 (chest.transform.localScale.x, chest.transform.localScale.y, Mathf.Lerp(1 - chestCompressionAmountScale, 1, ccValue));
    }

    float CalculatePlayerHandPosition()
    {
        GameObject closestHand;
        // Get closest hand
        float leftHandGap = Vector3.Distance(leftHand.transform.position, transform.position);
        float rightHandGap = Vector3.Distance(rightHand.transform.position, transform.position);
        closestHand =  leftHandGap < rightHandGap ? leftHand : rightHand;
        if(Vector3.Distance(closestHand.transform.position, transform.position) > compressionSize) return 1.0f;
        
        // Get distance between the compressor and the hand
        float yDistance = closestHand.transform.position.y - transform.position.y;
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
}
