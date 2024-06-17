using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

public static class CSVReader
{
    public static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    public static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    public static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string _data, int _headerIdx = 0)
    {
        var list = new List<Dictionary<string, object>>();
        var lines = Regex.Split(_data, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        int sidx = _headerIdx + 1;
        var header = Regex.Split(lines[_headerIdx], SPLIT_RE);
        for (var i = sidx; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;
            // 문자열이 없을 경우에만 예외처리
            if (values.Length <= 0) continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                else
                {
                    /////////////////////////////////
                    // 인트 인식때문에 수정
                    finalvalue = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n").Replace("\\", string.Empty).Replace("\"\"", "\"");
                    //finalvalue = value.Replace("\\n", "\n").Replace("\\", string.Empty).Replace("\"\"", "\"");
                    ////////////////////////////////
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<T> MakeList<T>(string _data, int _headerIdx = 0) where T : new()
    {
        List<Dictionary<string, object>> temp = Read(_data, _headerIdx);

        List<T> list = new List<T>();
        for (int i = 0; i < temp.Count; ++i)
        {
            list.Add(DictionaryToObject<T>(temp[i]));
            temp[i].Clear();
            temp[i] = null;
        }

        temp.Clear();
        temp = null;

        return list;
    }

    public static List<T> MakeList<T>(List<Dictionary<string, object>> _data) where T : new()
    {
        List<T> list = new List<T>();
        for (int i = 0; i < _data.Count; ++i)
            list.Add(DictionaryToObject<T>(_data[i]));

        return list;
    }

    private static T DictionaryToObject<T>(Dictionary<string, object> _data) where T : new()
    {
        object obj = new T();
        Type type = obj.GetType();
        Type strType = typeof(string);

        // 프로퍼티
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            //if (!_data.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
            if (!_data.Any(x => x.Key.Equals(property.Name, StringComparison.Ordinal)))
                continue;

            //KeyValuePair<string, object> item = _data.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
            KeyValuePair<string, object> item = _data.First(x => x.Key.Equals(property.Name, StringComparison.Ordinal));

            PropertyInfo pinfo = type.GetProperty(property.Name);

            // Find which property type (int, string, double? etc) the CURRENT property is...
            Type tPropertyType = pinfo.PropertyType;

            // Fix nullables...
            Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

            // 변수가 string이 아닌 경우 입력값 예외처리
            if (newT != strType && string.IsNullOrEmpty(item.Value.ToString()))
                continue;

            // ...and change the type
            object newA = Convert.ChangeType(item.Value, newT);
            pinfo.SetValue(obj, newA, null);
        }

        // 필드
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            //if (!_data.Any(x => x.Key.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)))
            if (!_data.Any(x => x.Key.Equals(field.Name, StringComparison.Ordinal)))
                continue;

            //KeyValuePair<string, object> item = _data.First(x => x.Key.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase));
            KeyValuePair<string, object> item = _data.First(x => x.Key.Equals(field.Name, StringComparison.Ordinal));

            FieldInfo finfo = type.GetField(field.Name);

            Type tFieldType = finfo.FieldType;

            Type newT = Nullable.GetUnderlyingType(tFieldType) ?? tFieldType;

            // 변수가 string이 아닌 경우 입력값 예외처리
            if (newT != strType && string.IsNullOrEmpty(item.Value.ToString()))
                continue;

            object newA = Convert.ChangeType(item.Value, newT);
            finfo.SetValue(obj, newA);
        }

        return (T)obj;
    }
}
