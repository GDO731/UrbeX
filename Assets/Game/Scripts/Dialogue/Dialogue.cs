using Assets.Game.Scripts.Dialogue.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] 
        List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(DialogueConstants.NewNodeOffsetDefault, 0);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnEnable()
        {
            BuildNodeLookup();
        }

        private void OnValidate()
        {
            BuildNodeLookup();
        }

        private void BuildNodeLookup()
        {
            nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                if (node == null) continue;
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes() => nodes;

        public DialogueNode GetRootNode() => nodes.FirstOrDefault();

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childId in parentNode.GetChildren())
            {
                if(nodeLookup.ContainsKey(childId))
                {
                    yield return nodeLookup[childId];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (var node in GetAllChildren(currentNode))
            {
                if (node.GetCharacter() == Character.Player)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (var node in GetAllChildren(currentNode))
            {
                if (node.GetCharacter() != Character.Player)
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created dialogue node.");
            Undo.RecordObject(this, "Added dialogue node.");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted dialogue node.");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif


        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                foreach(var node in GetAllNodes())
                {
                    if (node == null) continue;

                    if (AssetDatabase.GetAssetPath(node) == string.Empty)
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}

