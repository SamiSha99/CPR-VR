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

        public virtual string GetDescription() { return description; }
        
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
            CleanUp();
        }

        protected abstract void CleanUp();

        public virtual void Skip() => Complete();
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

    public void Initialize(UnityAction<Quest> manager)
    {
        Initialize();
        if(manager != null)
            questCompleted.AddListener(manager);
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
        ProjectWindowUtil.CreateAsset(newQuest, "QuestName.asset");
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
            var newInstance = CreateInstance(m_QuestGoalType[choice]);
            AssetDatabase.AddObjectToAsset(newInstance, target);
            m_QuestGoalListProperty.InsertArrayElementAtIndex(m_QuestGoalListProperty.arraySize);
            m_QuestGoalListProperty.GetArrayElementAtIndex(m_QuestGoalListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }

        // Horizontal Line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        Editor ed = null;
        int toDelete = -1;
        Texture2D arrowUp = EditorGUIUtility.Load("Assets/Icons/upArrow.png") as Texture2D,
            arrowDown = EditorGUIUtility.Load("Assets/Icons/downArrow.png") as Texture2D;

        for (int i = 0; i < m_QuestGoalListProperty.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            SerializedProperty item = m_QuestGoalListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);
            CreateCachedEditor(item.objectReferenceValue, null, ref ed);
            ed.OnInspectorGUI();
            var oldcolor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(2f, 0.2f, 0.2f, 1);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.Height(28), GUILayout.Width(48)))
            {
                toDelete = i;
            }

            // Swappers
            if (m_QuestGoalListProperty.arraySize > 1)
            {
                GUI.backgroundColor = new Color(0.8f, 1.25f, 2.5f, 1);
                if (i != 0)
                {
                    if (GUILayout.Button(arrowUp, GUILayout.Height(28), GUILayout.Width(48)))
                    {
                        m_QuestGoalListProperty.MoveArrayElement(i, i-1);
                        break;
                    }
                }
                if (i != m_QuestGoalListProperty.arraySize - 1)
                {
                    if (GUILayout.Button(arrowDown, GUILayout.Height(28), GUILayout.Width(48)))
                    {
                        m_QuestGoalListProperty.MoveArrayElement(i, i + 1);
                        break;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = oldcolor;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        if(toDelete != -1)
        {
            var item = m_QuestGoalListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            m_QuestGoalListProperty.DeleteArrayElementAtIndex(toDelete);
            //m_QuestGoalListProperty.DeleteArrayElementAtIndex(toDelete);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif