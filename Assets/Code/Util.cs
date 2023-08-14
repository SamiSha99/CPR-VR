using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

// Utility Class
public static class Util
{
    //#######//
    // Paths //
    //#######//

    const string PATH_MEDIA = "Assets/Media/";
    const string PATH_ART = "Assets/Media/Art/";
    const string PATH_ICONS = "Assets/Media/Art/Icons/";
    const string PATH_TEXTURES = "Assets/Media/Art/Textures/";
    const string PATH_MATERIALS = "Assets/Media/Art/Materials/";
    const string PATH_SHADERS = "Assets/Media/Art/Shaders/";
    const string PATH_PARTICLES = "Assets/Media/Art/Particles/";
    const string PATH_OBJECTS = "Assets/Media/Art/Objects/";
    const string PATH_AUDIO_CLIPS = "Assets/Audio";
    // Path way search to specific items, just look up the name of the file + extension
    public static string GetIcon(string name)       => PATH_ICONS + name;
    public static string GetTexture(string name)    => PATH_TEXTURES + name;
    public static string GetMaterial(string name)   => PATH_MATERIALS + name;
    public static string GetShader(string name)     => PATH_SHADERS + name;
    public static string GetParticle(string name)   => PATH_PARTICLES + name;
    public static string GeObject(string name)      => PATH_OBJECTS + name;
    
    //########//
    // Consts //
    //########//

    public const float MICROPHONE_LOUDNESS_THRESHOLD = 0.04f;
    public const int MICROPHONE_LOUDNESS_MULTIPLIER = 135;
    public const string MICROPHONE_OCULUS_NAME = "Headset Microphone (Oculus Virtual Audio Device)"; // to-do: simplified name identifier?

    public const float INCH_TO_CENTIMETER = 2.54f;
    public const float CENTIMETER_TO_INCH = 0.393701f;

    public const string MENU_SCENE = "VR CPR Menu";

    //#######//
    // Debug //
    //#######//

    public enum PrintType
    {
        Normal,
        Warn,
        Error,
        Save
    };
    
    ///<summary>Prints a message with "FileName->FunctionName():LineNumber" attached for reference, with Log type of Normal/Warning/Error/Save.</summary>
    public static void Print(string msg, PrintType _type = PrintType.Normal, [CallerMemberName] string functionName = null, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        msg = Path.GetFileNameWithoutExtension(file) + "->" + functionName + "():" + line + " | " + msg;
        switch(_type)
        {
            case PrintType.Normal: default: Debug.Log($"<color='#F8F8FF'>{msg}</color>");   break;
            case PrintType.Save:   Debug.Log($"<color='cyan'>{msg}</color>");               break;
            case PrintType.Warn:   Debug.LogWarning($"<color='#FFC107'>{msg}</color>");     break;
            case PrintType.Error:  Debug.LogError($"<color='#FF534A'>{msg}</color>");       break;
        }
    }

    //############//
    // Extensions //
    //############//

    ///<summary>Returns true if the GameObject contains that component.</summary>
    public static bool HasComponent<T>(this GameObject obj) where T : Component => obj.GetComponent<T>();
    ///<summary>Returns true if the GameObject contains that component and an out result of that component.</summary>
    public static bool HasComponent<T>(this GameObject obj, out T componentResult) where T : Component => (componentResult = obj.GetComponent<T>()) != null;
    ///<summary>Find a component and return the component using a path similar to Transform.Find(); return null if couldn't find it.</summary>
    public static T FindComponent<T>(this Transform trans, string path) where T : Component
    {
        GameObject o = trans.Find(path).gameObject;
        if(o == null)
        {
            Print("Could not find the game object in FindComponent returned null", PrintType.Warn);
            return null;
        }
        T comp = o.GetComponent<T>();
        if(comp == null)
        {
            Print($"GameObject lacks the component {typeof(T)} returned null", PrintType.Warn);
            return null;
        }
        return comp;
    }
    /// <summary>Hides and deactivates Renderer, Collider and Rigidbody components, but the gameobject itself remains active</summary>
    public static void ToggleHidden(this Transform transform, bool _hidden = true)
    {
        if (transform == null) return;
        List<Transform> objects = null;
        objects = GetChildren(transform, objects, true);
        objects.Add(transform);
        if (objects.Count <= 0) return;
        
        foreach(Transform obj in objects)
        {
            GameObject go = obj.gameObject;
            if(go.HasComponent<Renderer>()) Array.ForEach(go.GetComponents<Renderer>(), x => x.enabled = !_hidden);
            if(go.HasComponent<Collider>()) Array.ForEach(go.GetComponents<Collider>(), x => x.enabled = !_hidden);
            if(go.HasComponent<Rigidbody>()) Array.ForEach(go.GetComponents<Rigidbody>(), x => x.isKinematic = _hidden);
        }
    }
    // Get all children of a parent and return into a transformList using a tree search
    // bool getChildrenOfChild returns all children of children of children and so on, 
    // if false will only return children connected to THIS transform parent
    public static List<Transform> GetChildren(this Transform parent, List<Transform> transformList = null, bool getChildrenOfChild = false)
    {
        if (transformList == null) transformList = new List<Transform>();
        
        foreach (Transform child in parent) 
        {
            transformList.Add(child);
            if(getChildrenOfChild) GetChildren(child, transformList, getChildrenOfChild);
        }

        return transformList;
    }
    // Returns the top most gameobject parent of this child
    public static GameObject GetRoot(this GameObject o) => o.transform.root.gameObject;
    // Returns the camera of this Owned player object (works with root too)
    public static GameObject GetPlayerCameraObject(this GameObject o) => o.GetRoot().transform.Find("CameraOffset/XR Camera").gameObject;

    public enum HandType
    {
        HT_Auto,
        HT_Controller,
        HT_Hand
    };
    // Returns the Left Hand of this Owned player object (works with root too)
    // param right - Returns the Right Hand instea
    // param type - force get a specific object regardless of state
    public static GameObject GetPlayerHandObject(this GameObject o, bool right = false, HandType type = 0)
    {
        GameObject hand = o.GetRoot().transform.Find("CameraOffset/" + (right ? "Right" : "Left") + " Hand/Direct Interactor").gameObject;
        if(hand != null && hand.activeInHierarchy || type == HandType.HT_Hand) return hand;
        GameObject controller = o.GetRoot().transform.Find("CameraOffset/" + (right ? "Right" : "Left") + " Controller").gameObject;
        if(controller != null && controller.activeInHierarchy || type == HandType.HT_Controller) return controller;
        return null;
    }
    public static GameObject GetPlayerXRHands(this GameObject o, bool right)
    {
        GameObject hand = o.GetRoot().transform.Find("CameraOffset/" + (right ? "Right" : "Left") + " Hand/Direct Interactor").gameObject;
        return hand;
    }
    public static HandVisualizer GetPlayerXRHandsVisualizer(this GameObject o)
    {
        HandVisualizer hv = o.GetRoot().transform.FindComponent<HandVisualizer>("CameraOffset/Hand Visualizer");
        return hv;
    }
    public static Vector3 GetPlayerXRHandsPosition(this GameObject o, bool right, string jointName = "")
    {
        HandVisualizer hv = o.GetPlayerXRHandsVisualizer();
        string path = (right ? "RightHand(Clone)/R_Wrist" : "LeftHand(Clone)/L_Wrist");
        if(jointName != "")
            path += "/" + (right ? "R_" : "L_") + jointName;

        Transform t = hv.transform.Find(path);
        return t != null ? t.position : Vector3.zero;
    }
    public static GameObject GetPlayer() => GameObject.FindWithTag("Player");
    public static XREvents GetXREvents(GameObject plyr = null)
    {
        if(plyr == null) plyr = GetPlayer(); // Didn't pass the player? Try looking for one.
        if(plyr == null)
        {
            Print("Cannot get XREvents, player does not exist?", PrintType.Warn);
            return null;
        }
        return plyr.transform.FindComponent<XREvents>("XREvents");
    }

    public static HandVisualizer GetHandVisualizer(GameObject plyr = null)
    {
        if(plyr == null) plyr = GetPlayer();
        if(plyr == null)
        {
            Print("Cannot get HandVisualizer, player does not exist?", PrintType.Warn);
            return null;
        }
        return plyr.transform.FindComponent<HandVisualizer>("CameraOffset/Hand Visualizer");
    }

    //####//
    // XR //
    //####//

    ///<summary>Returns if XR Hands is active.</summary>
    public static bool IsUsingHandTracking()
    {
        GameObject player = GetPlayer();
        GameObject leftHand = player.GetPlayerHandObject();
        GameObject rightHand = player.GetPlayerHandObject(true);

        if(leftHand == null || rightHand == null) return false;
        if(leftHand.name != "Direct Interactor" || rightHand.name != "Direct Interactor") return false;
        if(leftHand.transform.parent.gameObject == null || rightHand.transform.parent.gameObject == null) return false;

        return leftHand.transform.parent.gameObject.name == "Left Hand" || rightHand.transform.parent.gameObject.name == "Right Hand"; 
    }

    //########//
    // Quests //
    //########//

    public static bool IsQuestActive() => IsQuestManagerActive() && QuestManager.activeQuest != null;
    public static bool IsQuestManagerActive() => QuestManager._Instance != null;

    //########//
    // Invoke //
    //########//

    // Invoke that allows parameter passing via lambda expression
    // Example: Util.Invoke(this, () => MyFunctionName(parameter1, parameter2), 3.0f);
    public static void Invoke(this MonoBehaviour mb, Action f, float delay) => mb.StartCoroutine(InvokeRoutine(f, delay));
    
    // Not tested!!  
    public static void CancelInvoke(this MonoBehaviour mb, Action f) => mb.StopCoroutine(InvokeRoutine(f));
    
    private static IEnumerator InvokeRoutine(System.Action f, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        f();
    }

    //#############//
    // Conversions //
    //#############//

    public static bool IntToBool(int i) => i > 0;
    public static int BoolToInt(bool b) => b ? 1 : 0; 

    public static float ConvertInchesToCentimeters(float inches) => inches * INCH_TO_CENTIMETER; 
    public static float ConvertCentimetersToInches(float centimeters) => centimeters * CENTIMETER_TO_INCH; 
    
    //###########//
    // Materials //
    //###########//

    public static void SetMaterial(this MonoBehaviour mb, Material[] mats)
    {
        Renderer r = mb.GetComponent<Renderer>();
        if(r == null) return;
        r.materials = mats;
    }

    //#######//
    // Audio //
    //#######//

    // Get loudness of an audio clip
    // clipPosition => Current position of the clip (from AudioSource)
    // clip => The clip we are checking
    // sampleWindow => The amount of samples.
    public static float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip, int sampleWindow = 64)
    {
        int startPosition = clipPosition - sampleWindow;
        if(startPosition < 0) return 0;
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);
        float totalLoudness = 0;
        for(int i = 0; i < sampleWindow; i++) totalLoudness = Mathf.Abs(waveData[i]);
        return totalLoudness / sampleWindow;
    }
    // Gets the loudness of a microphone
    public static float GetLoudnessFromMicrophone(AudioClip microphoneClip, int sampleWindow = 64, int micDeviceIndex = 0)
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[micDeviceIndex]), microphoneClip, sampleWindow);
    }
    // Instantiate a Microphone Clip that listens to the microphone using the index.
    public static AudioClip MicrophoneToAudioClip(int index = 0)
    {
        if(Microphone.devices.Length <= 0) return null;
        
        string microphoneName = Microphone.devices[index];
        AudioClip microphoneClip = Microphone.Start(microphoneName, true, 10, AudioSettings.outputSampleRate);
        return microphoneClip;
    }
    public static AudioClip MicrophoneToAudioClipByDeviceName(string deviceName)
    {
        int index = GetIndexFromDeviceName(deviceName);
        Debug.Log("" + index);
        if(index == -1) return null;
        return MicrophoneToAudioClip(index);
    }
    // Halts 
    public static void StopListeningToMicrophone(int index = 0) => Microphone.End(Microphone.devices[index]);
    // Returns the device's index of the specified name if matched
    // Returns -1 if index could not be found
    public static int GetIndexFromDeviceName(string deviceName) => Array.IndexOf(Microphone.devices, deviceName);

    // AudioSource.PlayClipAtPoint but returns a source and can be used to attach to something else
    // _base = parent we want to attach this AudioSource to.
    public static AudioSource PlayClipAt(AudioClip ac, Vector3 pos, float volume = 1.0f, GameObject _base = null, float pitch = 1.0f)
    {
        GameObject go = new GameObject("PlayClipAt() Audio");
        go.transform.position = pos;
        if(_base != null) go.transform.parent = _base.transform;

        AudioSource _as = go.AddComponent<AudioSource>();    
        _as.clip = ac;
        _as.volume = volume;
        _as.pitch = pitch;
        _as.Play();
        GameObject.Destroy(go, ac.length + 0.25f); // fixes clip cutting off 
        return _as;
    }

    //######//
    // Math //
    //######//

    ///<summary>Calculates distance in 2D without height (Y value).</summary>
    ///<returns>Distance between two vectors.</returns>
    public static float Vector3_Distance2D(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(Vector3.Scale(a, new Vector3(1,0,1)), Vector3.Scale(b, new Vector3(1, 0, 1)));
    }

    //#########//
    // Finders //
    //#########//

    ///<summary>Literally UnityEngine.Object.FindObjectsOfType() but cooler 😎.</summary>
    public static T[] FindAllInScene<T>(bool includeInactive = false) where T : UnityEngine.Object
    {
        return UnityEngine.Object.FindObjectsOfType<T>(includeInactive);
    }

    //#######//
    // Scene //
    //#######//
    
    public static bool IsInMainMenu() => SceneManager.GetActiveScene().name == MENU_SCENE;

    // Loads the main menu
    public static void LoadMenu() => SceneManager.LoadScene(MENU_SCENE);

    public static void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    //######//
    // Path //
    //######//
    /// <summary>Returns the path from Unity's "Asset" folder till where this was called. folderPath returns without the file this was called in the path.</summary>
    public static string GetPathTillAssetFolder(this UnityEngine.Object o, bool folderPath = false, [CallerFilePath]string fileName = null)
    {
        string fullPath = folderPath ? System.IO.Path.GetDirectoryName(fileName) : fileName;
        int beginIndex = fullPath.IndexOf("Assets");
        string path = beginIndex >= 0 ? fullPath.Substring(beginIndex) : "";
        return path;
    }
}