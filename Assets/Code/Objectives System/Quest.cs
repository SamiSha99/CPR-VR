using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEditor;
using System.Runtime.CompilerServices;

// Reference from https://www.youtube.com/watch?v=-65u991cdtw, the system itself isn't the same like in the video anymore as we need more features added.
// Quests represents objectives handled by the Quest Manager, all they do is store data and run accordingly to their designed function as a scriptable object.
// See goals for more info.
[System.Serializable]
public class Quest : ScriptableObject
{
    public const string QUEST_BEGIN_COMMAND = "_Begin";
    public const string QUEST_COMPLETE_COMMAND = "_Complete";

    [System.Serializable]
    public struct Info 
    {
        [Tooltip("Name of the information.")]
        public string name;
        [TextArea(4,4)]
        [Tooltip("Description of the information.")]
        public string description;
    };

    public Info information;
    [Tooltip("The expected time for this quest to be finished, each second missed beyond this time will remove points up to 10 after 30 seconds!")]
    public float averageTime;
    [Tooltip("Command listeners will recieve this command on different quest states using \"THIS + _CommandName\".\nCommand types:\n_Begin\n_Complete")]
    public string questCommand = "???";
    [Tooltip("On completion, immediatly start the following quest.")]
    public Quest nextQuest;
    public bool completed { get; protected set; }
    public QuestCompletedEvent questCompleted;
    public QuestGoalUpdatedEvent questGoalUpdated;
    [System.Serializable]
    public abstract class QuestGoal : ScriptableObject
    {
        public enum GoalUIType {
            GUIT_Default, // 0
            GUIT_Checkbox, // 1
            GUIT_ProgressBar, // 2
            GUIT_None // 3 (Empty)
        };
        public string editorQuestGoalName = "A Quest Goal";
        public const string QUEST_GOAL_COMPLETE_COMMAND = "_Goal_Complete";
        [TextArea(1,3)]
        public string description;
        public float currentAmount { get; protected set; }
        [Tooltip("The amount of times required to finish this goal to be \"Completed\".")]
        public float requiredAmount = 1;
        [Tooltip("Quest Canvas will showcase based on what we've selected for the UI type.\n\nGUIT_Default = \"1/10\"\nGUIT_Checkbox = Empty box with a checkmark on completion\nGUIT_ProgressBar = A fill in progress bar")]
        public GoalUIType _GoalUIType;
        [Tooltip("On Goal Completetion run this command, where \"THIS + _Goal_Complete\"")]
        public string goalCompletedCommand = "???";
        public bool completed { get; protected set; }
        [Tooltip("Contains list of \"names\" for allowed GameObjects.")]
        [SerializeField] [NonReorderable]
        public List<string> objectiveNameList;
        [Tooltip("A reference to goals found in this quest, this quest goal will NOT progress at all unless these QuestGoal(s) are completed.")]
        [NonReorderable]
        public List<QuestGoal> requiredGoals;
        [Tooltip("Don't play checkmark on completion, because the person who made this and writing this comment is not in the mood to fix the bug where it spams it.")]
        public bool silent;
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
            
            if(quest != null) quest.questGoalUpdated?.Invoke(this);
        }
        private void Complete(bool detachAndCleanup = true)
        {
            completed = true;
            currentAmount = requiredAmount;
            goalCompleted?.Invoke();
            if(detachAndCleanup) CleanUp();
        }
        private void Incomplete() => completed = false;
        public virtual void CleanUp() => goalCompleted.RemoveAllListeners();
        public virtual void Skip()
        {
            Complete();
            if(quest != null) quest.questGoalUpdated?.Invoke(this);
        }
        public virtual void QuestGoalUpdate() {}
        public bool IsValidToEvaluate(string objectname)
        {
            if(RequiresQuestGoalsCompleted()) return false;
            if(objectiveNameList.Count > 0 && !objectiveNameList.Contains(objectname)) return false;
            return true;
        }
        public bool RequiresQuestGoalsCompleted()
        {
            return requiredGoals.Count > 0 & !requiredGoals.All(x => x.completed);
        }

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
    // Cleanup Quest if all Goals were completed, return true if Quest Complete
    private bool CheckGoals()
    {
        completed = goals.All(g => g.completed);
        if(completed)
        {
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
            // Final clean up
            foreach(var goal in goals) goal.CleanUp();
            return true;
        }
        return false;
    }
    public void QuestUpdate() 
    {
        if(goals.Count > 0) foreach (QuestGoal g in goals) g.QuestGoalUpdate();
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
    SerializedProperty m_QuestCommandProperty;
    SerializedProperty m_QuestAverageTime;

    //SerializedProperty m_QuestNextQuestProperty;
    List<string> m_QuestGoalType;
    SerializedProperty m_QuestGoalListProperty;
    string folderPath;

    [MenuItem("Assets/Create/Quest", priority = 0)]
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
        m_QuestCommandProperty = serializedObject.FindProperty(nameof(Quest.questCommand));
        m_QuestAverageTime = serializedObject.FindProperty(nameof(Quest.averageTime));
        // next quest if specified
        //m_QuestNextQuestProperty = serializedObject.FindProperty(nameof(Quest.nextQuest));
        // goals
        m_QuestGoalListProperty = serializedObject.FindProperty(nameof(Quest.goals));
        var lookup = typeof(Quest.QuestGoal);
        m_QuestGoalType = System.AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
        .Select(type => type.Name)
        .ToList();

        folderPath = this.GetPathTillAssetFolder(true);
    }

    public override void OnInspectorGUI()
    {
        var child = m_QuestInfoProperty.Copy();
        var depth = child.depth;
        child.NextVisible(true);
        EditorGUILayout.LabelField("Quest Info", EditorStyles.boldLabel);
        // title + desc from info
        while(child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }
        
        // Add quest command to GUI
        child = m_QuestCommandProperty.Copy();
        EditorGUILayout.PropertyField(child, true);

        // Adds average time
        child = m_QuestAverageTime.Copy();
        EditorGUILayout.PropertyField(child, true);

        // Quest Goals + sort
        int choice = EditorGUILayout.Popup("Add new Quest Goal", -1, m_QuestGoalType.ToArray());
        
        // Extremely buggy, will make array NonReorderable
        //child = m_QuestGoalListProperty.Copy();
        //EditorGUILayout.PropertyField(child, true);

        if(choice != -1)
        {
            var newInstance = CreateInstance(m_QuestGoalType[choice]);
            newInstance.name = m_QuestGoalType[choice].ToString();
            AssetDatabase.AddObjectToAsset(newInstance, target);
            m_QuestGoalListProperty.InsertArrayElementAtIndex(m_QuestGoalListProperty.arraySize);
            m_QuestGoalListProperty.GetArrayElementAtIndex(m_QuestGoalListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }

        // Horizontal Line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        Editor ed = null;
        int toDelete = -1;
        
        // For icons
        Texture2D arrowUp = EditorGUIUtility.Load(folderPath + "/Icons/upArrow.png") as Texture2D;
        Texture2D arrowDown = EditorGUIUtility.Load(folderPath + "/Icons/downArrow.png") as Texture2D;

        // List cannot be modified if its exactly 1
        // Soft fix: Make it [NonReorderable] for now
        for (int i = 0; i < m_QuestGoalListProperty.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            SerializedProperty item = m_QuestGoalListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);
            obj.Update();
            obj.targetObject.name = ((Quest.QuestGoal)obj.targetObject).editorQuestGoalName;
            CreateCachedEditor(item.objectReferenceValue, null, ref ed);
            ed.OnInspectorGUI();
            var oldcolor = GUI.backgroundColor;
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayoutOption widthGUI = GUILayout.Width(25), heightGUI = GUILayout.Height(15);
            
            // Swappers
            if (m_QuestGoalListProperty.arraySize > 1)
            {
                GUI.backgroundColor = new Color(0.8f, 1.25f, 2.5f, 1);
                if (i != 0)
                {
                    if (GUILayout.Button(arrowUp, heightGUI, widthGUI))
                    {
                        m_QuestGoalListProperty.MoveArrayElement(i, i - 1);
                        break;
                    }
                }
                if (i != m_QuestGoalListProperty.arraySize - 1)
                {
                    if (GUILayout.Button(arrowDown, heightGUI, widthGUI))
                    {
                        m_QuestGoalListProperty.MoveArrayElement(i, i + 1);
                        break;
                    }
                }
            }
            
            GUI.backgroundColor = new Color(2.25f, 0.25f, 0.25f, 1);
            if (GUILayout.Button("X", heightGUI, widthGUI)) toDelete = i;

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = oldcolor;
            
            if(i < m_QuestGoalListProperty.arraySize - 1)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        if(toDelete != -1)
        {
            var item = m_QuestGoalListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);
            m_QuestGoalListProperty.DeleteArrayElementAtIndex(toDelete);
        }

        serializedObject.ApplyModifiedProperties();
    }

    string GetPath([CallerFilePath]string fileName = null) => fileName;
}
#endif