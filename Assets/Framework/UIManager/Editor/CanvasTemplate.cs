using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CanvasTemplate", menuName = "UnityFramework/CreateCanvasTemplate")]
public class CanvasTemplate : ScriptableObject
{
    [SerializeField] CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    [SerializeField] Vector2 resolution = new Vector2(1920.0f, 1080.0f);
    [SerializeField][Range(0.0f, 1.0f)] float match = 0.5f;
    [SerializeField] bool useGraphicRaycaster = true;


    public CanvasScaler.ScaleMode ScaleMode => scaleMode;
    public Vector2 Resolution => resolution;
    public float Match => match;
    public bool UseGraphicRaycaster => useGraphicRaycaster;


    
}


public class CanvasTemplateCreater
{
    [MenuItem("GameObject/UnityFramework/CreateCanvasTemplate")]
    public static void CreaterCanvs()
    {
        string[] guids = AssetDatabase.FindAssets("t:CanvasTemplate");

        if (guids.Length == 0)
        {
            Debug.LogWarning("CanvasTemplate가 없습니다");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        CanvasTemplate canvasTemplate = AssetDatabase.LoadAssetAtPath<CanvasTemplate>(path);


        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = canvasTemplate.ScaleMode;
        canvasScaler.referenceResolution = canvasTemplate.Resolution;
        canvasScaler.matchWidthOrHeight = canvasTemplate.Match;
        if (canvasTemplate.UseGraphicRaycaster)
            canvasObject.AddComponent<GraphicRaycaster>();
    }
}