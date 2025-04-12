using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
#endif


[CreateAssetMenu(fileName = "GameDatabase", menuName = "GameDatabase")]
public class GameDatabase : ScriptableObject
{
    
    public static GameDatabase Instance
    {
        get
        {
            if (s_Instance == null)
            {
                var db = Resources.Load<GameDatabase>("GameDatabase");

                if (db == null)
                {
                    Debug.LogError("Game Database couldn't be found.");
                    return null;
                }

                s_Instance = db;
            }

            return s_Instance;
        }
    }

    static GameDatabase s_Instance;

    public AmmoDatabase ammoDatabase;
    
}

[System.Serializable]
public class AmmoDatabase
{
    public int maxId = 0;
    public Queue<int> freeID = new Queue<int>();

    [System.Serializable]
    public class Entry
    {
        public string name;
        public int id;
    }

    public Entry[] entries;

#if  UNITY_EDITOR
    public Entry AddEntry(string name)
    {
        Entry e = new Entry();
        
        if(freeID.Count > 0)
            e.id = freeID.Dequeue();
        else
        {
            e.id = maxId;
            maxId++;
        }

        e.name = name;

        ArrayUtility.Add(ref entries, e);

        return e;
    }
    
    public void RemoveEntry(Entry e)
    {
        freeID.Enqueue(e.id);
        ArrayUtility.Remove(ref entries, e);
    }
#endif

    public Entry GetEntry(string name)
    {
        return entries.First(entry => entry.name == name);
    }

    public Entry GetEntry(int id)
    {
        return entries.First(entry => entry.id == id);
    }
}



