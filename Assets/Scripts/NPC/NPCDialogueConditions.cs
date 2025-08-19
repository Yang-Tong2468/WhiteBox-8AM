/*
对话条件管理器
  - 根据任务状态选择不同对话
  - 支持道具条件检查
  - 优先级系统决定对话顺序
*/
using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

[System.Serializable]
public class DialogueCondition
{
    [Header("对话信息")]
    public string conversationTitle;
    public string description;
    
    [Header("任务条件")]
    public List<string> requiredCompletedQuests = new List<string>();
    public List<string> requiredActiveQuests = new List<string>();
    public List<string> blockedByQuests = new List<string>();
    
    [Header("道具条件")]
    public List<string> requiredItems = new List<string>();
    public List<int> requiredItemCounts = new List<int>();
    
    [Header("自定义条件")]
    public List<string> customVariables = new List<string>();
    public List<bool> customVariableValues = new List<bool>();
    
    [Header("优先级")]
    public int priority = 0; // 优先级越高越优先
}

public class NPCDialogueConditions : MonoBehaviour
{
    [Header("对话配置")]
    public List<DialogueCondition> dialogueConditions = new List<DialogueCondition>();
    public string defaultConversation = "";
    
    [Header("调试信息")]
    public bool showDebugInfo = true;
    
    public string GetCurrentConversation()
    {
        if (showDebugInfo)
        {
            Debug.Log($"检查 {gameObject.name} 的对话条件...");
        }
        
        // 按优先级排序
        dialogueConditions.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        foreach (DialogueCondition condition in dialogueConditions)
        {
            if (CheckCondition(condition))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"满足条件，启动对话: {condition.conversationTitle}");
                }
                return condition.conversationTitle;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"无特殊条件满足，使用默认对话: {defaultConversation}");
        }
        
        return defaultConversation;
    }
    
    bool CheckCondition(DialogueCondition condition)
    {
        // 检查任务条件
        if (!CheckQuestConditions(condition))
        {
            return false;
        }
        
        // 检查道具条件
        if (!CheckItemConditions(condition))
        {
            return false;
        }
        
        // 检查自定义变量条件
        if (!CheckCustomVariableConditions(condition))
        {
            return false;
        }
        
        return true;
    }
    
    bool CheckQuestConditions(DialogueCondition condition)
    {
        // 检查必须完成的任务
        foreach (string questName in condition.requiredCompletedQuests)
        {
            if (!IsQuestCompleted(questName))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"任务 {questName} 未完成，条件不满足");
                }
                return false;
            }
        }
        
        // 检查必须激活的任务
        foreach (string questName in condition.requiredActiveQuests)
        {
            if (!IsQuestActive(questName))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"任务 {questName} 未激活，条件不满足");
                }
                return false;
            }
        }
        
        // 检查不能有的任务（阻塞任务）
        foreach (string questName in condition.blockedByQuests)
        {
            if (IsQuestActive(questName) || IsQuestCompleted(questName))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"被任务 {questName} 阻塞，条件不满足");
                }
                return false;
            }
        }
        
        return true;
    }
    
    bool CheckItemConditions(DialogueCondition condition)
    {
        for (int i = 0; i < condition.requiredItems.Count; i++)
        {
            string itemName = condition.requiredItems[i];
            int requiredCount = i < condition.requiredItemCounts.Count ? condition.requiredItemCounts[i] : 1;
            
            if (!HasItem(itemName, requiredCount))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"缺少道具 {itemName} x{requiredCount}，条件不满足");
                }
                return false;
            }
        }
        
        return true;
    }
    
    bool CheckCustomVariableConditions(DialogueCondition condition)
    {
        for (int i = 0; i < condition.customVariables.Count; i++)
        {
            string variableName = condition.customVariables[i];
            bool requiredValue = i < condition.customVariableValues.Count ? condition.customVariableValues[i] : true;
            
            if (!CheckCustomVariable(variableName, requiredValue))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"自定义变量 {variableName} 不满足条件");
                }
                return false;
            }
        }
        
        return true;
    }
    
    // 以下方法为接口预留，后续集成具体系统时实现
    bool IsQuestCompleted(string questName)
    {
        // 预留给Quest Machine集成
        // 临时使用Dialogue System的变量系统
        return DialogueLua.GetVariable($"Quest[\"{questName}\"].State").asString == "success";
    }
    
    bool IsQuestActive(string questName)
    {
        // 预留给Quest Machine集成
        // 临时使用Dialogue System的变量系统
        return DialogueLua.GetVariable($"Quest[\"{questName}\"].State").asString == "active";
    }
    
    bool HasItem(string itemName, int count)
    {
        // 预留给Ultimate Inventory System集成
        // 临时使用Dialogue System的变量系统
        return DialogueLua.GetVariable($"Item[\"{itemName}\"]").asInt >= count;
    }
    
    bool CheckCustomVariable(string variableName, bool expectedValue)
    {
        // 使用Dialogue System的变量系统
        return DialogueLua.GetVariable(variableName).asBool == expectedValue;
    }
    
    // 公共方法用于设置变量（调试用）
    public void SetCustomVariable(string variableName, bool value)
    {
        DialogueLua.SetVariable(variableName, value);
    }
    
    public void SetItemCount(string itemName, int count)
    {
        DialogueLua.SetVariable($"Item[\"{itemName}\"]", count);
    }
    
    public void SetQuestState(string questName, string state)
    {
        DialogueLua.SetVariable($"Quest[\"{questName}\"].State", state);
    }
}