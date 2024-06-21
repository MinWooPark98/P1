using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardRewardRarity : DataTable
{
    public override string Path => "DataTable/CardRewardRarity";

    public enum GetFrom
    {
        NormalCombats,
        EliteCombats,
        BossCombats,
        Shop,
    }

    public struct Books
    {
        public int GET_FROM;                    // ȹ��ó
        public float UNCOMMON;                  // Uncommon ȹ�� Ȯ��
        public float COMMON;                    // Common ȹ�� Ȯ��
        public float RARE;                      // Rare ȹ�� Ȯ��
    }


    private List<Books> books = new List<Books>();

    public override void Load()
    {
        books = CSVReader.MakeList<Books>(FileLoad());
    }

    public Books GetBook(GetFrom _getFrom)
    {
        if (books.Count <= 0)
        {
            Load();
        }

        for (int i = 0; i < books.Count; i++)
        {
            if (books[i].GET_FROM == (int)_getFrom)
            {
                return books[i];
            }
        }

        LogManager.LogError(GetType().ToString() + "_���ǿ� �´� ������ ����", this);
        return books[0];
    }
}
