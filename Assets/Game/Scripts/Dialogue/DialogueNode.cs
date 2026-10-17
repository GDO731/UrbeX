using Assets.Game.Scripts.Dialogue.Enums;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    public class DialogueNode: ScriptableObject
    {
        [SerializeField] Character character;
        [SerializeField] string text;
        [SerializeField] AudioClip audioClip;
        [SerializeField] Animation animation;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, DialogueConstants.DefaultNodeWidth, DialogueConstants.DefaultNodeHeight);
        [SerializeField] string onEnterAction;
        [SerializeField] string onExitAction;
        [SerializeField] float delay = 0f;
        public Rect GetRect() => rect;
        public string GetText() => text;
        public AudioClip GetAudioClip() => audioClip;
        public Animation GetAnimation() => animation;
        public List<string> GetChildren() => children;
        public Character GetCharacter() => character;
        public string GetOnEnterAction() => onEnterAction;
        public string GetOnExitAction() => onExitAction;
        public float GetDelay() => delay;


#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move dialoge node.");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if(newText != text)
            {
                Undo.RecordObject(this, "Update dialogue text.");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetAudioClip(AudioClip newAudioClip)
        {
            if(newAudioClip != audioClip)
            {
                Undo.RecordObject(this, "Update dialogue audio.");
                audioClip = newAudioClip;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetAnimation(Animation newAnimation)
        {
            if (newAnimation != animation)
            {
                Undo.RecordObject(this, "Update dialogue animation.");
                animation = newAnimation;
                EditorUtility.SetDirty(this);
            }
        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove dialogue link.");
            children.Remove(childId);
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add dialogue link.");
            children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void SetCharacer(Character character)
        {
            Undo.RecordObject(this, "Change speaker.");
            this.character = character;
            EditorUtility.SetDirty(this);
        }

        public void SetDelay(float newDelay)
        {
            if (newDelay != delay)
            {
                Undo.RecordObject(this, "Change dialogue delay.");
                delay = newDelay;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}