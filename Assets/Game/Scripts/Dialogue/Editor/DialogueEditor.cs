using Assets.Game.Scripts.Core;
using Assets.Game.Scripts.Dialogue.Enums;
using Assets.Game.Scripts.Dialogue.Enums.Attributes;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        Vector2 scrollPosition;

        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEdittorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(DialogueConstants.AssetAttributeIndex)]
        public static bool OnOpenAsset(int instanceId, int late)
        {
            var dialogue = EditorUtility.EntityIdToObject(instanceId) as Dialogue;
            if (dialogue != null)
            {
                ShowEdittorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
             
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;

            nodeStyle.padding = new RectOffset(
                DialogueConstants.RectOffsetPaddingValue, 
                DialogueConstants.RectOffsetPaddingValue,
                DialogueConstants.RectOffsetPaddingValue,
                DialogueConstants.RectOffsetPaddingValue);

            nodeStyle.border = new RectOffset(
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue
                );

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;

            playerNodeStyle.padding = new RectOffset(
                DialogueConstants.RectOffsetPaddingValue,
                DialogueConstants.RectOffsetPaddingValue,
                DialogueConstants.RectOffsetPaddingValue,
                DialogueConstants.RectOffsetPaddingValue);

            playerNodeStyle.border = new RectOffset(
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue,
                DialogueConstants.RectOffsetBorderValue
                );
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                var canvas =  GUILayoutUtility.GetRect(DialogueConstants.CanvasSize, DialogueConstants.CanvasSize);
                var backgroundTexture = Resources.Load(DialogueConstants.CanvasBackground) as Texture2D;
                var texCoords = new Rect(0, 0,
                    DialogueConstants.CanvasSize / DialogueConstants.BackgroundSize,
                    DialogueConstants.CanvasSize / DialogueConstants.BackgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, texCoords);

                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawConnection(node);
                }
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;  
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if(Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            var style = nodeStyle;
            if(node.GetCharacter() == Character.Player)
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);

            var newText = EditorGUILayout.TextField(node.GetText());
            var newAudioClip = (AudioClip)EditorGUILayout.ObjectField(node.GetAudioClip(), typeof(AudioClip), false);
            var newAnimation = (Animation)EditorGUILayout.ObjectField(node.GetAnimation(), typeof(Animation), false);

            node.SetText(newText);
            node.SetAudioClip(newAudioClip);
            node.SetAnimation(newAnimation);

            GUILayout.BeginHorizontal();
            CreateButtons(node);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnection(DialogueNode node)
        {
            var startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                var endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);

                var controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= DialogueConstants.ControlPointOffsetXMultiplier;

                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlPointOffset, endPosition - controlPointOffset,
                    Color.white, null, DialogueConstants.BezierCurveWidth);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void CreateButtons(DialogueNode node)
        {
            if (GUILayout.Button(DialogueConstants.AddButtonText))
            {
                creatingNode = node;
            }
            if (linkingParentNode == null)
            {
                if (GUILayout.Button(DialogueConstants.LinkButtonText))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button(DialogueConstants.CancelButtonText))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button(DialogueConstants.UnlinkButtonText))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button(DialogueConstants.ChildButtonText))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
            if (GUILayout.Button(DialogueConstants.DeleteButtonText))
            {
                deletingNode = node;
            }
        }
    }
}
