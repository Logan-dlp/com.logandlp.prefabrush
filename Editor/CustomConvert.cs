#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.logandlp.prefabrush.editor
{
    public class CustomConvert
    {
        public static List<string> PrefabsListToStringList(List<GameObject> gameobjectsList)
        {
            List<string> PrefabsPathList = new();
            foreach (GameObject gameObject in gameobjectsList)
            {
                PrefabsPathList.Add(AssetDatabase.GetAssetPath(gameObject));
            }
            return PrefabsPathList;
        }
        
        public static List<GameObject> StringListToPrefabsList(List<string> pathList)
        {
            List<GameObject> PrefabsPathList = new();
            foreach (string path in pathList)
            {
                PrefabsPathList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            }
            return PrefabsPathList;
        }
    }
}

#endif // UNITY_EDITOR