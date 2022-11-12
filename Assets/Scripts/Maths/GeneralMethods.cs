using System.Collections.Generic;

public static class GeneralMethods
{
    public static void Swap<T>(ref T a, ref T b) 
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

    public static void RemoveQuick<T>(ref List<T> list, int index)
    {
        if(list.Count == 0)
        {
            return;
        }
        else
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1); 
        }
    }
}
