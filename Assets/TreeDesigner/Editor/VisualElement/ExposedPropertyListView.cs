#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class ExposedPropertyListView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ExposedPropertyListView, UxmlTraits> { }

        static string propertyName;

        protected bool canInteract;
        protected BaseTree tree;
        protected DropArea dropArea;
        protected EnumField propertyTypeField;
        protected TextField propertyNameField;
        protected ScrollView propertyContent;
        protected VisualElement insertLine;
        protected List<ExposedProperty> properties;
        protected Dictionary<ExposedProperty, ExposedPropertyView> propertyViews;

        public ExposedPropertyListView()
        {
            propertyViews = new Dictionary<ExposedProperty, ExposedPropertyView>();

            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("UIBuilder/ExposedPropertyListView");
            template.CloneTree(this);
            propertyContent = this.Q<ScrollView>("property-content");
            insertLine = this.Q("insert-line");

            propertyNameField = this.Q<TextField>("property-name");
            propertyNameField.RegisterValueChangedCallback((i) => propertyName = i.newValue.ToString());

            var propertyAddBtn = this.Q("property-add-btn");
            propertyAddBtn.AddManipulator(new CustomContextualMenuManipulator((ContextualMenuPopulateEvent evt) => DisplayAddMenu(evt)));

            dropArea?.Dispose();
            dropArea = new DropArea();
            dropArea.Init(this);
            dropArea.onDragUpdatedEvent += (e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                object draggedObject = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
                if (draggedObject != null && draggedObject is ExposedPropertyView exposedPropertyView)
                {
                    if (!propertyViews.ContainsValue(exposedPropertyView))
                        return;
                    float y = e.localMousePosition.y;
                    Rect selfRect = this.WorldToLocal(exposedPropertyView.worldBound);
                    if (y < selfRect.yMin || y > selfRect.yMax)
                    {
                        propertyContent.Remove(insertLine);
                        int index = propertyContent.childCount;
                        foreach (var item in propertyViews)
                        {
                            Rect targetRect = this.WorldToLocal(item.Value.worldBound);
                            if (y < targetRect.yMin)
                            {
                                int targetIndex = propertyContent.IndexOf(item.Value);
                                if (index > targetIndex)
                                    index = targetIndex;
                            }
                        }
                        insertLine.style.display = DisplayStyle.Flex;
                        propertyContent.Insert(index, insertLine);
                    }
                    else
                        insertLine.style.display = DisplayStyle.None;
                }
            };
            dropArea.onDragLeaveEvent += (e) =>
            {
                insertLine.style.display = DisplayStyle.None;
            };
            dropArea.onDragPerformEvent += (e) =>
            {
                insertLine.style.display = DisplayStyle.None;
                object draggedObject = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
                if (draggedObject != null && draggedObject is ExposedPropertyView exposedPropertyView)
                {
                    if (!propertyViews.ContainsValue(exposedPropertyView))
                        return;
                    float y = e.localMousePosition.y;
                    Rect selfRect = this.WorldToLocal(exposedPropertyView.worldBound);
                    exposedPropertyView.style.position = Position.Relative;
                    if (y < selfRect.yMin || y > selfRect.yMax)
                    {
                        int originalIndex = properties.IndexOf(exposedPropertyView.ExposedProperty);
                        int index = properties.Count;
                        foreach (var item in propertyViews)
                        {
                            Rect targetRect = this.WorldToLocal(item.Value.worldBound);
                            if (y < targetRect.yMin)
                            {
                                int targetIndex = properties.IndexOf(item.Key);
                                if (index > targetIndex)
                                    index = targetIndex;
                            }
                        }
                        if (index > originalIndex)
                            index--;
                        tree.MoveProperty(index, exposedPropertyView.ExposedProperty);
                    }
                }
            };
            dropArea.onDragExitEvent += (e) =>
            {
                object draggedObject = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
                if (draggedObject != null && draggedObject is ExposedPropertyView exposedPropertyView)
                {
                    exposedPropertyView.RemoveFromClassList("dragged");
                }
            };
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="exposedProperties"></param>
        public void Init(BaseTree tree, bool canInteract)
        {
            if (this.tree)
                this.tree.onExposedPropertyChangedEvent -= UpdatePropertyViews;
            this.tree = tree;
            this.canInteract = canInteract;
            properties = tree.exposedProperties;
            ExposedProperty.UpdateAllProperties(properties);
            UpdatePropertyViews();
            tree.onExposedPropertyChangedEvent += UpdatePropertyViews;
        }
        /// <summary>
        /// 施放
        /// </summary>
        public void Dispose()
        {
            if (tree)
                tree.onExposedPropertyChangedEvent -= UpdatePropertyViews;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }
        /// <summary>
        /// 添加Property
        /// </summary>
        public void AddProperty(ExposedProperty exposedProperty)
        {
            if (Application.isPlaying || tree == null)
                return;
            if (propertyName == string.Empty)
                propertyName = "New Exposed Property";
            while (properties.Find(i => i.Name == propertyName))
            {
                propertyName += "(1)";
            }
            exposedProperty.name = exposedProperty.GetType().Name;
            exposedProperty.Name = propertyName;

            tree.AddProperty(exposedProperty, propertyName);
            ExposedProperty.UpdateAllProperties(properties);
        }
        /// <summary>
        /// 删除Property
        /// </summary>
        /// <param name="exposedProperty"></param>
        public void RemoveProperty(ExposedProperty exposedProperty)
        {
            if (Application.isPlaying || tree == null)
                return;
            if (exposedProperty == null)
                return;
            tree.RemoveProperty(exposedProperty);
            ExposedProperty.UpdateAllProperties(properties);
        }
        /// <summary>
        /// 刷新PropertyViews
        /// </summary>
        public virtual void UpdatePropertyViews()
        {
            propertyViews.Clear();
            int index = 0;
            for (int i = 0; i < propertyContent.childCount; i++)
            {
                if (propertyContent[i] is ExposedPropertyView exposedPropertyView)
                {
                    if (index < properties.Count)
                    {
                        exposedPropertyView.Init(properties[index], canInteract);
                        propertyViews.Add(properties[index], exposedPropertyView);
                        index++;
                    }
                    else
                    {
                        propertyContent.RemoveAt(i);
                    }
                }
            }
            for (int i = index; i < properties.Count; i++)
            {
                AddPropertyView(properties[i]);
            }
            for (int i = propertyContent.childCount - 1; i >= 0; i--)
            {
                if (propertyContent[i] is ExposedPropertyView exposedPropertyView && !properties.Contains(exposedPropertyView.ExposedProperty))
                    propertyContent.RemoveAt(i);
            }
        }
        /// <summary>
        /// 清空PropertyViews
        /// </summary>
        public void ClearView()
        {
            tree = null;
            foreach (var item in propertyViews)
            {
                propertyContent.Remove(item.Value);
            }
            propertyViews.Clear();
        }
        /// <summary>
        /// 添加PropertyView
        /// </summary>
        /// <param name="exposedProperty"></param>
        protected void AddPropertyView(ExposedProperty exposedProperty)
        {
            if (exposedProperty == null)
                return;
            propertyName = string.Empty;
            propertyNameField.SetValueWithoutNotify(propertyName);
            ExposedPropertyView exposedPropertyView = new ExposedPropertyView();
            exposedPropertyView.Init(exposedProperty, canInteract);
            exposedPropertyView.onRemovedEvent += RemoveProperty;
            exposedPropertyView.onDisplayMenuEvent += (e) => OnDisplayMenuCallback(e, exposedPropertyView);
            propertyContent.Add(exposedPropertyView);
            propertyViews.Add(exposedProperty, exposedPropertyView);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        protected void OnUndoRedo()
        {
            if (tree == null)
                return;
            UpdatePropertyViews();
            ExposedProperty.UpdateAllProperties(properties);
            AssetDatabase.SaveAssets();
        }
        
        protected virtual void OnDisplayMenuCallback(ContextualMenuPopulateEvent evt, ExposedPropertyView exposedPropertyView)
        {

        }

        protected virtual void DisplayAddMenu(ContextualMenuPopulateEvent evt)
        {
            List<Type> types = TypeCache.GetTypesDerivedFrom<ExposedProperty>().ToList();

            int totalIndex = 0;
            types = types.OrderBy(i =>
            {
                int index = totalIndex;
                if (i.GetCustomAttributes(typeof(ExposedPropertyOrder), false).Length > 0)
                    index = (i.GetCustomAttributes(typeof(ExposedPropertyOrder), false)[0] as ExposedPropertyOrder).Index;
                else
                    index++;
                totalIndex++;
                return index;
            }).ToList();

            foreach (var item in types)
            {
                evt.menu.AppendAction(item.Name, (a) =>
                {
                    ExposedProperty exposedProperty = ScriptableObject.CreateInstance(item.FullName) as ExposedProperty;
                    AddProperty(exposedProperty);
                });
            }
        }
    }
}
#endif