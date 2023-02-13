using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Utility Class
public static class Util
{
    //#######//
    // Paths //
    //#######//

    const string PATH_ART = "Assets/Art/";
    const string PATH_ICONS = "Assets/Art/Icons/";
    const string PATH_TEXTURES = "Assets/Art/Textures/";
    const string PATH_MATERIALS = "Assets/Art/Materials/";
    const string PATH_SHADERS = "Assets/Art/Shaders/";
    const string PATH_PARTICLES = "Assets/Art/Particles/";
    const string PATH_OBJECTS = "Assets/Art/Objects/";
    const string PATH_AUDIO_CLIPS = "Assets/Audio";
    // Path way search to specific items, just look up the name of the file + extension
    public static string GetIcon(string name) { return PATH_ICONS + name; }
    public static string GetTexture(string name) { return PATH_TEXTURES + name; }
    public static string GetMaterial(string name) { return PATH_MATERIALS + name; }
    public static string GetShader(string name) { return PATH_SHADERS + name; }
    public static string GetParticle(string name) { return PATH_PARTICLES + name; }
    public static string GeObject(string name) { return PATH_OBJECTS + name; }
    
    //#######//
    // Debug //
    //#######//
    
    // Print function with "typeof" being passed, helps with debug reasons in which a specified class name will pin point where this was called
    public static void Print<T>(this T _class, string msg) => Debug.Log("[" + _class.ToString().ToUpper() +"] | " + msg);
    public static void Print<T>(string msg) where T : UnityEngine.Object => Debug.Log("[" + typeof(T).ToString().ToUpper() +"] | " + msg);
    // Print with tagging
    public static void Print(string tag, string msg) => Debug.Log("[" + tag + "] " + msg);

    //############//
    // Extensions //
    //############//

    // Search for a component and return true if said component exists
    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
    // Find a component and return the component using a path similar to Transform.Find(); return null if couldn't find it.
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
    // Hides the component from being visible, the component is still active, its just their components are completely none interactive.
    // Supports Colliders, Renderers and Rigidbody, will add eventually more when needed.
    // Cons: Will un/hide everything willingly and doesn't consider nor respect pre-hidden or meant to be hidden content
    // The opposite is also true.
    public static void ToggleHidden(this Transform transform, bool _hidden = true, bool disableCollision = true, bool disableRBs = true)
    {
        GameObject go = transform.gameObject;
        if(go.HasComponent<Renderer>()) Array.ForEach(transform.GetComponents<Renderer>(), x => x.enabled = !_hidden);
        if(go.HasComponent<Collider>()) Array.ForEach(transform.GetComponents<Collider>(), x => x.enabled = !_hidden);
        if(go.HasComponent<Rigidbody>()) Array.ForEach(transform.GetComponents<Rigidbody>(), x => x.isKinematic = _hidden);

        List<Transform> children = null;
        children = Util.GetChildren(transform, children, true);
        
        if(children.Count <= 0) return;
        
        foreach(Transform child in children)
        {
            go = child.gameObject;
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
    
    //########//
    // Quests //
    //########//
    
    public static bool IsQuestActive()
    {
        GameObject o = GameObject.Find("QuestCanvas");
        if(o == null) return false;
        QuestManager qm = o.GetComponent<QuestManager>();
        if(qm == null) return false;

        return QuestManager.activeQuest != null;
    }
    public static void ForceCompleteCurrentQuest()
    {
        GameObject o = GameObject.Find("QuestCanvas");
        if(o == null) return;
        QuestManager qm = o.GetComponent<QuestManager>();
        if(qm == null) return;
    }
    
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

    public static bool IntToBool(int i) { return i > 0; }
    public static int BoolToInt(bool b) { return b ? 1 : 0; }
}