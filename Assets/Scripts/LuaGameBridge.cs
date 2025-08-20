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
    }

    public GameObject FindGameObject(string name)
    {
        return GameObject.Find(name);
    }

    public void SetActive(string name, bool active)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) obj.SetActive(active);
    }
}