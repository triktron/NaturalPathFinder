using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache<T>
{
    public delegate T[] CalculationFunction();
    public Cache(CalculationFunction func)
    {
        _Func = func;
    }

    private CalculationFunction _Func;
    private T[] _Value = new T[0];
    private bool _IsDirty = false;

    public T[] GetValue() {
        if (_IsDirty)
        {
            _Value = _Func();
            _IsDirty = false;
        }

        return _Value;
    }

    public void SetDirty()
    {
        _IsDirty = true;
    }
}
