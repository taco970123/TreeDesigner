using System;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [ExposedPropertyName("Int"), ExposedPropertyColor(90, 81, 131), ExposedPropertyOrder(1)]
    public class ExposedIntProperty : ExposedProperty
    {
        [SerializeField]
        int value;
        public int Value { get => value; set => this.value = value; }
        public override Type Type => typeof(int);

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(object value)
        {
            this.value = (int)value;
        }
    }
}
