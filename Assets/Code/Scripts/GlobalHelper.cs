using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// Utility Class
public static class GlobalHelper
{
    const string PATH_ART = "Assets/Art/";
    const string PATH_ICONS = "Assets/Art/Icons/";
    const string PATH_TEXTURES = "Assets/Art/Textures/";
    const string PATH_MATERIALS = "Assets/Art/Materials/";
    const string PATH_SHADERS = "Assets/Art/Shaders/";
    const string PATH_PARTICLES = "Assets/Art/Particles/";
    const string PATH_OBJECTS = "Assets/Art/Objects/";
    // Path way search to specific items, just look up the name of the file + extension
    public static string GetIcon(string name) { return PATH_ICONS + name; }
    public static string GetTexture(string name) { return PATH_TEXTURES + name; }
    public static string GetMaterial(string name) { return PATH_MATERIALS + name; }
    public static string GetShader(string name) { return PATH_SHADERS + name; }
    public static string GetParticle(string name) { return PATH_PARTICLES + name; }
    public static string GeObject(string name) { return PATH_OBJECTS + name; }
    // Prints
    public static void Print<T>(this T _class, string msg) => Debug.Log("[" + _class.ToString().ToUpper() +"] | " + msg);
    public static void Print<T>(string msg) where T : UnityEngine.Object => Debug.Log("[" + typeof(T).ToString().ToUpper() +"] | " + msg);
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
    // Hides the component from being visible, the component is still active, its just their components are completely invisible and none interactive.
    // Supports Colliders, Mesh Renders and Rigidbody, will add eventually more when needed.
    // Cons: Will un/hide everything willingly and doesn't not consider or respect pre-hidden or meant to be hidden content
    // The opposite is also true.
    public static void ToggleHidden(this Transform transform, bool _hidden = true, bool disableCollision = true, bool disableRBs = true)
    {
        GameObject go = transform.gameObject;
        if(go.HasComponent<Renderer>()) Array.ForEach(transform.GetComponents<Renderer>(), x => x.enabled = !_hidden);
        if(go.HasComponent<Collider>()) Array.ForEach(transform.GetComponents<Collider>(), x => x.enabled = !_hidden);
        if(go.HasComponent<Rigidbody>()) Array.ForEach(transform.GetComponents<Rigidbody>(), x => x.isKinematic = _hidden);

        List<Transform> children = null;
        children = GlobalHelper.GetChildren(transform, children, true);
        
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
        
        foreach (Transform child in parent) {
            transformList.Add(child);
            if(getChildrenOfChild) GetChildren(child, transformList, getChildrenOfChild);
        }

        return transformList;
    }
    // Invoke that allows parameter passing via lambda expression (anonymous function)
    public static void Invoke(this MonoBehaviour mb, Action f, float delay) => mb.StartCoroutine(InvokeRoutine(f, delay));    
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
    // THIS IS ALL WRONG! FIX!!!
    public const int LAYER_INTERACTABLE = 3;
    public const int LAYER_DIRECT_INTERACTABLE = 4;
    public const int LAYER_RAY_INTERACTABLE = 5;
    public static void ChangeInteractionLayerMask(this XRBaseInteractable xrBase, int[] add = null, int[] remove = null)
    {
        Print<UnityEngine.Object>(xrBase.interactionLayers.value.ToString());
        foreach(int i in add) xrBase.interactionLayers += i;
        foreach(int i in remove) xrBase.interactionLayers -= i;
    }
    // WIP???
    public static int[] GetLayerMasks(string[] layers)
    {
        int[] ints = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++) ints[i] = InteractionLayerMask.GetMask(layers[i]);
        return ints.Length == 0 ? null : ints;
    }
}