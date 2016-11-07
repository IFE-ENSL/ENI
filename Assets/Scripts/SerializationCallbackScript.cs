using UnityEngine;
using System;
using System.Collections.Generic;

public class SerializationCallbackScript : MonoBehaviour, ISerializationCallbackReceiver
{
    public int Size;
    public List<int> _keys = new List<int>();
    public List<string> _values = new List<string>();

    public List<string> testlol = new List<string>();

    //Unity doesn't know how to serialize a Dictionary
    public Dictionary<int, string> _myDictionary = new Dictionary<int, string>()
    {
        {3, "I"},
        {4, "Love"},
        {5, "Unity"},
    };

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        /*if (Size != _keys.Count || Size != _values.Count)
        {
            _keys.Capacity = Size;
            _values.Capacity = Size;
        }*/

        foreach (var kvp in _myDictionary)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _myDictionary = new Dictionary<int, string>();

        for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            _myDictionary.Add(_keys[i], _values[i]);
    }

    void OnGUI()
    {
        foreach (var kvp in _myDictionary)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    }
}