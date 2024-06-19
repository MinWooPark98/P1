using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableMapRandomType : DataTable
{
    public override string Path => "DataTable/MapRandomType";

    public struct Books
    {
        public int TYPE;                    // �� Ÿ��
        public float RATE;                  // Ȯ��
    }


    private List<Books> books = new List<Books>();

    public override void Load()
    {
        books = CSVReader.MakeList<Books>(FileLoad());
    }

    public Books GetBook(MapType _mapType)
    {
        if (books.Count <= 0)
        {
            Load();
        }

        for (int i = 0; i < books.Count; i++)
        {
            if (books[i].TYPE == (int)_mapType)
            {
                return books[i];
            }
        }

        LogManager.LogError(GetType().ToString() + "_���ǿ� �´� ������ ����");
        return books[0];
    }

    public List<Books> GetBooks()
    {
        return books;
    }
}
