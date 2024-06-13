using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowEnemyPattern : EditorWindow
{
    private EnemyInfo target = null;

    private EnemyPatternState selectedState = null;
    private EnemyPatternTransition selectedTransition = null;
    private EnemyPatternState transitionStartState = null;
    private Vector2 mousePosition;

    private static Vector2 MIN_SIZE = new Vector2(1600, 800);

    private string FolderPath
    {
        get => string.Format("Assets/Resources/Scriptables/EnemyPatternState/" + target.name);
    }

    public static void Open(EnemyInfo _target)
    {
        WindowEnemyPattern window = GetWindow<WindowEnemyPattern>("Enemy Pattern State");
        window.target = _target;
        window.minSize = MIN_SIZE;
        window.Show();
    }

    private void OnGUI()
    {
        if (target == null)
        {
            Close();
            return;
        }

        Event e = Event.current;
        mousePosition = e.mousePosition;

        if (e.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add NormalPattern"), false, AddNormalPatternState, mousePosition);
            menu.AddItem(new GUIContent("Add RandomPattern"), false, AddRandomPatternState, mousePosition);

            menu.ShowAsContext();

            e.Use();
        }

        // 마우스 클릭
        if (e.type == EventType.MouseDown)
        {
            // 좌클릭일 때
            if (e.button == 0)
            {
                EnemyPatternState stateClicked = null;
                EnemyPatternTransition transitionClicked = null;
                bool isTransitionSelected = false;
                foreach (var state in target.patternStates)
                {
                    foreach (var transition in state.transitions)
                    {
                        if (HandleUtility.DistancePointLine(e.mousePosition, transition.startPos, transition.endPos) < 5f)
                        {
                            isTransitionSelected = true;
                            transitionClicked = transition;
                            break;
                        }
                    }

                    if (isTransitionSelected)
                    {
                        break;
                    }

                    if (state.rect.Contains(mousePosition))
                    {
                        stateClicked = state;
                        break;
                    }
                }

                // 박스가 클릭되었을 때
                if (stateClicked != null)
                {
                    // 현재 트랜지션 연결 중이면
                    if (transitionStartState != null)
                    {
                        // 이미 연결되어있지 않아야 연결
                        if (transitionStartState.transitions.Exists((transition) => transition.targetState == stateClicked) == false)
                        {
                            AddTransition(transitionStartState, stateClicked);
                        }
                        else
                        {
                            // 창 띄워서 이미 연결되어있다고 알려주는 것도 괜찮을 듯
                        }
                        transitionStartState = null;
                        SetSelectedState(null);
                        e.Use();
                    }
                    else
                    {
                        SetSelectedState(stateClicked);
                    }
                }
                else if (transitionClicked != null)
                {
                    SetSelectedTransition(transitionClicked);
                }
                else
                {
                    transitionStartState = null;
                    SetSelectedState(null);
                    //SetSelectedTransition(null);
                }
            }

            // 우클릭일 때
            if (e.button == 1)
            {
                // 트랜지션 연결 중이 아니라면
                if (transitionStartState == null)
                {
                    foreach (var state in target.patternStates)
                    {
                        if (state.rect.Contains(mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add Transition"), false, StartTransition, state);
                            menu.AddItem(new GUIContent("Set This State to First"), false, SetFirstState, state);
                            menu.ShowAsContext();

                            e.Use();
                            break;
                        }
                    }
                }
            }    
        }

        if (e.isKey)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                RemoveSelectedState();
                e.Use();
            }
        }

        foreach (var state in target.patternStates)
        {
            DrawState(state);
        }

        if (transitionStartState != null)
        {
            Handles.DrawLine(transitionStartState.rect.center, mousePosition);
            Repaint();
        }

        BeginWindows();
        for (int i = 0; i < target.patternStates.Count; i++)
        {
            DrawStateWindow(i);
        }
        EndWindows();

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void SetSelectedState(EnemyPatternState _state)
    {
        if (_state != null)
        {
            SetSelectedTransition(null);
        }
        GUI.FocusControl(null);
        selectedState = _state;
    }

    private void SetSelectedTransition(EnemyPatternTransition _transition)
    {
        if (_transition != null)
        {
            SetSelectedState(null);
        }
        GUI.FocusControl(null);
        selectedTransition = _transition;
    }

    private void AddNormalPatternState(object _mousePositionObject)
    {
        Vector2 mousePosition = (Vector2)_mousePositionObject;
        EnemyPatternState newState = ScriptableObject.CreateInstance<EnemyNormalPatternState>();
        newState.rect = new Rect(mousePosition.x, mousePosition.y, 220, 80);
        UtilsEditor.CreateFolderIfNotExists(FolderPath);
        UtilsEditor.CreateAsset(newState, string.Format(FolderPath + "newState.asset"));
        target.patternStates.Add(newState);
    }
    private void AddRandomPatternState(object _mousePositionObject)
    {
        Vector2 mousePosition = (Vector2)_mousePositionObject;
        EnemyPatternState newState = ScriptableObject.CreateInstance<EnemyRandomPatternState>();
        newState.rect = new Rect(mousePosition.x, mousePosition.y, 400, 0);
        UtilsEditor.CreateFolderIfNotExists(FolderPath);
        UtilsEditor.CreateAsset(newState, string.Format(FolderPath + "newState.asset"));
        target.patternStates.Add(newState);
    }

    private void AddTransition(EnemyPatternState _startState, EnemyPatternState _targetState)
    {
        EnemyPatternTransition newTransition = ScriptableObject.CreateInstance<EnemyPatternTransition>();
        newTransition.targetState = _targetState;
        AssetDatabase.AddObjectToAsset(newTransition, _startState);
        AssetDatabase.SaveAssets();
        _startState.transitions.Add(newTransition);
    }
    private void RemoveSelectedState()
    {
        target.patternStates.Remove(selectedState);
        for (int i = 0; i < target.patternStates.Count; i++)
        {
            for (int j = 0; j < target.patternStates[i].transitions.Count; j++)
            {
                if (target.patternStates[i].transitions[j].targetState == selectedState)
                {
                    target.patternStates[i].transitions.RemoveAt(j);
                    AssetDatabase.RemoveObjectFromAsset(target.patternStates[i].transitions[j]);
                    j--;
                }
            }
        }
        AssetDatabase.DeleteAsset(string.Format(FolderPath + selectedState.name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        selectedState = null;
    }

    private void SetFirstState(object _stateObject)
    {
        EnemyPatternState state = (EnemyPatternState)_stateObject;
        target.patternStates.Remove(state);
        target.patternStates.Insert(0, state);
    }

    private void StartTransition(object _stateObject)
    {
        EnemyPatternState state = (EnemyPatternState)_stateObject;
        transitionStartState = state;
    }

    private void DrawState(EnemyPatternState _state)
    {
        GUI.Box(_state.rect, _state.name.ToString());
        if (Event.current.type == EventType.MouseDown && _state.rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0)
            {
                SetSelectedState(_state);
            }
        }
    }

    private void DrawStateWindow(int _index)
    {
        EnemyPatternState state = target.patternStates[_index];
        if (_index == 0)
        {
            GUI.color = Color.red;
        }
        state.rect = GUI.Window(_index, state.rect, id => DrawStateContents(state), state.name.ToString());
        GUI.color = Color.white;

        foreach (var transition in state.transitions)
        {
            if (transition.targetState != null)
            {
                float a = 0.5f * (state.rect.width < state.rect.height ? state.rect.width : state.rect.height);
                float b = 0.5f * (transition.targetState.rect.width < transition.targetState.rect.height ? transition.targetState.rect.width : transition.targetState.rect.height);
                // 윈도우 중점 좌상단이라 y는 아래로 갈 수록 커짐
                float angle = Mathf.Rad2Deg * Mathf.Atan2(state.rect.center.y - transition.targetState.rect.center.y, transition.targetState.rect.center.x - state.rect.center.x);
                float startAngleRad = Mathf.Deg2Rad * (angle + 45f);
                float endAngleRad = Mathf.Deg2Rad * (180f + angle - 45f);
                Vector2 startPos = state.rect.center + a * new Vector2(Mathf.Cos(startAngleRad), -Mathf.Sin(startAngleRad));
                Vector2 endPos = transition.targetState.rect.center + b * new Vector2(Mathf.Cos(endAngleRad), -Mathf.Sin(endAngleRad));
                // Rect 밖에서 시작하고 끝나도록
                // Rect 경계 계산
                startPos = GetNearestPosOnBorder(startPos, angle, state.rect);
                endPos = GetNearestPosOnBorder(endPos, 180f + angle, transition.targetState.rect);

                transition.startPos = startPos;
                transition.endPos = endPos;

                DrawArrow(startPos, endPos, transition == selectedTransition ? Color.yellow : Color.white);

                if (transition == selectedTransition)
                {
                    Vector2 size = new Vector2(220f, 100f);
                    Vector2 bottomPos = 0.5f * (startPos + endPos) - size * 0.5f - new Vector2(0f, 10f);
                    Rect rect = new Rect();
                    rect.center = new Vector2(bottomPos.x, bottomPos.y - size.y * 0.5f);
                    rect.size = size;
                    DrawTransitionInfo(transition, rect);
                }
            }
        }
    }

    // rect위치와 크기에 Transition 정보 보여줌
    private void DrawTransitionInfo(EnemyPatternTransition _transition, Rect _rect)
    {
        GUI.Box(_rect, "Transition Info");
        _rect = GUI.Window(target.patternStates.Count + 1, _rect, id => DrawTransitionContents(_transition), "Transition Info");
    }

    private void DrawTransitionContents(EnemyPatternTransition _transition)
    {
        GUILayout.BeginVertical();

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;

        EditorGUILayout.BeginHorizontal();
        _transition.condition.condition = (ENEMY_PATTERN_CONDITION)EditorGUILayout.EnumPopup("Condition", _transition.condition.condition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _transition.condition.value = EditorGUILayout.FloatField("Value", _transition.condition.value);
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originalLabelWidth;

        GUILayout.EndVertical();
    }

    private Vector2 GetNearestPosOnBorder(Vector2 _pos, float _angle, Rect _border)
    {
        float angleRad = _angle * Mathf.Deg2Rad;
        float stateBorderX = Mathf.Cos(angleRad) >= 0f ? _border.center.x + _border.width * 0.5f : _border.center.x - _border.width * 0.5f;
        float stateBorderY = Mathf.Sin(angleRad) >= 0f ? _border.center.y - _border.height * 0.5f : _border.center.y + _border.height * 0.5f;
        float stateAlpha = (stateBorderX - _pos.x) / Mathf.Cos(angleRad);
        float stateBeta = (stateBorderY - _pos.y) / -Mathf.Sin(angleRad);
        float stateDistance = stateAlpha < stateBeta ? stateAlpha : stateBeta;
        Vector2 returnPos = _pos + stateDistance * new Vector2(Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
        return returnPos;
    }

    private void DrawStateContents(EnemyPatternState _state)
    {
        GUILayout.BeginVertical();

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;
        if (_state.GetType() == typeof(EnemyNormalPatternState))
        {
            EditorGUILayout.BeginHorizontal();

            EnemyNormalPatternState state = (EnemyNormalPatternState) _state;
            state.pattern = EditorGUILayout.ObjectField("Pattern", state.pattern, typeof(EnemyPattern), false) as EnemyPattern;

            EditorGUILayout.EndHorizontal();
        }
        else if (_state.GetType() == typeof(EnemyRandomPatternState))
        {
            float totalHeight = 0f;
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EnemyRandomPatternState state = (EnemyRandomPatternState) _state;
            if (state.randomPatterns == null)
            {
                state.randomPatterns = new List<EnemyRandomPatternState.RandomPattern>();
            }

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < state.randomPatterns.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                state.randomPatterns[i].pattern = EditorGUILayout.ObjectField("Pattern", state.randomPatterns[i].pattern, typeof(EnemyPattern), false) as EnemyPattern;
                state.randomPatterns[i].weight = EditorGUILayout.FloatField("Weight", state.randomPatterns[i].weight);

                if (GUILayout.Button("Delete", new GUILayoutOption[] { GUILayout.Width(50f) }))
                {
                    state.randomPatterns.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();

                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Add New"))
            {
                state.randomPatterns.Add(new EnemyRandomPatternState.RandomPattern());
            }
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            totalHeight += 10f;
            state.rect.height = totalHeight;
        }
        EditorGUIUtility.labelWidth = originalLabelWidth;

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private void DrawArrow(Vector3 _startPos, Vector3 _endPos, Color _color)
    {
        Handles.color = _color;

        Handles.DrawLine(_startPos, _endPos);

        Vector3 direction = (_endPos - _startPos).normalized;
        Vector3 right = new Vector3(-direction.y, direction.x * 8f);
        Vector3 arrowHeadPoint = _endPos - direction * 16f;

        Handles.DrawLine(_endPos, arrowHeadPoint + right);
        Handles.DrawLine(_endPos, arrowHeadPoint - right);

        Handles.color = Color.white;
    }
}
