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

    void OnEnable() {
        showDialogue.text = "";
    }

    void Start() {
        Parse();
    }

    //Note on setting up the file to parse: 
    //ShowDialogue() gets called every time we exit a window in the rhythm game, so if you want a specific line to stay up
    //for multiple notes (that the player hits, not notes in the song), then put an empty, new line between spoken lines!! 
    private void Parse() {
        string text = dialogueFile.text;

        dialogue = text.Split('\n');

        foreach(string line in dialogue) {
            Debug.Log(line);
        }
    }

    //gets the length of dialogue[] so we don't get an OutOfBounds exception
    public int GetLineCount() {
        return dialogue.Length;
    }

    public void ShowDialogue(int index) {
        //only change the dialogue when the line actually had dialogue, not just empty
        if (!dialogue[index].Equals("")) 
            showDialogue.text = dialogue[index];
    }

    public void ClearDialogue()
    {
        showDialogue.text = "";
    }
}
