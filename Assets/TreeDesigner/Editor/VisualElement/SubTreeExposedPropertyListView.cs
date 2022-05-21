#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class SubTreeExposedPropertyListView : ExposedPropertyListView
    {
        SubTree subTree => tree as SubTree;

        protected override void OnDisplayMenuCallback(ContextualMenuPopulateEvent evt, ExposedPropertyView exposedPropertyView)
        {
            base.OnDisplayMenuCallback(evt, exposedPropertyView);
            if (subTree.OutExposedProperties.Contains(exposedPropertyView.ExposedProperty))
            {
                evt.menu.AppendAction("Remove/Out", (a) =>
                {
                    subTree.RemoveOutProperty(exposedPropertyView.ExposedProperty);
                });
            }
            else
            {
                evt.menu.AppendAction("Add/Out", (a) =>
                {
                    subTree.AddOutProperty(exposedPropertyView.ExposedProperty);
                });
            }
            if (subTree.InExposedProperties.Contains(exposedPropertyView.ExposedProperty))
            {
                evt.menu.AppendAction("Remove/In", (a) =>
                {
                    subTree.RemoveInProperty(exposedPropertyView.ExposedProperty);
                });
            }
            else
            {
                evt.menu.AppendAction("Add/In", (a) =>
                {
                    subTree.AddInProperty(exposedPropertyView.ExposedProperty);
                });
            }
        }

        public override void UpdatePropertyViews()
        {
            base.UpdatePropertyViews();
            foreach (var item in propertyViews)
            {
                if (subTree.OutExposedProperties.Contains(item.Key) || subTree.InExposedProperties.Contains(item.Key))
                    item.Value.OriginalEditorContainer.SetEnabled(false);
                else
                    item.Value.OriginalEditorContainer.SetEnabled(true);
            }
        }
    }
}

#endif