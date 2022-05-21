using System;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [ExposedPropertyName("Bool"), ExposedPropertyColor(200, 200, 200), ExposedPropertyOrder(0)]
    public class ExposedBoolProperty : ExposedProperty
    {
        [SerializeField]
        bool value;
        public bool Value { get => value; set => this.value = value; }
        public override Type Type => typeof(bool);

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(object value)
        {
            this.value = (bool)value;
        }
    }
}
