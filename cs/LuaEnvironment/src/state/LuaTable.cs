using System;
using System.Collections.Generic;
using System.Text;

public class LuaTable
{
    public List<LuaValue> Arr;
    public Dictionary<LuaValue, LuaValue> Map;

    public LuaTable(int nArr, int nRec)
    {
        if (nArr > 0)
        {
            Arr = new List<LuaValue>(nArr);
            for (int i = 0; i < nArr; i++)
            {
                Arr.Add(new LuaValue(null));
            }
        }
        else
        {
            Arr = new List<LuaValue>();
        }
        if (nRec > 0)
        {
            Map = new Dictionary<LuaValue, LuaValue>(nRec);
        }
    }

    public LuaValue Get(LuaValue key)
    {
        LuaValue k = FloatToInteger(key);
        var (i, ok) = k.ToInteger();
        if (ok)
        {
            if (i >= 1 && i <= Arr.Count)
            {
                return Arr[(int)i - 1];
            }
        }

        return Map.GetValueOrDefault(key, defaultValue: new LuaValue(null));
    }

    public void Put(LuaValue key, LuaValue val)
    {
        if (key.Value == null)
        {
            throw new Exception("table index is nil!");
        }
        if (key.Value is double f && double.IsNaN(f))
        {
            throw new Exception("table index is NaN!");
        }
        key = FloatToInteger(key);

        if (key.Value is Int64 idx && idx >= 1)
        {
            int arrLen = Arr.Count;
            if (idx <= arrLen)
            {
                Arr[(int)idx - 1] = val;
                if (idx == arrLen && val.Value == null)
                {
                    ShrinkArray();
                }
                return;
            }
            if (idx == arrLen + 1)
            {
                Map?.Remove(key);
                if (val.Value != null)
                {
                    Arr.Add(val);
                    ExpandArray();
                }
                return;
            }
        }

        if (val.Value != null)
        {
            if (Map == null)
            {
                Map = new Dictionary<LuaValue, LuaValue>(8);
            }
            Map.Add(key, val);
        }
        else
        {
            Map.Remove(key);
        }
    }

    private LuaValue FloatToInteger(LuaValue key)
    {
        if (key.Value is double d)
        {
            var (i, ok) = Number.FloatToInteger(d);
            if (ok)
            {
                return i;
            }
        }
        return key;
    }

    private void ShrinkArray()
    {
        for (int i = Arr.Count - 1; i >= 0; i++)
        {
            if (Arr[i].Value == null)
            {
                Arr.RemoveAt(Arr.Count - 1);
            }
        }
    }

    private void ExpandArray()
    {
        for (int idx = Arr.Count + 1; true; idx++)
        {
            if (Map.TryGetValue(idx, out LuaValue val))
            {
                Map.Remove(val);
                Arr.Add(val);
            }
            else
            {
                break;
            }
        }
    }

    public int Len => Arr.Count;

    public LuaValue this[LuaValue key]
    {
        get => Get(key);
        set => Put(key, value);
    }
}

