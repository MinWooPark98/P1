using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataTable
{
    public abstract string Path { get; }

    public abstract void Load();

    protected string FileLoad()
    {
        string text = string.Empty;
        TextAsset asset = (TextAsset)Resources.Load(Path, typeof(TextAsset));
        if (asset != null)
        {
            text = asset.text;
            Resources.UnloadAsset(asset);
        }
        return text;
    }
}
