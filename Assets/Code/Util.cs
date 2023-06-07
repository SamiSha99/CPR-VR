using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public const int MICROPHONE_LOUDNESS_MULTIPLIER = 100;
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
        Error
    };
    
    // Print function with "typeof" being passed, helps with debug reasons in which a specified class name will pin point where this was called
    public static void Print<T>(this T _class,  string msg) => Debug.Log("[" + _class.ToString().ToUpper() +"] | " + msg);
    public static void Print(string msg, [CallerMemberName] string _class = null, PrintType _type = PrintType.Normal)
    {
        msg = "[" + _class.ToUpper() +"] | " + msg;
        switch(_type)
        {
            case PrintType.Normal: Debug.Log(msg);          break;
            case PrintType.Warn:   Debug.LogWarning(msg);   break;
            case PrintType.Error:  Debug.LogError(msg);     break;
        }
    }
    public static void Print<T>(string msg) where T : UnityEngine.Object => Debug.Log("[" + typeof(T).ToString().ToUpper() +"] | " + msg);
    // Print with tagging
    //public static void Print(string tag, string msg) => Debug.Log("[" + tag + "] " + msg);

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
            Debug.LogWarning("Could not find the game object in FindComponent returned null");
            return null;
        }
        T comp = o.GetComponent<T>();
        if(comp == null)
        {
            Debug.LogWarning("GameObject lacks the component " + typeof(T) + " returned null");
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
    public static GameObject GetPlayerCameraObject(this GameObject o) => o.GetRoot().transform.Find("Camera Offset/XR Camera").gameObject;
    // Returns the Left Hand of this Owned player object (works with root too)
    // param right - Returns the Right Hand instead
    public static GameObject GetPlayerHandObject(this GameObject o, bool right = false) => o.GetRoot().transform.Find("Camera Offset/" + (right ? "Right" : "Left") + " Hand").gameObject;
    public static GameObject GetPlayer() => GameObject.FindWithTag("Player");
    public static XREvents GetXREvents(GameObject plyr = null)
    {
        if(plyr == null) plyr = GetPlayer(); // Didn't pass the player? Try looking for one.
        if(plyr == null)
        {
            Print("Utility Class", "Cannot get XREvents, player does not exists?");
            return null;
        }
        return plyr.GetComponent<XREvents>();
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
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
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
    public static AudioSource PlayClipAt(AudioClip ac, Vector3 pos, float volume = 1.0f, GameObject _base = null)
    {
        GameObject go = new GameObject("PlayClipAt() Audio");
        go.transform.position = pos;
        if(_base != null) go.transform.parent = _base.transform;

        AudioSource _as = go.AddComponent<AudioSource>();    
        _as.clip = ac;
        _as.volume = volume;
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
}