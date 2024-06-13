using UnityEditor;
using UnityEngine;

public static class UtilsEditor
{
    public static void CreateFolderIfNotExists(string _folderPath)
    {
        if (!AssetDatabase.IsValidFolder(_folderPath))
        {
            string[] folders = _folderPath.Split('/');
            string currentPath = "Assets";
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }
    }

    public static bool AssetExists(string _assetPath)
    {
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_assetPath);
        return asset != null;
    }

    public static void CreateAsset(Object _obj, string _assetPath)
    {
        if (_obj == null)
        {
            return;
        }

        int count = 1;
        string[] arrStr = _assetPath.Split(".");
        while (AssetExists(_assetPath))
        {
            _assetPath = string.Format(arrStr[0] + count + "." + arrStr[1]);
            count++;
        }
        AssetDatabase.CreateAsset(_obj, _assetPath);
    }   
}
