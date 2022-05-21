using System;
using System.Collections;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [ExposedPropertyName("List"), ExposedPropertyColor(200, 200, 200), ExposedPropertyOrder(4)]
    public class ExposedListProperty : ExposedProperty
    {
        [SerializeField]
        IList value;
        public IList Value { get => value; set => this.value = value; }
        public override Type Type => typeof(IList);

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(object value)
        {
            this.value = (IList)value;
        }
    }
}
