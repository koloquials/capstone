using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Usage: 

/// This is a helper script for AudioManager, which will need a dictionary of all sound effects available for the overworld puzzles
/// Assign keys (strings) and AudioClips in the inspector.
/// !IMPORTANT! When assigning things in the inspector, the AudioClips have to be put in in the same order as the strings.
/// So, for example, if Element 0 in the keys array is "WaterDrop", then Element 0 in the SoundClips array has to be the corresponding
/// WaterDrop sound effect.

/// </summary>

public class SoundFxDictionary : MonoBehaviour
{
    public string[] keys;
    public AudioClip[] soundClips;

    [SerializeField]private Dictionary<string, AudioClip> puzzleSoundsDict;

    // Start is called before the first frame update
    void Awake()
    {
        puzzleSoundsDict = new Dictionary<string, AudioClip>(); 

        for (int i = 0; i < keys.Length; i++) {
            puzzleSoundsDict.Add(keys[i], soundClips[i]);
        }
    }

    public Dictionary<string, AudioClip> GetDictionary() {
        return puzzleSoundsDict;
    }
}
