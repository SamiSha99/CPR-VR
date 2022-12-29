using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEditor;

// From https://www.youtube.com/watch?v=-65u991cdtw
// Quests represents objectives handled by the Quest Manager, all they do is store data and run accordingly to their designed function as a scriptable object.
// See goals for more info.
[System.Serializable]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct Info 
    {
        [Tooltip("Name of the information.")]
        public string name;
        [Tooltip("Description of the information.")]
        public string desc;
    };

    public Info information;
    [Tooltip("Command listeners will recieve this command on quest begin to react accordingly.")]
    public string beginQuestCommand = "";
    [Tooltip("Command listeners will recieve this command on quest completion to react accordingly.")]
    public string completeQuestCommand = "";
    public Quest nextQuest;
    public bool completed { get; protected set; }
    public QuestCompletedEvent questCompleted;
    public QuestGoalUpdatedEvent questGoalUpdated;
    public abstract class QuestGoal : ScriptableObject
    {
        public string description;
        public int currentAmount { get; protected set; }
        [Tooltip("The amount of times required to finish this goal to be \"Completed\".")]
        public int requiredAmount = 1;
        public string goalCompletedCommand = "";
        public bool completed { get; protected set; }
        [HideInInspector] public UnityEvent goalCompleted;
        // Quest related to this goal
        private Quest quest;
        [HideInInspector] public int index = 0;

        public virtual string GetDescription() { return description; }
        
        public virtual void Initialize()
        {
            completed = false;
            currentAmount = 0;
        }

        public virtual void Initialize(Quest q)
        {
            quest = q;
            Initialize();
        }

        protected void Evaluate(bool detachAndCleanup = true)
        {
            if(currentAmount >= requiredAmount)
                Complete(detachAndCleanup);
            else
                Incomplete();
            if(quest != null)
                quest.questGoalUpdated?.Invoke(this);
        }

        private void Complete(bool detachAndCleanup = true)
        {
            completed = true;
            goalCompleted.Invoke();
            if(detachAndCleanup) CleanUp();
        }
        
        private void Incomplete()
        {
            completed = false;
        }

        public virtual void CleanUp() => goalCompleted.RemoveAllListeners();
        public virtual void Skip() => Complete();
    }

    public List<QuestGoal> goals;
    // Init the Quest, returns the quest itself on completion
    public Quest Initialize()
    {
        completed = false;
        questCompleted = new QuestCompletedEvent();
        questGoalUpdated = new QuestGoalUpdatedEvent();
        
        int index = 0;
        foreach (var goal in goals)
        {
            goal.Initialize(this);
            goal.goalCompleted.AddListener(delegate { CheckGoals(); }); 
            goal.index = index;
            index++;
        }
        return this;
    }
    // Adds the Delegate call from QuestManager, return the Quest itself
    public Quest Initialize(UnityAction<Quest> manager, UnityAction<QuestGoal> updater)
    {
        Quest q = Initialize();
        
        if(manager != null) questCompleted.AddListener(manager);
        if(updater != null) questGoalUpdated.AddListener(updater);

        return q;
    }

    private void CheckGoals()
    {
        completed = goals.All(g => g.completed);
        if(completed)
        {
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
            // Final clean up
            foreach(var goal in goals) goal.CleanUp();
        }
    }
}

public class QuestCompletedEvent : UnityEvent<Quest> { }
public class QuestGoalUpdatedEvent : UnityEvent<Quest.QuestGoal> { }

#if UNITY_EDITOR
// Unity inspector editor for the Quest system.
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    SerializedProperty m_QuestInfoProperty;
    SerializedProperty m_QuestBeginCommandProperty;
    SerializedProperty m_QuestCompleteCommandProperty;
    SerializedProperty m_QuestNextQuestProperty;
    List<string> m_QuestGoalType;
    SerializedProperty m_QuestGoalListProperty;

    [MenuItem("Assets/Create/New Quest", priority = 0)]
    public static void CreateQuest()
    {
        var newQuest = CreateInstance<Quest>();
        ProjectWindowUtil.CreateAsset(newQuest, "QuestName.asset");
    }

    void OnEnable() 
    {
        // info
        m_QuestInfoProperty = serializedObject.FindProperty(nameof(Quest.information));
        // commands when beginning and completing the quest
        m_QuestBeginCommandProperty = serializedObject.FindProperty(nameof(Quest.beginQuestCommand));
        m_QuestCompleteCommandProperty = serializedObject.FindProperty(nameof(Quest.completeQuestCommand));
        // next quest if specified
        m_QuestNextQuestProperty = serializedObject.FindProperty(nameof(Quest.nextQuest));
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
        // title + desc from info
        while(child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }
        // Add begin and complete command to GUI
        child = m_QuestBeginCommandProperty.Copy();
        EditorGUILayout.PropertyField(child, true);
        child = m_QuestCompleteCommandProperty.Copy();
        EditorGUILayout.PropertyField(child, true);
        // Add quest
        child = m_QuestNextQuestProperty.Copy();
        EditorGUILayout.PropertyField(child, true);
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
        Texture2D arrowUp = EditorGUIUtility.Load(GlobalHelper.GetIcon("upArrow.png")) as Texture2D,
            arrowDown = EditorGUIUtility.Load(GlobalHelper.GetIcon("downArrow.png")) as Texture2D;

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