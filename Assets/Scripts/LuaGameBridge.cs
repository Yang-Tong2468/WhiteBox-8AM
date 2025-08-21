using System;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Opsive.UltimateInventorySystem.Core;
public class LuaGameBridge : MonoBehaviour
{
    void Start()
    {
        Lua.RegisterFunction("FindGameObject", this,
typeof(LuaGameBridge).GetMethod("FindGameObject"));
        Lua.RegisterFunction("SetActive", this,
typeof(LuaGameBridge).GetMethod("SetActive"));
        Lua.RegisterFunction("DeactivateObject", this,
typeof(LuaGameBridge).GetMethod("DeactivateObject"));
        Lua.RegisterFunction("DeactivateAllActive", this,
typeof(LuaGameBridge).GetMethod("DeactivateAllActive"));
    }

    public GameObject FindGameObject(string name)
    {
        return GameObject.Find(name);
    }

    public void SetActive(string name, bool active)
    {
        // 方法1: 使用Find查找激活的GameObject
        GameObject obj = GameObject.Find(name);
        
        // 方法2: 如果Find失败，尝试查找所有GameObject（包括非激活的）
        if (obj == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in allObjects)
            {
                if (go.name == name && go.scene.name != null) // 确保是场景中的物体，不是预制体
                {
                    obj = go;
                    break;
                }
            }
        }
        
        if (obj != null) 
        {
            obj.SetActive(active);
            Debug.Log($"{name} 找到并设置为 {active}，当前状态: {obj.activeSelf}");
        }
        else
        {
            Debug.LogError($"未找到名为 '{name}' 的GameObject！请检查名称是否正确。");
            
            // 列出所有包含"NPC"的GameObject名称，帮助调试
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            Debug.Log("场景中包含'NPC'的GameObject：");
            foreach (GameObject go in allObjects)
            {
                if (go.name.Contains("NPC") && go.scene.name != null)
                {
                    Debug.Log($"- {go.name} (激活状态: {go.activeSelf})");
                }
            }
        }
    }

    // 关闭指定名称的GameObject
    public void DeactivateObject(string name)
    {
        SetActive(name, false);
    }

    // 关闭所有当前激活的指定类型GameObject（如所有NPC）
    public void DeactivateAllActive(string namePattern)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int deactivatedCount = 0;
        
        foreach (GameObject go in allObjects)
        {
            if (go.scene.name != null && go.activeSelf && go.name.Contains(namePattern))
            {
                go.SetActive(false);
                deactivatedCount++;
                Debug.Log($"关闭了 {go.name}");
            }
        }
        
        Debug.Log($"总共关闭了 {deactivatedCount} 个包含 '{namePattern}' 的激活GameObject");
    }
}