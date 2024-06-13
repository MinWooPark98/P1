using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowEnemyPattern : EditorWindow
{
    private List<EnemyPatternState> states = new List<EnemyPatternState>();
    private EnemyPatternState selectedState = null;
    private EnemyPatternState transitionStartState = null;
    private Vector2 mousePosition;

    [MenuItem("Window/Enemy Pattern Editor")]
    public static void ShowWindow()
    {
        GetWindow<WindowEnemyPattern>("Enemy Pattern Editor");
    }

    private void OnGUI()
    {
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
                foreach (var state in states)
                {
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
                            transitionStartState.transitions.Add(new Transition { targetState = stateClicked });
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
                else
                {
                    transitionStartState = null;
                    SetSelectedState(null);
                }
            }

            // 우클릭일 때
            if (e.button == 1)
            {
                // 트랜지션 연결 중이 아니라면
                if (transitionStartState == null)
                {
                    foreach (var state in states)
                    {
                        if (state.rect.Contains(mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add Transition"), false, StartTransition, state);
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

        foreach (var state in states)
        {
            DrawState(state);
        }

        if (transitionStartState != null)
        {
            Handles.DrawLine(transitionStartState.rect.center, mousePosition);
            Repaint();
        }

        BeginWindows();
        for (int i = 0; i < states.Count; i++)
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
        if (_state == null)
        {
            GUI.FocusControl(null);
        }
        selectedState = _state;
    }

    private void RemoveSelectedState()
    {
        states.Remove(selectedState);
        for (int i = 0; i < states.Count; i++)
        {
            for (int j = 0; j < states[i].transitions.Count; j++)
            {
                if (states[i].transitions[j].targetState == selectedState)
                {
                    states[i].transitions.RemoveAt(j);
                    j--;
                }
            }
        }
        selectedState = null;
    }

    private void AddNormalPatternState(object _mousePositionObject)
    {
        Vector2 mousePosition = (Vector2)_mousePositionObject;
        EnemyPatternState newState = new EnemyNormalPatternState { rect = new Rect(mousePosition.x, mousePosition.y, 220, 80) };
        states.Add(newState);
    }
    private void AddRandomPatternState(object _mousePositionObject)
    {
        Vector2 mousePosition = (Vector2)_mousePositionObject;
        EnemyPatternState newState = new EnemyRandomPatternState { rect = new Rect(mousePosition.x, mousePosition.y, 400, 0) };
        states.Add(newState);
    }

    private void StartTransition(object _stateObject)
    {
        EnemyPatternState state = (EnemyPatternState)_stateObject;
        if (state.selfTransition)
        {
            return;
        }
        transitionStartState = state;
    }

    private void DrawState(EnemyPatternState _state)
    {
        GUI.Box(_state.rect, _state.GetType().ToString());
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
        EnemyPatternState state = states[_index];
        state.rect = GUI.Window(_index, state.rect, id => DrawStateContents(state), state.GetType().ToString());

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

                DrawTransition(startPos, endPos);
            }
        }
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

                if (GUILayout.Button("X"))
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

    private void DrawTransition(Rect _startRect, Rect _endRect)
    {
        Vector3 startPos = new Vector3(_startRect.x + _startRect.width / 2, _startRect.y + _startRect.height / 2, 0);
        Vector3 endPos = new Vector3(_endRect.x + _endRect.width / 2, _endRect.y + _endRect.height / 2, 0);

        DrawTransition(startPos, endPos);
    }

    private void DrawTransition(Vector3 _startPos, Vector3 _endPos)
    {
        Handles.DrawLine(_startPos, _endPos);

        Vector3 direction = (_endPos - _startPos).normalized;
        Vector3 right = new Vector3(-direction.y, direction.x) * 5;
        Vector3 arrowHeadPoint = _endPos - direction * 10;

        Handles.DrawLine(_endPos, arrowHeadPoint + right);
        Handles.DrawLine(_endPos, arrowHeadPoint - right);
    }
}
