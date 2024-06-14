using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowEnemyPattern : EditorWindow
{
    private class StateBox
    {
        public Rect rect;
        public EnemyPatternState state;
    }

    private class TransitionLine
    {
        public Vector2 startPos;
        public Vector2 endPos;
        public EnemyPatternTransition transition;
        public StateBox masterBox;
        public StateBox targetBox;
    }

    private class TransitionInfoBox
    {
        public Rect rect;
        public EnemyPatternTransition transition;
    }


    private EnemyInfo target = null;

    private List<StateBox> listStateBox = new List<StateBox>();
    private List<TransitionLine> listTransitionLine = new List<TransitionLine>();
    private TransitionInfoBox transitionInfoBox = new TransitionInfoBox();

    private StateBox selectedStateBox = null;
    private TransitionLine selectedTransitionLine = null;
    private StateBox transitionStartStateBox = null;
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
        window.Set();
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

        ProcessEvent(Event.current);

        DrawState();

        bool shouldRepaint = false;

        if (transitionStartStateBox != null)
        {
            Handles.DrawLine(transitionStartStateBox.rect.center, mousePosition);
            shouldRepaint = true;
        }

        CalculateTransitionLinePos();
        DrawTransitionLine();

        BeginWindows();
        for (int i = 0; i < target.patternStates.Count; i++)
        {
            DrawStateWindow(i);
        }

        if (selectedTransitionLine != null)
        {
            DrawTransitionInfo();
        }
        EndWindows();

        if (GUI.changed || shouldRepaint)
        {
            Repaint();
        }
    }

    private void ProcessEvent(Event e)
    {
        mousePosition = e.mousePosition;

        if (e.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add NormalPattern"), false, AddNormalPatternState);
            menu.AddItem(new GUIContent("Add RandomPattern"), false, AddRandomPatternState);

            menu.ShowAsContext();

            e.Use();
        }

        // 마우스 클릭
        if (e.type == EventType.MouseDown)
        {
            // 좌클릭일 때
            if (e.button == 0)
            {
                bool isTransitionInfoBoxClicked = false;
                StateBox stateBoxClicked = null;
                TransitionLine transitionLineClicked = null;

                if (selectedTransitionLine != null && transitionInfoBox.rect.Contains(mousePosition))
                {
                    isTransitionInfoBoxClicked = true;
                }

                if (isTransitionInfoBoxClicked == false)
                {
                    foreach (var box in listStateBox)
                    {
                        if (box.rect.Contains(mousePosition))
                        {
                            stateBoxClicked = box;
                        }
                    }

                    if (stateBoxClicked == null)
                    {
                        foreach (var line in listTransitionLine)
                        {
                            if (HandleUtility.DistancePointLine(e.mousePosition, line.startPos, line.endPos) < 8f)
                            {
                                transitionLineClicked = line;
                            }
                        }
                    }
                }

                if (isTransitionInfoBoxClicked)
                {
                    // 당장은 트랜지션 정보 상자 건드렸을 때 동작하는 거 없음
                }
                // 박스가 클릭되었을 때
                else if (stateBoxClicked != null)
                {
                    // 현재 트랜지션 연결 중이면
                    if (transitionStartStateBox != null)
                    {
                        // 이미 연결되어있지 않아야 연결
                        if (transitionStartStateBox.state.transitions.Exists((transition) => transition.targetState == stateBoxClicked.state) == false)
                        {
                            AddTransition(transitionStartStateBox, stateBoxClicked);
                        }
                        else
                        {
                            // 창 띄워서 이미 연결되어있다고 알려주는 것도 괜찮을 듯
                        }
                        transitionStartStateBox = null;
                        SetSelectedState(null);
                        e.Use();
                    }
                    else
                    {
                        SetSelectedState(stateBoxClicked);
                    }
                }
                else if (transitionLineClicked != null)
                {
                    SetSelectedTransitionLine(transitionLineClicked);
                }
                else
                {
                    transitionStartStateBox = null;
                    SetSelectedState(null);
                    SetSelectedTransitionLine(null);
                }
            }

            // 우클릭일 때
            if (e.button == 1)
            {
                // 트랜지션 연결 중이 아니라면
                if (transitionStartStateBox == null)
                {
                    foreach (var box in listStateBox)
                    {
                        if (box.rect.Contains(mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add Transition"), false, StartTransition, box);
                            menu.AddItem(new GUIContent("Set This State to First"), false, SetFirstState, box);
                            menu.ShowAsContext();

                            e.Use();
                            break;
                        }
                    }
                }
            }
        }

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                if (selectedStateBox != null)
                {
                    RemoveSelectedStateBox();
                }
                if (selectedTransitionLine != null)
                {
                    RemoveSelectedTransitionLine();
                }
                e.Use();
            }
        }
    }

    private void Set()
    {
        for (int i = 0; i < target.patternStates.Count; i++)
        {
            AddStateBox(target.patternStates[i]);
        }

        for (int i = 0; i < listStateBox.Count; i++)
        {
            for (int j = 0; j < listStateBox[i].state.transitions.Count; j++)
            {
                StateBox targetBox = listStateBox.Find((box) => box.state == listStateBox[i].state.transitions[j].targetState);
                if (targetBox != null)
                {
                    AddTransitionLine(listStateBox[i], targetBox, listStateBox[i].state.transitions[j]);
                }
            }
        }
    }

    private StateBox AddStateBox(EnemyPatternState _state)
    {
        StateBox box = new StateBox();
        box.state = _state;
        if (_state.GetType() == typeof(EnemyNormalPatternState))
        {
            box.rect = new Rect(mousePosition.x, mousePosition.y, 220, 80);
        }
        else
        {
            box.rect = new Rect(mousePosition.x, mousePosition.y, 400, 0);
        }
        listStateBox.Add(box);
        return box;
    }

    private void AddTransitionLine(StateBox _masterBox, StateBox _targetBox, EnemyPatternTransition _transition)
    {
        listTransitionLine.Add(MakeTransitionLine(_masterBox, _targetBox, _transition));
    }

    private TransitionLine MakeTransitionLine(StateBox _masterBox, StateBox _targetBox, EnemyPatternTransition _transition)
    {
        TransitionLine newLine = new TransitionLine();
        newLine.transition = _transition;
        newLine.masterBox = _masterBox;
        newLine.targetBox = _targetBox;
        return newLine;
    }

    private void SetSelectedState(StateBox _stateBox)
    {
        if (_stateBox != null)
        {
            SetSelectedTransitionLine(null);
        }
        GUI.FocusControl(null);
        selectedStateBox = _stateBox;
    }

    private void SetSelectedTransitionLine(TransitionLine _line)
    {
        if (_line != null)
        {
            SetSelectedState(null);
        }
        GUI.FocusControl(null);
        selectedTransitionLine = _line;
    }

    private void AddNormalPatternState()
    {
        EnemyPatternState newState = ScriptableObject.CreateInstance<EnemyNormalPatternState>();
        UtilsEditor.CreateFolderIfNotExists(FolderPath);
        UtilsEditor.CreateAsset(newState, string.Format(FolderPath + "newState.asset"));
        target.patternStates.Add(newState);

        AddStateBox(newState);
    }
    private void AddRandomPatternState()
    {
        EnemyPatternState newState = ScriptableObject.CreateInstance<EnemyRandomPatternState>();
        UtilsEditor.CreateFolderIfNotExists(FolderPath);
        UtilsEditor.CreateAsset(newState, string.Format(FolderPath + "newState.asset"));
        target.patternStates.Add(newState);

        AddStateBox(newState);
    }

    private void AddTransition(StateBox _startBox, StateBox _targetBox)
    {
        EnemyPatternTransition newTransition = ScriptableObject.CreateInstance<EnemyPatternTransition>();
        newTransition.targetState = _targetBox.state;
        AssetDatabase.AddObjectToAsset(newTransition, _startBox.state);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _startBox.state.transitions.Add(newTransition);

        AddTransitionLine(_startBox, _targetBox, newTransition);
    }

    private void RemoveSelectedStateBox()
    {
        RemoveStateBox(selectedStateBox);
        selectedStateBox = null;
    }

    private void RemoveSelectedTransitionLine()
    {
        RemoveTransitionLine(selectedTransitionLine);
        selectedTransitionLine = null;
    }

    private void RemoveStateBox(StateBox _box)
    {
        if (_box == null)
        {
            return;
        }

        var linesToRemove = listTransitionLine.FindAll((line) => line.masterBox == _box || line.targetBox == _box).ToList();
        for (int i = 0; i < linesToRemove.Count; i++)
        {
            RemoveTransitionLine(linesToRemove[i]);
        }

        EnemyPatternState stateToRemove = _box.state;

        target.patternStates.Remove(stateToRemove);

        AssetDatabase.DeleteAsset(string.Format(FolderPath + _box.state.name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        listStateBox.Remove(_box);
    }

    private void RemoveTransitionLine(TransitionLine _line)
    {
        AssetDatabase.RemoveObjectFromAsset(_line.transition);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        for (int i = 0; i < target.patternStates.Count; i++)
        {
            target.patternStates[i].transitions.Remove(_line.transition);
        }

        listTransitionLine.Remove(_line);
    }

    private void SetFirstState(object _stateBoxObject)
    {
        StateBox stateBox = (StateBox)_stateBoxObject;
        listStateBox.Remove(stateBox);
        listStateBox.Insert(0, stateBox);
        target.patternStates.Remove(stateBox.state);
        target.patternStates.Insert(0, stateBox.state);
    }

    private void StartTransition(object _stateBoxObject)
    {
        StateBox stateBox = (StateBox)_stateBoxObject;
        transitionStartStateBox = stateBox;
    }

    private void DrawState()
    {
        foreach (var box in listStateBox)
        {
            GUI.Box(box.rect, box.state.name.ToString());
        }
    }

    private void DrawStateWindow(int _index)
    {
        StateBox stateBox = listStateBox[_index];
        if (_index == 0)
        {
            GUI.color = Color.red;
        }
        stateBox.rect = GUI.Window(_index, stateBox.rect, id => DrawStateContents(stateBox), stateBox.state.name.ToString());
        GUI.color = Color.white;
    }

    private void DrawTransitionLine()
    {
        foreach (var line in listTransitionLine)
        {
            DrawArrow(line.startPos, line.endPos, line == selectedTransitionLine ? Color.yellow : Color.white);
        }
    }

    private void CalculateTransitionLinePos()
    {
        foreach (var line in listTransitionLine)
        {
            StateBox stateBox = line.masterBox;
            StateBox targetState = line.targetBox;
            float a = 0.5f * (stateBox.rect.width < stateBox.rect.height ? stateBox.rect.width : stateBox.rect.height);
            float b = 0.5f * (targetState.rect.width < targetState.rect.height ? targetState.rect.width : targetState.rect.height);
            // 윈도우 중점 좌상단이라 y는 아래로 갈 수록 커짐
            float angle = Mathf.Rad2Deg * Mathf.Atan2(stateBox.rect.center.y - targetState.rect.center.y, targetState.rect.center.x - stateBox.rect.center.x);
            float startAngleRad = Mathf.Deg2Rad * (angle + 45f);
            float endAngleRad = Mathf.Deg2Rad * (180f + angle - 45f);
            Vector2 startPos = stateBox.rect.center + a * new Vector2(Mathf.Cos(startAngleRad), -Mathf.Sin(startAngleRad));
            Vector2 endPos = targetState.rect.center + b * new Vector2(Mathf.Cos(endAngleRad), -Mathf.Sin(endAngleRad));
            // Rect 밖에서 시작하고 끝나도록
            // Rect 경계 계산
            startPos = GetNearestPosOnBorder(startPos, angle, stateBox.rect);
            endPos = GetNearestPosOnBorder(endPos, 180f + angle, targetState.rect);

            line.startPos = startPos;
            line.endPos = endPos;

            if (line == selectedTransitionLine)
            {
                Vector2 size = new Vector2(220f, 100f);
                Vector2 bottomPos = 0.5f * (startPos + endPos) - size * 0.5f - new Vector2(0f, 10f);
                Rect rect = new Rect();
                rect.center = new Vector2(bottomPos.x, bottomPos.y - size.y * 0.5f);
                rect.size = size;

                transitionInfoBox.rect = rect;
                transitionInfoBox.transition = line.transition;
            }
        }
    }

    private void DrawStateContents(StateBox _stateBox)
    {
        GUILayout.BeginVertical();

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;
        if (_stateBox.state.GetType() == typeof(EnemyNormalPatternState))
        {
            EditorGUILayout.BeginHorizontal();

            EnemyNormalPatternState state = (EnemyNormalPatternState)_stateBox.state;
            state.pattern = EditorGUILayout.ObjectField("Pattern", state.pattern, typeof(EnemyPattern), false) as EnemyPattern;

            EditorGUILayout.EndHorizontal();
        }
        else if (_stateBox.state.GetType() == typeof(EnemyRandomPatternState))
        {
            float totalHeight = 0f;
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EnemyRandomPatternState state = (EnemyRandomPatternState)_stateBox.state;
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
            _stateBox.rect.height = totalHeight;
        }
        EditorGUIUtility.labelWidth = originalLabelWidth;

        GUILayout.EndVertical();

        GUI.DragWindow();
    }


    // rect위치와 크기에 Transition 정보 보여줌
    private void DrawTransitionInfo()
    {
        if (selectedTransitionLine == null)
        {
            return;
        }
        GUI.Box(transitionInfoBox.rect, "Transition Info");
        transitionInfoBox.rect = GUI.Window(target.patternStates.Count + 1, transitionInfoBox.rect, id => DrawTransitionContents(transitionInfoBox.transition), "Transition Info");
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

        EditorGUILayout.BeginHorizontal();
        // 값을 못 바꾸도록 함
        GUI.enabled = false;
        _transition.targetState = EditorGUILayout.ObjectField("Target", _transition.targetState, typeof(EnemyPatternState), false) as EnemyPatternState;
        GUI.enabled = true;
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

    private void DrawArrow(Vector3 _startPos, Vector3 _endPos, Color _color)
    {
        Handles.color = _color;

        Handles.DrawLine(_startPos, _endPos);

        float angle = Mathf.Rad2Deg * Mathf.Atan2(_endPos.y - _startPos.y, _startPos.x - _endPos.x);
        Vector3 right = new Vector3(Mathf.Cos((angle - 30f) * Mathf.Deg2Rad), -Mathf.Sin((angle - 30f) * Mathf.Deg2Rad));
        Vector3 left = new Vector3(Mathf.Cos((angle + 30f) * Mathf.Deg2Rad), -Mathf.Sin((angle + 30f) * Mathf.Deg2Rad));

        Handles.DrawLine(_endPos, _endPos + right * 20f);
        Handles.DrawLine(_endPos, _endPos + left * 20f);

        Handles.color = Color.white;
    }
}
