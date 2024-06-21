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
        public int GET_FROM;                    // È¹µæÃ³
        public float UNCOMMON;                  // Uncommon È¹µæ È®·ü
        public float COMMON;                    // Common È¹µæ È®·ü
        public float RARE;                      // Rare È¹µæ È®·ü
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

        LogManager.LogError(GetType().ToString() + "_Á¶°Ç¿¡ ¸Â´Â µ¥ÀÌÅÍ ¾øÀ½", this);
        return books[0];
    }
}
