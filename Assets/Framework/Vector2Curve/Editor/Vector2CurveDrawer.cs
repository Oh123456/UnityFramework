using System.Buffers;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using UnityFramework;


[CustomPropertyDrawer(typeof(Vector2Curve), true)]
public class Vector2CurveDrawer : PropertyDrawer
{
    private const int BUTTON_SIZE = 100;
    private const int GRAPH_SIZE = 300; // 그래프 크기 (px)
    private const int GRAPH_SIZE_X = 500; // 그래프 크기 (px)
    private const string MOVEC_URVES = "moveCurves";
    private const string ADD_POINT = "Add Point";    
    private const string CURVE_MODE = "curveMode";    
    private GUIContent addPointGUIContent = new GUIContent(ADD_POINT);

    private const float xValue = 20.0f;
    private const float yValue = 30.0f;
    private int selectedPointIndex = -1; // 선택된 점의 인덱스
    private Vector2 lastMousePos;
    private List<Vector2> smoothPoints = new List<Vector2>();

    private bool showRealCure = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GUIContent gUIContent = new GUIContent();

        // 라벨과 UI 그리기
        Rect foldoutRect = new Rect(position.x + 10.0f, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            // color 필드
            Rect graphRect = new Rect(position.x + xValue, position.y + yValue, GRAPH_SIZE, GRAPH_SIZE);
            GUI.Box(graphRect, ""); // 그래프 배경
            GUI.Label(new Rect(position.x + xValue + 10.0f + GRAPH_SIZE, position.y + GRAPH_SIZE - 30.0f, GRAPH_SIZE, BUTTON_SIZE), $"SelectedPointIndex : {selectedPointIndex}");
            SerializedProperty curveProp = property.FindPropertyRelative(MOVEC_URVES);
            Rect posRect = new Rect(position.x, position.y + yValue + 10.0f, position.width, EditorGUIUtility.singleLineHeight);
            DrawGraph(graphRect, curveProp, property); // 그래프 그리기
            EditorGUI.PropertyField(new Rect(position.x, position.y + yValue + 20.0f + GRAPH_SIZE, GetPropertyHeight(property, null), GRAPH_SIZE), curveProp, new GUIContent(MOVEC_URVES));

            SerializedProperty modeProp = property.FindPropertyRelative(CURVE_MODE);
            EditorGUILayout.PropertyField(modeProp);

            //modeProp.enu
            ChildPropertys(position, property, label);
            EditorGUI.indentLevel--;
        }



        EditorGUI.EndProperty();
    }


    private void DrawGraph(Rect rect, SerializedProperty curveProp, SerializedProperty target)
    {
        if (curveProp.arraySize < 2) return; // 최소한 두 개의 점이 있어야 선을 그림

        Handles.color = Color.black; // 선 색상 설정
        List<Vector2> points = new List<Vector2>();

        Event e = Event.current;
        lastMousePos = e.mousePosition;

        float left = rect.x;
        float right = rect.x + rect.width;
        float top = rect.y;
        float bottom = rect.y + rect.height;
        Vector2 zeroPoint = new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);

        Handles.DrawLine(new Vector2(left, zeroPoint.y), new Vector2(right, zeroPoint.y));
        Handles.DrawLine(new Vector2(zeroPoint.x, top), new Vector2(zeroPoint.x, bottom));

        for (int i = 0; i < curveProp.arraySize; i++)
        {
            SerializedProperty pointProp = curveProp.GetArrayElementAtIndex(i);
            Vector2 point = pointProp.vector2Value;


            // 좌표계를 0~1 범위로 정규화하여 그래프에 맞게 변환
            Vector2 graphPoint = new Vector2(
                Mathf.Lerp(rect.x, rect.x + rect.width, (point.x + 1) * 0.5f),
                Mathf.Lerp(rect.y + rect.height, rect.y, (point.y + 1) * 0.5f)
            );

            points.Add(graphPoint);

            // 점을 선택 (마우스 클릭 감지)
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (Vector2.Distance(lastMousePos, graphPoint) < 10f) // 클릭 감지 범위
                {
                    selectedPointIndex = i;
                    e.Use();
                }

            }

            if (selectedPointIndex == i && e.type == EventType.MouseDrag)
            {
                Vector2 newPoint = new Vector2(
                    Mathf.InverseLerp(rect.x, rect.x + rect.width, lastMousePos.x) * 2 - 1,
                    Mathf.InverseLerp(rect.y + rect.height, rect.y, lastMousePos.y) * 2 - 1
                );

                pointProp.vector2Value = newPoint;
                e.Use();
            }

            if (selectedPointIndex == i)
                Handles.color = Color.red;
            else if (i == 0)
                Handles.color = Color.blue;
            else if (i == curveProp.arraySize - 1)
                Handles.color = Color.white;
            else
                Handles.color = Color.gray; // 선 색상 설정

            if (Handles.Button(graphPoint, Quaternion.identity, 5, 10, Handles.CircleHandleCap))
            {
                selectedPointIndex = i;
            }


            if (rect.Contains(e.mousePosition) && e.type == EventType.ContextClick) // 우클릭 감지
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(addPointGUIContent, false, () => AddPoint(curveProp, rect));
                menu.ShowAsContext();
                e.Use();
            }

        }

        Handles.color = Color.green;
        // 점들을 선으로 연결

        var list = GenerateCatmullRomSpline(points);

        for (int i = 0; i < list.Count - 1; i++)
        {
            //Handles.DrawAAPolyLine(points[i], points[i + 1]);
            Handles.DrawLine(list[i], list[i + 1]);
        }

        Handles.color = Color.red;
        if (this.showRealCure)
        {

            Vector2[] vector2s = new Vector2[curveProp.arraySize];

            for (int i = 0; i < curveProp.arraySize; i++)
            {
                Vector2 v = curveProp.GetArrayElementAtIndex(i).vector2Value;
                vector2s[i] = new Vector2(
                                          Mathf.Lerp(rect.x, rect.x + rect.width, (v.x + 1) * 0.5f),
                                          Mathf.Lerp(rect.y + rect.height, rect.y, (v.y + 1) * 0.5f));
            }


            Vector2 point = Vector2Curve.EvaluateCatmullRom(0, vector2s);
            for (int i = 1; i < 100; i++)
            {
                Vector2 current = Vector2Curve.EvaluateCatmullRom(i * 0.01f, vector2s);
                Handles.DrawLine(point, current);
                point = current;
            }

        }

    }

    protected virtual void ChildPropertys(Rect position, SerializedProperty property, GUIContent label)
    {
        
    }

    void AddPoint(SerializedProperty property, Rect graphRect)
    {
        Undo.RecordObject(property.serializedObject.targetObject, ADD_POINT);

        // 마우스 클릭 위치를 그래프 내부 좌표로 변환
        Vector2 graphMousePos = new Vector2(
            Mathf.InverseLerp(graphRect.x, graphRect.x + GRAPH_SIZE, lastMousePos.x) * 2 - 1,
            Mathf.InverseLerp(graphRect.y + GRAPH_SIZE, graphRect.y, lastMousePos.y) * 2 - 1
        );

        // 가장 가까운 두 점 찾기
        int insertIndex = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < property.arraySize - 1; i++)
        {
            SerializedProperty p1 = property.GetArrayElementAtIndex(i);
            SerializedProperty p2 = property.GetArrayElementAtIndex(i + 1);
            Vector2 v1 = p1.vector2Value;
            Vector2 v2 = p2.vector2Value;

            // 마우스 위치가 두 점을 잇는 선분과 얼마나 가까운지 확인
            Vector2 projectedPoint = ClosestPointOnLineSegment(v1, v2, graphMousePos);
            float dist = Vector2.Distance(graphMousePos, projectedPoint);

            if (dist < closestDist)
            {
                closestDist = dist;
                insertIndex = i + 1; // 현재 위치 다음에 삽입
            }
        }

        // 리스트에 새 점 삽입
        property.InsertArrayElementAtIndex(insertIndex);
        property.GetArrayElementAtIndex(insertIndex).vector2Value = graphMousePos;

        property.serializedObject.ApplyModifiedProperties();

    }

    private List<Vector2> GenerateCatmullRomSpline(List<Vector2> points /*int resolution*/)
    {
        smoothPoints.Clear();

        for (int i = 0; i < points.Count - 1; i++)
        {
            // 시작과 끝에서 제어점 처리
            Vector2 p0 = (i == 0) ? points[i] : points[i - 1];
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];
            Vector2 p3 = (i == points.Count - 2) ? points[i + 1] : points[i + 2];

            // 해당 구간을 세분화하여 부드러운 곡선 생성
            for (int j = 0; j < 100; j++)
            {
                float t = j * 0.01f;// (float)resolution;                
                smoothPoints.Add(Vector2Curve.CatmullRom(p0, p1, p2, p3, t));
            }
        }
        return smoothPoints;
    }

    private Vector2 ClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;
        Vector2 AB = B - A;
        float magnitudeAB = AB.sqrMagnitude;
        float ABAPproduct = Vector2.Dot(AP, AB);
        float distance = ABAPproduct / magnitudeAB;

        if (distance < 0) return A;
        if (distance > 1) return B;

        return A + AB * distance;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // 기본 높이
        if (property.isExpanded)
        {

            height += PropertyHeight(property, MOVEC_URVES);
            height += GRAPH_SIZE + 20; // 그래프 높이 추가

        }
        return height + yValue;
    }

    float PropertyHeight(SerializedProperty property , string propertyName)
    { 
        SerializedProperty pointsProp = property.FindPropertyRelative(MOVEC_URVES);

        return EditorGUI.GetPropertyHeight(pointsProp, true);
    }
}
