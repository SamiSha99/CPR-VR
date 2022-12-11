using System.Collections;
using System.Collections.Generic;
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
    const string PATH_OBJECTS = "Assets/Art/Particles/";
    public static string GetIcon(string name) { return PATH_ICONS + name; }
    public static string GetTexture(string name) { return PATH_TEXTURES + name; }
    public static string GetMaterial(string name) { return PATH_MATERIALS + name; }
    public static string GetShader(string name) { return PATH_SHADERS + name; }
    public static string GetParticle(string name) { return PATH_PARTICLES + name; }
    public static string GeObject(string name) { return PATH_OBJECTS + name; }

    // WIP
    public static void Print<T>(this T _class, string msg)
    {
        Debug.Log("[" + _class.ToString().ToUpper() +"] | " + msg);
    }

    public static bool HasComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}