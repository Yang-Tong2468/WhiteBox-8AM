/*
对话系统管理器
  - 统一管理对话系统
  - 对话时暂停游戏/禁用移动
  - 集中的变量管理
*/
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class GameDialogueManager : MonoBehaviour
{
    [Header("系统设置")]
    public bool initializeOnStart = true;
    public bool pauseGameDuringDialogue = true;
    
    [Header("Player设置")]
    public GameObject player;
    public string playerTag = "Player";
    
    [Header("调试设置")]
    public bool enableDebugMode = true;
    
    private static GameDialogueManager instance;
    public static GameDialogueManager Instance => instance;
    
    void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (initializeOnStart)
        {
            InitializeDialogueSystem();
        }
    }
    
    void InitializeDialogueSystem()
    {
        // 自动找到Player如果没有设置
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
            if (player == null)
            {
                Debug.LogWarning($"未找到标签为 {playerTag} 的Player对象，请确保Player对象有正确的标签");
            }
        }
        
        // 设置对话系统事件监听
        SetupDialogueEvents();
        
        // 初始化系统变量
        InitializeSystemVariables();
        
        if (enableDebugMode)
        {
            Debug.Log("对话系统初始化完成");
        }
    }
    
    void SetupDialogueEvents()
    {
        // 对话开始事件
        DialogueManager.instance.conversationStarted += OnConversationStarted;
        
        // 对话结束事件
        DialogueManager.instance.conversationEnded += OnConversationEnded;
    }
    
    void InitializeSystemVariables()
    {
        // 初始化玩家变量
        DialogueLua.SetVariable("Player.Name", "玩家");
        
        // 初始化游戏状态变量
        DialogueLua.SetVariable("GameStarted", true);
        
        if (enableDebugMode)
        {
            Debug.Log("系统变量初始化完成");
        }
    }
    
    void OnConversationStarted(Transform actor)
    {
        if (enableDebugMode)
        {
            Debug.Log($"对话开始: {actor.name}");
        }
        
        if (pauseGameDuringDialogue)
        {
            Time.timeScale = 0f;
        }
        
        // 禁用Player移动
        if (player != null)
        {
            PlayerMover playerController = player.GetComponent<PlayerMover>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }
    }
    
    void OnConversationEnded(Transform actor)
    {
        if (enableDebugMode)
        {
            Debug.Log($"对话结束: {actor.name}");
        }
        
        if (pauseGameDuringDialogue)
        {
            Time.timeScale = 1f;
        }
        
        // 重新启用Player移动
        if (player != null)
        {
            PlayerMover playerController = player.GetComponent<PlayerMover>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
    
    // 公共API方法
    public void StartConversation(string conversationTitle, Transform actor)
    {
        if (player != null)
        {
            DialogueManager.StartConversation(conversationTitle, actor, player.transform);
        }
        else
        {
            Debug.LogError("Player对象未设置，无法开始对话");
        }
    }
    
    public void SetVariable(string variableName, object value)
    {
        DialogueLua.SetVariable(variableName, value);
        
        if (enableDebugMode)
        {
            Debug.Log($"设置变量: {variableName} = {value}");
        }
    }
    
    public object GetVariable(string variableName)
    {
        return DialogueLua.GetVariable(variableName);
    }
    
    // Quest Machine集成预留接口
    public void OnQuestCompleted(string questName)
    {
        SetVariable($"Quest[\"{questName}\"].State", "success");
        
        if (enableDebugMode)
        {
            Debug.Log($"任务完成: {questName}");
        }
    }
    
    public void OnQuestStarted(string questName)
    {
        SetVariable($"Quest[\"{questName}\"].State", "active");
        
        if (enableDebugMode)
        {
            Debug.Log($"任务开始: {questName}");
        }
    }
    
    // Ultimate Inventory System集成预留接口
    public void OnItemAdded(string itemName, int count)
    {
        int currentCount = DialogueLua.GetVariable($"Item[\"{itemName}\"]").asInt;
        SetVariable($"Item[\"{itemName}\"]", currentCount + count);
        
        if (enableDebugMode)
        {
            Debug.Log($"获得道具: {itemName} x{count}");
        }
    }
    
    public void OnItemRemoved(string itemName, int count)
    {
        int currentCount = DialogueLua.GetVariable($"Item[\"{itemName}\"]").asInt;
        SetVariable($"Item[\"{itemName}\"]", Mathf.Max(0, currentCount - count));
        
        if (enableDebugMode)
        {
            Debug.Log($"失去道具: {itemName} x{count}");
        }
    }
    
    void OnDestroy()
    {
        // 清理事件监听
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationStarted -= OnConversationStarted;
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
        }
    }
}