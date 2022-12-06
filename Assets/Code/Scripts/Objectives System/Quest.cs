using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

// From https://www.youtube.com/watch?v=-65u991cdtw

public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct Info 
    {
        public string name, desc;
    };

    public Info information;
    public bool completed { get; protected set; }
    public QuestCompletedEvent questCompleted;

    public abstract class QuestGoal : ScriptableObject
    {
        protected string description;
        public int currentAmount { get; protected set; }
        public int requiredAmout = 1;

        public bool completed { get; protected set; }
        [HideInInspector] public UnityEvent goalCompleted;

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual void Initialize()
        {
            completed = false;
            currentAmount = 0;
        }

        protected void Evaluate()
        {
            if(currentAmount >= requiredAmout)
                Complete();
        }
        private void Complete()
        {
            completed = true;
            goalCompleted.Invoke();
            goalCompleted.RemoveAllListeners();
        }

        public virtual void Skip()
        {
            Complete();
        }
    }

    public List<QuestGoal> goals;

    public void Initialize()
    {
        completed = false;
        questCompleted = new QuestCompletedEvent();
        
        foreach (var goal in goals)
        {
            goal.Initialize();
            goal.goalCompleted.AddListener(delegate { CheckGoals(); }); 
        }
         
    }

    private void CheckGoals()
    {
        completed = goals.All(g => g.completed);
        if(completed)
        {
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
        }
    }
}

public class QuestCompletedEvent : UnityEvent<Quest> { }

#if UNITY_EDITOR
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    SerializedProperty m_QuestInfoProperty;

    List<string> m_QuestGoalType;
    SerializedProperty m_QuestGoalListProperty;

    [MenuItem("Assets/Quest", priority = 0)]
    public static void CreateQuest()
    {
        var newQuest = CreateInstance<Quest>();
        ProjectWindowUtil.CreateAsset(newQuest, "quest.asset");
    }

    void OnEnable() 
    {
        // info
        m_QuestInfoProperty = serializedObject.FindProperty(nameof(Quest.information));
        // goals
        m_QuestGoalListProperty = serializedObject.FindProperty(nameof(Quest.goals));
        var lookup = typeof(Quest.QuestGoal);
        m_QuestGoalType = System.AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
        .Select(type => type.Name)
        .ToList();
    }

    public override void OnInspectorGUI()
    {
        var child = m_QuestInfoProperty.Copy();
        var depth = child.depth;
        child.NextVisible(true);
        EditorGUILayout.LabelField("Quest info", EditorStyles.boldLabel);
        while(child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }

        // next goal?
        int choice = EditorGUILayout.Popup("Add new Quest Goal", -1, m_QuestGoalType.ToArray());

        if(choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(m_QuestGoalType[choice]);
            AssetDatabase.AddObjectToAsset(newInstance, target);
            m_QuestGoalListProperty.InsertArrayElementAtIndex(m_QuestGoalListProperty.arraySize);
            m_QuestGoalListProperty.GetArrayElementAtIndex(m_QuestGoalListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }

        Editor ed = null;
        int toDelete = -1;
        for(int i = 0; i < m_QuestGoalListProperty.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var item = m_QuestGoalListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);
            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);
            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();
            var oldcolor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(3, 0, 0, 1);
            if(GUILayout.Button("X", GUILayout.Width(32)))
            {
                toDelete = i;
            }
            GUI.backgroundColor = oldcolor;
            EditorGUILayout.EndHorizontal();
        }

        if(toDelete != -1)
        {
            var item = m_QuestGoalListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            m_QuestGoalListProperty.DeleteArrayElementAtIndex(toDelete);
            m_QuestGoalListProperty.DeleteArrayElementAtIndex(toDelete);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif