using System;
using System.Collections;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [Serializable]
    public abstract class SharedValue
    {
        [HideInInspector]
        public ExposedProperty exposedProperty;
    }
    [Serializable]
    public class SharedBoolValue : SharedValue
    {
        ExposedBoolProperty exposedBoolProperty => exposedProperty as ExposedBoolProperty;

        [SerializeField]
        bool value;
        public bool Value { get => GetValue(); set => SetValue(value); }


        bool GetValue()
        {
            return exposedProperty ? exposedBoolProperty.Value : value;
        }
        void SetValue(bool newValue)
        {
            if (exposedProperty)
                exposedBoolProperty.Value = newValue;
            value = newValue;
        }
    }
    [Serializable]
    public class SharedIntValue : SharedValue
    {
        ExposedIntProperty exposedIntProperty => exposedProperty as ExposedIntProperty;

        [SerializeField]
        int value;
        public SharedIntValue(int value)
        {
            this.value = value;
        }
        public int Value { get => GetValue(); set => SetValue(value); }

        int GetValue()
        {
            return exposedProperty ? exposedIntProperty.Value : value;
        }
        void SetValue(int newValue)
        {
            if (exposedProperty)
                exposedIntProperty.Value = newValue;
            value = newValue;
        }
    }
    [Serializable]
    public class SharedFloatValue : SharedValue
    {
        ExposedFloatProperty exposedFloatProperty => exposedProperty as ExposedFloatProperty;

        [SerializeField]
        float value;
        public float Value { get => GetValue(); set => SetValue(value); }

        float GetValue()
        {
            return exposedProperty ? exposedFloatProperty.Value : value;
        }
        void SetValue(float newValue)
        {
            if (exposedProperty)
                exposedFloatProperty.Value = newValue;
            value = newValue;
        }
    }
    [Serializable]
    public class SharedStringValue : SharedValue
    {
        ExposedStringProperty exposedStringProperty => exposedProperty as ExposedStringProperty;

        [SerializeField]
        string value;
        public string Value { get => GetValue(); set => SetValue(value); }

        string GetValue()
        {
            return exposedProperty ? exposedStringProperty.Value : value;
        }
        void SetValue(string newValue)
        {
            if (exposedProperty)
                exposedStringProperty.Value = newValue;
            value = newValue;
        }
    }
    [Serializable]
    public class SharedListValue : SharedValue
    {
        ExposedListProperty exposedListProperty => exposedProperty as ExposedListProperty;

        [SerializeField]
        IList value;
        public IList Value { get => GetValue(); set => SetValue(value); }

        IList GetValue()
        {
            return exposedProperty ? exposedListProperty.Value : value;
        }
        void SetValue(IList newValue)
        {
            if (exposedProperty)
                exposedListProperty.Value = newValue;
            value = newValue;
        }
    }
}