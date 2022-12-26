using System.Collections;
using System.Collections.Generic;
using static System.Type;
using UnityEngine;
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
    public static string GetIcon(string name) { return PATH_ICONS + name; }
    public static string GetTexture(string name) { return PATH_TEXTURES + name; }
    public static string GetMaterial(string name) { return PATH_MATERIALS + name; }
    public static string GetShader(string name) { return PATH_SHADERS + name; }
    public static string GetParticle(string name) { return PATH_PARTICLES + name; }
    public static string GeObject(string name) { return PATH_OBJECTS + name; }
    // Prints
    public static void Print<T>(this T _class, string msg) => Debug.Log("[" + _class.ToString().ToUpper() +"] | " + msg);
    public static void Print<T>(string msg) where T : Object => Debug.Log("[" + typeof(T).ToString().ToUpper() +"] | " + msg);
    // Components
    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
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

    public static List<Transform> GetChildren(this Transform parent, List<Transform> transformList = null, bool getChildrenOfChild = false)
    {
        if (transformList == null) transformList = new List<Transform>();
        
        foreach (Transform child in parent) {
            transformList.Add(child);
            if(getChildrenOfChild) GetChildren(child, transformList, getChildrenOfChild);
        }

        return transformList;
    }
}