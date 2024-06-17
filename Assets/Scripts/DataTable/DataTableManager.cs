using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataTableManager
{
    private static List<DataTable> dataTables = new List<DataTable>();

    public static void LoadAll()
    {
        dataTables.Add(new TableCardRewardRarity());
        // ���̺� �þ ������ �߰�

        for (int i = 0; i < dataTables.Count; i++)
        {
            dataTables[i].Load();
        }
    }

    public static T GetTable<T>() where T : DataTable
    {
        return dataTables.Find((table) => table.GetType() == typeof(T)) as T;
    }
}
