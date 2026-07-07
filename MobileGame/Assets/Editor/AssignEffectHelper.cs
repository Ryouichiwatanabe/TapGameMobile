#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssignEffectHelper
{
    [InitializeOnLoadMethod]
    static void AssignEffect()
    {
        string circlePath = "Assets/Resources/Circle.prefab";
        string effectPath = "Assets/AssestStore/CartoonVFX9x/Comic_FX/Prefabs/Battle_Effect_White.prefab";
        
        GameObject circle = AssetDatabase.LoadAssetAtPath<GameObject>(circlePath);
        GameObject effect = AssetDatabase.LoadAssetAtPath<GameObject>(effectPath);
        
        if (circle != null && effect != null)
        {
            var controller = circle.GetComponent<CircleController>();
            if (controller != null)
            {
                SerializedObject so = new SerializedObject(controller);
                SerializedProperty prop = so.FindProperty("clickEffectPrefab");
                if (prop != null && prop.objectReferenceValue != effect)
                {
                    prop.objectReferenceValue = effect;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(circle);
                    AssetDatabase.SaveAssets();
                    Debug.Log("[AssignEffectHelper] Successfully assigned Battle_Effect_White to CirclePrefab!");
                }
            }
        }
    }
}
#endif
