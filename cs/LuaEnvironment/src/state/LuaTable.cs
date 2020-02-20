using System;
using System.Collections.Generic;
using System.Text;

public class LuaTable
{
    public LuaTable Metatable;
    public List<LuaValue> Arr;
    public Dictionary<LuaValue, LuaValue> Map;
    public Dictionary<LuaValue, LuaValue> Keys;
    private LuaValue lastKey;

    public LuaTable(int nArr, int nRec)
    {
        if (nArr > 0)
        {
            Arr = new List<LuaValue>(nArr);
            for (int i = 0; i < nArr; i++)
            {
                Arr.Add(LuaValue.Nil);
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
        else
        {
            Map = new Dictionary<LuaValue, LuaValue>();
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

        return Map.GetValueOrDefault(key, defaultValue: LuaValue.Nil);
    }

    public void Put(LuaValue key, LuaValue val)
    {
        if (key.Value == null)
        {
            throw new LuaException("table index is nil!");
        }
        if (key.Value is double f && double.IsNaN(f))
        {
            throw new LuaException("table index is NaN!");
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
            Map[key] = val;
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

    public bool HasMetafield(string fieldName)
    {
        return Metatable?.Get(fieldName) != null;
    }

    public LuaValue NextKey(LuaValue key)
    {
        if (Keys == null || key == null)
        {
            InitKeys();
        }

        Keys.TryGetValue(key ?? LuaValue.Nil, out LuaValue nextKey);
        nextKey ??= LuaValue.Nil;

        if (nextKey == null && key != null && key != lastKey)
        {
            throw new LuaException("invalid key to 'next'");
        }

        return nextKey;
    }

    private void InitKeys()
    {
        Keys = new Dictionary<LuaValue, LuaValue>();
        LuaValue key = LuaValue.Nil;
        if (Arr != null)
        {
            for (int i = 0; i < Arr.Count; i++)
            {
                var v = Arr[i];
                if (v.Value != null)
                {
                    Keys[key] = (Int64)(i + 1);
                    key = (Int64)(i + 1);
                }
            }
        }

        if (Map != null)
        {
            foreach (var kv in Map)
            {
                if (kv.Value.Value != null)
                {
                    Keys[key] = kv.Key;
                    key = kv.Key;
                }
            }
        }

        lastKey = key;
    }
}

