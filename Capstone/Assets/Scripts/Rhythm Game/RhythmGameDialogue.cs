using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RhythmGameDialogue : MonoBehaviour
{
    // public List<string> dialogue;
    public string[] dialogue;
    public TextAsset dialogueFile;

    public Text showDialogue; 

    void Start() {
        Parse();
    }

    private void Parse() {
        string text = dialogueFile.text;

        dialogue = text.Split('\n');
    }

    public void ShowDialogue(int index) {
        showDialogue.text = dialogue[index];
    }
}
