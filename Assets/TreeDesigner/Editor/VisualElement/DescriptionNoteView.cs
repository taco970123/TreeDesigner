#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeDesigner.Runtime;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace TreeDesigner.Editor
{
    public class DescriptionNoteView : UnityEditor.Experimental.GraphView.StickyNote
    {
        public DescriptionNote note;

        Label titleLabel;
        ColorField colorField;

        public DescriptionNoteView()
        {
            fontSize = StickyNoteFontSize.Small;
            theme = StickyNoteTheme.Classic;

            style.backgroundColor = Color.gray;
            this.Q<Label>("title").style.color = Color.black;
            TextField titleField = this.Q<TextField>("title-field");
            titleField.style.backgroundColor = Color.gray;
            titleField.Q("unity-text-input").style.backgroundColor = Color.gray;
            this.Q<Label>("contents").style.color = Color.black;
            TextField contentsField = this.Q<TextField>("contents-field");
            contentsField.style.backgroundColor = Color.gray;
            contentsField.Q("unity-text-input").style.backgroundColor = Color.gray;
        }

        public void Initialize(DescriptionNote note)
        {
            this.note = note;

            this.Q<TextField>("title-field").RegisterCallback<ChangeEvent<string>>(e => {
                note.title = e.newValue;
            });
            this.Q<TextField>("contents-field").RegisterCallback<ChangeEvent<string>>(e => {
                note.content = e.newValue;
            });

            title = note.title;
            contents = note.content;
            SetPosition(note.Position);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            if (!note) return;
            Undo.RecordObject(note, "Note (Set Position)");
            note.Position = newPos;
            EditorUtility.SetDirty(note);
        }

        public override void OnResized()
        {
            note.Position = layout;
        }
    }
}
#endif