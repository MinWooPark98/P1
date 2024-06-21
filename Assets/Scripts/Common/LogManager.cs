using UnityEngine;

public static class LogManager
{
    public static void Log(object _message)
    {
#if UNITY_EDITOR
        Debug.Log(_message);
#endif
    }

    public static void LogError(object _message, object _classObj, string _methodName = "")
    {
#if UNITY_EDITOR
        Debug.Log(string.Format("ERROR :: " + _message + "  by " + _classObj.GetType() + " " + _methodName));
#endif
    }

    public static void Log(object _message, Object _context)
    {
#if UNITY_EDITOR
        Debug.Log(_message, _context);
#endif
    }
}
