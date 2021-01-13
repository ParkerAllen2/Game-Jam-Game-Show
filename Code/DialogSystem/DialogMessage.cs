using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogMessage : ScriptableObject
{
    public Sprite nameTag;

    [TextArea(2, 10)]
    public string[] sentences;

    public bool endDialog;

    public DialogueOption[] dialogueOptions;

    [System.Serializable]
    public struct DialogueOption
    {
        public string text;
        public DialogMessage nextDialogue;
    }
}
