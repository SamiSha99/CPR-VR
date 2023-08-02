using System;
using System.Collections;
using System.Collections.Generic;
using Bhaptics.SDK2;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

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
    public GameObject leftController, rightController;
    public GameObject _CPRHand;
    public bool handsInRange;

    private HandVisualizer handVisualizer;
    private Vector3 lastHandPosition;
    public void BeginTrial()
    {
        _GraphScript.gameObject.SetActive(true);
        _GraphScript.rate = rate;
        _GraphScript.StartGraphs(this);
        GameObject plyr = Util.GetPlayer();
        leftHand = plyr.GetPlayerHandObject(false, Util.HandType.HT_Hand);
        rightHand = plyr.GetPlayerHandObject(true, Util.HandType.HT_Hand);
        leftController = plyr.GetPlayerHandObject(false, Util.HandType.HT_Controller);
        rightController = plyr.GetPlayerHandObject(true, Util.HandType.HT_Controller);
        ToggleCPRHand(true);
        SetChestCompression(1);
        currentCompressionAmount = 1.0f;
        enabled = true;
        lastHandPosition = _CPRHand.transform.position;
    }

    public void OnTrialFinish(int finalCompressionAmount = 0)
    {
        QuestManager qm = QuestManager._Instance;
        GameManager gm = GameManager._Instance;
        if(finalCompressionAmount < 30)
        {
            Util.Print("NOT ENOUGH COMPRESSIONS!");
            gm.AddExamPenalty("ExamPenalty.InsufficentCompressions", 3.0f);
            qm.AddQuestToRetry();
        }
        if (qm.IsQuestType("CPR")) qm.ForceCompleteQuest();
        ToggleCPRHand(false);
        enabled = false;
        leftHand.transform.ToggleHidden(false);
        rightHand.transform.ToggleHidden(false);
        leftController.transform.ToggleHidden(false);
        rightController.transform.ToggleHidden(false);
        Util.Invoke(this, () => SetChestCompression(1), 0.01f);
        lastHandPosition = Vector3.zero;
    }
    
    void Update()
    {
        if(!_GraphScript.enabled) return;
        
        if(leftHand == null || rightHand == null || !leftHand.activeInHierarchy || !rightHand.activeInHierarchy) 
        {
            if(handVisualizer != null)
            leftHand = Util.GetPlayer().GetPlayerHandObject();
            rightHand = Util.GetPlayer().GetPlayerHandObject(true);
        }
        float ccValue = CalculatePlayerHandPosition();
        currentCompressionAmount = Mathf.Lerp(currentCompressionAmount, ccValue, Time.deltaTime * 20);
        _GraphScript.OnCompressionGraphInfo(ccValue);
        
        float hitSpeed = Vector3.Distance(lastHandPosition, _CPRHand.transform.position);
        //Util.Print("HIT SPEED:" + Mathf.Clamp01(hitSpeed*100/3).ToString("f4"));
        
        if(currentCompressionAmount <= 0.4f && !_CompressionPressed)
        {
            hitSpeed = Mathf.Clamp01(hitSpeed*100/2.5f);
            Util.Print("HIT SPEED:" + hitSpeed.ToString("f2"));
            _GraphScript.OnCompressionRecieved();
            _CompressionPressed = true;

            AudioSource.PlayClipAtPoint(_CompressionSound, transform.position, 3.0f);
            BhapticsLibrary.PlayParam(BhapticsEvent.LEFT_CPR_PRESS, 0.2f, 0.3f, 20.0f, 3.0f);
            BhapticsLibrary.PlayParam(BhapticsEvent.RIGHT_CPR_PRESS, 0.2f, 0.3f, 20.0f, 3.0f);
            
            // above 0.1875 and below to be ok 
            float inches = Mathf.Lerp(1.2f, 2.8f, hitSpeed);
            _GraphScript.OnCompressionDepthRecived(inches);
        }
        else if(currentCompressionAmount >= 0.7f && _CompressionPressed)
        {
            _CompressionPressed = false;
        }

        SetChestCompression(ccValue);
        lastHandPosition = _CPRHand.transform.position;
    }

    float CalculatePlayerHandPosition()
    {
        Vector3 centerPoint, leftHandTrackPosition, rightHandTrackPosition;
        
        bool isHands = Util.IsUsingHandTracking();

        // i hope this developer made lots of (code) sphagetti! luigi look!
        if(isHands && handVisualizer != null)
        {
            // the palm for now
            Transform leftPalm = handVisualizer.transform.Find("LeftHand(Clone)/L_Wrist/L_Palm");
            if(leftPalm != null)
                leftHandTrackPosition = leftPalm.position;
            else
                leftHandTrackPosition = Vector3.zero;
            Transform rightPalm = handVisualizer.transform.Find("RightHand(Clone)/R_Wrist/R_Palm");
            if(rightPalm != null)
                rightHandTrackPosition = rightPalm.position;
            else
                rightHandTrackPosition = Vector3.zero;

            GameObject camera = Util.GetPlayer().GetPlayerCameraObject();
    
            float leftHandDistance = leftHandTrackPosition == Vector3.zero ? Mathf.Infinity : Vector3.Distance(leftHandTrackPosition, camera.transform.position);
            float rightHandDistance = rightHandTrackPosition == Vector3.zero ? Mathf.Infinity : Vector3.Distance(rightHandTrackPosition, camera.transform.position);
            if(leftHandDistance == Mathf.Infinity && rightHandDistance == Mathf.Infinity && leftHandDistance == rightHandDistance) return 1.0f;

            centerPoint = leftHandDistance > rightHandDistance ? rightHandTrackPosition : leftHandTrackPosition;
        }

        // Make sure they are close to each other
        //if (leftHand == null || rightHand == null 
        //|| isHands && (rightHandTrackPosition == Vector3.zero && leftHandTrackPosition == Vector3.zero || Util.Vector3_Distance2D(leftHandTrackPosition, rightHandTrackPosition) > compressionSize * 2.0f) 
        //|| Util.Vector3_Distance2D(leftHand.transform.position, rightHand.transform.position) > compressionSize * 1.334f)
        //{
        //    if(handsInRange) ToggleCPRHand(false);
        //    return 1.0f;
        //}

        else
        {
            if(leftHand == null || rightHand == null) return 1.0f;
            centerPoint = (leftHand.transform.position + rightHand.transform.position)/2;
        }
        ToggleCPRHand(true);
        //if (!handsInRange) ToggleCPRHand(true);
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
    void SetChestCompression(float v)
    {
        chest.transform.localScale = new Vector3 (chest.transform.localScale.x, chest.transform.localScale.y, Mathf.Lerp(1 - chestCompressionAmountScale, 1, v));
    }
    void ToggleCPRHand(bool _enabled)
    {
        if(leftHand == null || rightHand == null)
        {
            _CPRHand.transform.ToggleHidden(true);
            return;
        }
        HandVisualizer hv = Util.GetHandVisualizer();
        handVisualizer = hv;
        hv.drawMeshes = !_enabled;
        leftHand?.transform.ToggleHidden(_enabled);
        rightHand?.transform.ToggleHidden(_enabled);
        _CPRHand.transform.ToggleHidden(!_enabled);
        handsInRange = _enabled;
    }
}
