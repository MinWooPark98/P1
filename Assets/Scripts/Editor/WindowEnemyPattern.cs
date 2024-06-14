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

    private void OnDestroy()
    {
        foreach (var box in listStateBox)
        {
            EditorUtility.SetDirty(box.state);
            AssetDatabase.SaveAssets();
        }

        foreach (var line in listTransitionLine)
        {
            EditorUtility.SetDirty(line.transition);
            AssetDatabase.SaveAssets();
        }
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
            if (transitionStartStateBox.rect.Contains(mousePosition))
            {
                float radius = 40f; 
                Handles.DrawWireDisc(transitionStartStateBox.rect.center + new Vector2(0f, 0.5f * transitionStartStateBox.rect.height), Vector3.forward, radius);
            }
            else
            {
                Handles.DrawLine(transitionStartStateBox.rect.center, mousePosition);
            }
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

        // ���콺 Ŭ��
        if (e.type == EventType.MouseDown)
        {
            // ��Ŭ���� ��
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
                    // ������ Ʈ������ ���� ���� �ǵ���� �� �����ϴ� �� ����
                }
                // �ڽ��� Ŭ���Ǿ��� ��
                else if (stateBoxClicked != null)
                {
                    // ���� Ʈ������ ���� ���̸�
                    if (transitionStartStateBox != null)
                    {
                        // �̹� ����Ǿ����� �ʾƾ� ����
                        if (transitionStartStateBox.state.transitions.Exists((transition) => transition.targetState == stateBoxClicked.state) == false)
                        {
                            AddTransition(transitionStartStateBox, stateBoxClicked);
                        }
                        else
                        {
                            // â ����� �̹� ����Ǿ��ִٰ� �˷��ִ� �͵� ������ ��
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

            // ��Ŭ���� ��
            if (e.button == 1)
            {
                // Ʈ������ ���� ���� �ƴ϶��
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

        // self transition�� �ƴ� ���� line �߰�
        if (_startBox != _targetBox)
        {
            AddTransitionLine(_startBox, _targetBox, newTransition);
        }
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

        RemoveState(_box.state);
        listStateBox.Remove(_box);
    }

    private void RemoveState(EnemyPatternState _state)
    {
        target.patternStates.Remove(_state);

        AssetDatabase.DeleteAsset(string.Format(FolderPath + _state.name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemoveTransitionLine(TransitionLine _line)
    {
        RemoveTransition(_line.transition);
        listTransitionLine.Remove(_line);
    }

    private void RemoveTransition(EnemyPatternTransition _transition)
    {
        for (int i = 0; i < target.patternStates.Count; i++)
        {
            target.patternStates[i].transitions.Remove(_transition);
        }
        AssetDatabase.RemoveObjectFromAsset(_transition);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
            // ������ ���� �»���̶� y�� �Ʒ��� �� ���� Ŀ��
            float angle = Mathf.Rad2Deg * Mathf.Atan2(stateBox.rect.center.y - targetState.rect.center.y, targetState.rect.center.x - stateBox.rect.center.x);
            float startAngleRad = Mathf.Deg2Rad * (angle + 45f);
            float endAngleRad = Mathf.Deg2Rad * (180f + angle - 45f);
            Vector2 startPos = stateBox.rect.center + a * new Vector2(Mathf.Cos(startAngleRad), -Mathf.Sin(startAngleRad));
            Vector2 endPos = targetState.rect.center + b * new Vector2(Mathf.Cos(endAngleRad), -Mathf.Sin(endAngleRad));
            // Rect �ۿ��� �����ϰ� ��������
            // Rect ��� ���
            startPos = GetNearestPosOnBorder(startPos, angle, stateBox.rect);
            endPos = GetNearestPosOnBorder(endPos, 180f + angle, targetState.rect);

            line.startPos = startPos;
            line.endPos = endPos;

            if (line == selectedTransitionLine)
            {
                transitionInfoBox.rect.center = 0.5f * (startPos + endPos) - new Vector2(0f, transitionInfoBox.rect.size.y * 0.5f + 10f);
                transitionInfoBox.transition = line.transition;
            }
        }
    }

    private void DrawStateContents(StateBox _stateBox)
    {
        GUILayout.BeginVertical();

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        float totalHeight = 0f;
        EditorGUIUtility.labelWidth = 60f;
        if (_stateBox.state.GetType() == typeof(EnemyNormalPatternState))
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EnemyNormalPatternState state = (EnemyNormalPatternState)_stateBox.state;            
            EnemyPatternTransition selfTransition = state.transitions.Find((transition) => transition.targetState == state);
            if (selfTransition == null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Self-Transition");
                EditorGUILayout.LabelField("null");

                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Self-Transition");
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.condition = (ENEMY_PATTERN_CONDITION)EditorGUILayout.EnumPopup("Condition", selfTransition.condition.condition);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.value = EditorGUILayout.FloatField("Value", selfTransition.condition.value);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.priority = EditorGUILayout.IntField("Priority", selfTransition.condition.priority);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (GUILayout.Button("Delete Self Transition"))
                {
                    RemoveTransition(selfTransition);
                }
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUILayout.Space(10f);
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUILayout.BeginHorizontal();
            state.pattern = EditorGUILayout.ObjectField("Pattern", state.pattern, typeof(EnemyPattern), false) as EnemyPattern;
            EditorGUILayout.EndHorizontal();
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        else if (_stateBox.state.GetType() == typeof(EnemyRandomPatternState))
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EnemyRandomPatternState state = (EnemyRandomPatternState)_stateBox.state;
            if (state.randomPatterns == null)
            {
                state.randomPatterns = new List<EnemyRandomPatternState.RandomPattern>();
            }

            EditorGUILayout.BeginVertical();

            EnemyPatternTransition selfTransition = state.transitions.Find((transition) => transition.targetState == state);
            if (selfTransition == null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Self-Transition");
                EditorGUILayout.LabelField("null");

                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Self-Transition");
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.condition = (ENEMY_PATTERN_CONDITION)EditorGUILayout.EnumPopup("Condition", selfTransition.condition.condition);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.value = EditorGUILayout.FloatField("Value", selfTransition.condition.value);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUILayout.BeginHorizontal();
                selfTransition.condition.priority = EditorGUILayout.IntField("Priority", selfTransition.condition.priority);
                EditorGUILayout.EndHorizontal();
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (GUILayout.Button("Delete Self Transition"))
                {
                    RemoveTransition(selfTransition);
                }
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUILayout.Space(10f);
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

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
        }
        _stateBox.rect.height = totalHeight;

        EditorGUIUtility.labelWidth = originalLabelWidth;

        GUILayout.EndVertical();

        GUI.DragWindow();
    }


    // rect��ġ�� ũ�⿡ Transition ���� ������
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
        float totalHeight = 0f;

        GUILayout.BeginVertical();

        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;

        EditorGUILayout.BeginHorizontal();
        _transition.condition.condition = (ENEMY_PATTERN_CONDITION)EditorGUILayout.EnumPopup("Condition", _transition.condition.condition);
        EditorGUILayout.EndHorizontal();
        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUILayout.BeginHorizontal();
        _transition.condition.value = EditorGUILayout.FloatField("Value", _transition.condition.value);
        EditorGUILayout.EndHorizontal();
        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUILayout.BeginHorizontal();
        _transition.condition.priority = EditorGUILayout.IntField("Priority", _transition.condition.priority);
        EditorGUILayout.EndHorizontal();
        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUILayout.BeginHorizontal();
        // ���� �� �ٲٵ��� ��
        GUI.enabled = false;
        _transition.targetState = EditorGUILayout.ObjectField("Target", _transition.targetState, typeof(EnemyPatternState), false) as EnemyPatternState;
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        totalHeight += 10f;

        transitionInfoBox.rect.size = new Vector2(220f, totalHeight);

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
