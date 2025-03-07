using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting.YamlDotNet.Core.Tokens;

using UnityEditor;

using UnityEngine;

using UnityFramework.Addressable.Editor;

namespace AddressableEditor
{
    public class AddressableManagingEditor : EditorWindow
    {
        enum MenuType
        {
            Unsafes,
            Safes,
            Options
        }

        enum IndicatorColor
        {
            gray = 0,
            Green = 1,
            Red = 2,
        }


        private static bool isTracking = true;
        private static float splitRatio = 0.15f;
        private static GUIStyle menuStyle;
        private static GUIStyleState styleState;
        private static GUIStyleState styleStateBalckColor;
        private static GUIStyle foldoutStyle;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private int selectedIndex = 0;
        private float splitterWidth = 4.0f;
        private bool isResizing = false;
        private readonly string[] menuItems = { "Unsafes", "Safes", "Options" };
        private readonly string[] IndicatorColors =
        {
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.gray)}>●</color>",
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.green)}>●</color>",
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>●</color>" ,
        };

        [MenuItem("Addressable/Managing")]

        public static void ShowWindow()
        {
            GetWindow<AddressableManagingEditor>().Show();

            splitRatio = EditorPrefs.GetFloat("_splitRatio", 0.15f);
            isTracking = EditorPrefs.GetBool("_isTracking", true);

        }

        private void OnEnable()
        {
            InitiationStyle();
        }

        private void OnGUI()
        {
            if (!CheckStyles())
                InitiationStyle();

            Rect windowRect = position;
            float splitWidth = windowRect.width * splitRatio;

            GUILayout.BeginHorizontal();

            // 왼쪽 패널 (목차 / 네비게이션)
            GUILayout.BeginVertical(GUILayout.Width(splitWidth));
            leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, GUILayout.ExpandHeight(true));

            //GUILayout.Label("목차", EditorStyles.boldLabel);
            GUILayout.Space(10.0f);
            for (int i = 0; i < menuItems.Length; i++)
            {
                //EditorGUILayout.HelpBox("d", MessageType.Info);
                if (selectedIndex == i)
                    menuStyle.normal = styleStateBalckColor;
                else
                    menuStyle.normal = styleState;
                if (GUILayout.Button(menuItems[i], menuStyle))
                {
                    selectedIndex = i;
                }
                GUILayout.Space(3.0f);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            DrawLine(splitWidth, windowRect);

            GUILayout.Space(splitterWidth); // 선 너비 추가

            // 오른쪽 패널 (선택한 항목의 상세 정보)
            GUILayout.BeginVertical(GUI.skin.box);
            rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, GUILayout.ExpandHeight(true));

            if (selectedIndex == (int)MenuType.Options)
            {
                DrawOptions();
            }
            else
            {
                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("This editor only works during play mode.", MessageType.Info);
                }
                if (selectedIndex == (int)MenuType.Unsafes)
                {
                    DrawResourceData(AddressableManagingDataManager.LoadType.UnsafeLoad);
                }
                else if (selectedIndex == (int)MenuType.Safes)
                {
                    DrawResourceData(AddressableManagingDataManager.LoadType.SafeLoad);
                }
            }




            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawLine(float splitWidth, Rect windowRect)
        {
            Rect splitterRect = new Rect(splitWidth, 0, splitterWidth, windowRect.height);
            EditorGUI.DrawRect(splitterRect, new Color(0.4f, 0.4f, 0.4f, 1f)); // 회색 선 표시
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && splitterRect.Contains(Event.current.mousePosition))
            {
                isResizing = true;
            }
            if (isResizing && Event.current.type == EventType.MouseDrag)
            {
                splitRatio = Mathf.Clamp(Event.current.mousePosition.x / windowRect.width, 0.1f, 0.9f);
                Repaint();
            }
            if (Event.current.type == EventType.MouseUp)
            {
                isResizing = false;
                EditorPrefs.SetFloat("_splitRatio", splitRatio);
            }
        }

        private void InitiationStyle()
        {
            menuStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter,
            };

            styleState = new GUIStyleState()
            {
                textColor = Color.white,
            };

            styleStateBalckColor = new GUIStyleState()
            {
                textColor = Color.black,
            };

            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                richText = true,
            };
        }

        private bool CheckStyles()
        {
            return (menuStyle != null && styleState != null && styleStateBalckColor != null && foldoutStyle != null);
        }

        private void DrawOptions()
        {
            bool isbool = isTracking;
            isbool = EditorGUILayout.Toggle("Addressable Traking", isbool);
            if (isTracking != isbool)
            {
                isTracking = isbool;
                AddressableManagingDataManager.IsTracking = isTracking;
                EditorPrefs.SetBool("_isTracking", isTracking);
            }
        }

        private void DrawResourceData(AddressableManagingDataManager.LoadType loadType)
        {
            if (AddressableManagingDataManager.addressableManagingDatas.Count < 1)
                return;

            EditorGUI.indentLevel++;
            foreach (var keyValuePair in AddressableManagingDataManager.addressableManagingDatas)
            {
                var data = keyValuePair.Value;
                if (!data.stackTraces.TryGetValue(loadType, out var dataStack))
                    continue;


                string IndicatorColor = IndicatorColors[(int)GetIndicatorColor(data)];

                EditorGUILayout.BeginHorizontal();
                data.foldout = EditorGUILayout.Foldout(data.foldout, $"{IndicatorColor} Name : {data.name}  LoadCount : {data.loadCount} ", foldoutStyle);
                if (data.foldout)
                {

                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel++;

        }

        private IndicatorColor GetIndicatorColor(AddressableManagingData addressableManagingData)
        {
            if (addressableManagingData.loadCount < 1)
                return IndicatorColor.gray;

            int keyCount = 0;
            if (addressableManagingData.stackTraces.ContainsKey(AddressableManagingDataManager.LoadType.UnsafeLoad))
                keyCount++;

            if (addressableManagingData.stackTraces.ContainsKey(AddressableManagingDataManager.LoadType.SafeLoad))
                keyCount++;


            return (IndicatorColor)keyCount;
        }
    }

}