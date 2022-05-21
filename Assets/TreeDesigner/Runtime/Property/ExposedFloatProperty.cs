using System;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [ExposedPropertyName("Float"), ExposedPropertyColor(114, 186, 188), ExposedPropertyOrder(2)]
    public class ExposedFloatProperty : ExposedProperty
    {
        [SerializeField]
        float value;
        public float Value { get => value; set => this.value = value; }
        public override Type Type => typeof(float);

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(object value)
        {
            this.value = (float)value;
        }
    }
}
