using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MassMaterialChange : EditorWindow
{
    [MenuItem("Custom Tools/Materials/Update Shaders")]
    public static void UpdateMaterials()
    {
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        Shader shader = Shader.Find("Toony Colors Pro 2/Examples/Default/Comic Book");
        Debug.Log(materials.Length);
        Debug.Log(shader == null);

        foreach (var i in materials)
            i.shader = shader;
    }
}
