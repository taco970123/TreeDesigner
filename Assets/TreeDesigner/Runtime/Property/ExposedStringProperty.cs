using System;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [ExposedPropertyName("String"), ExposedPropertyColor(204, 176, 97), ExposedPropertyOrder(3)]
    public class ExposedStringProperty : ExposedProperty
    {
        [SerializeField]
        string value;
        public string Value { get => value; set => this.value = value; }
        public override Type Type => typeof(string);

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(object value)
        {
            this.value = (string)value;
        }
    }
}