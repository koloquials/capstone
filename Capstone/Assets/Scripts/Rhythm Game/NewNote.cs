using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

/// <summary>
/// Attach this to the rhythm game note prefab. Holds the properties of every note
/// </summary>


public class NewNote : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [HideInInspector] public int measure;
    [HideInInspector] public int beat;

    private bool moving = true;

    private int BPM = 138;

    Vector3 destinationPos;
    Vector3 startPos;

    Vector3 startScale;

    public Sprite UU;
    public Sprite UD;
    public Sprite UL;
    public Sprite UR;
    public Sprite DU;
    public Sprite DD;
    public Sprite DL;
    public Sprite DR;
    public Sprite LU;
    public Sprite LD;
    public Sprite LL;
    public Sprite LR;
    public Sprite RU;
    public Sprite RD;
    public Sprite RL;
    public Sprite RR;

    private float currTime = 0f;

    private SpriteGlowEffect glowScript;
    private TrailRenderer trail;

    private bool isVisible = false;

    public bool finishedMoving = false;                //flips to true once the note has reachd the fret. 

    [HideInInspector]public string combination;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        destinationPos = new Vector3(0f, transform.position.y, 0f);
        glowScript = gameObject.GetComponent<SpriteGlowEffect>();
        startScale = new Vector3(0f, 0f, 0f);
        trail = gameObject.GetComponent<TrailRenderer>();
        isVisible = gameObject.GetComponent<ScaleObject>().finishedScaling;
    }

    void Start() {
        SetVisibility(false);
        transform.localScale = startScale;
    }

    void Update() {
        if (isVisible) 
            SetTrail(isVisible);
    }

    public IEnumerator WaitAndMove(float delayTime)
    {
        currTime = 0f;

        // SetMaterial(true);
        SetVisibility(true);

        while (currTime < (SimpleClock.MeasureLength() * 4 - (SimpleClock.BeatLength()/2)))
        { // until one second passed
            currTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destinationPos, currTime/(SimpleClock.MeasureLength() * 4)); // lerp from A to B in currTime/(SimpleClock.MeasureLength() * 4
            glowScript.GlowBrightness = Mathf.Lerp(1f, 3.5f, currTime/(SimpleClock.MeasureLength() * 4));           //notes get brighter as they approach the fret
            yield return 1; // wait for next frame
        }
        
        //make a note invisible once it has reached its destination
        ResetNote(false, true);
    }

    //function for resetting all properties of the note that are mutable as the game is played. 
    //finishedMoving is true if the note has moved and reached the fret. Takes false when the rhythm game is trying to reset. 
    public void ResetNote(bool enabled, bool finishedMoving) {
        transform.position = startPos;
        // SetMaterial(enabled);
        SetVisibility(enabled);
        transform.localScale = startScale;
        this.finishedMoving = finishedMoving;
    }

    private void SetVisibility(bool visible) {
        spriteRenderer.enabled = visible;
    }

    private void SetTrail(bool enabled) {
        trail.enabled = enabled;
    }

    //SetSprite is a little more loaded than a setter function should be
    //takes three parameters: the combination for this note, the parentTransform (used to set start position), and the fretTransform (used to set finish position)
    public void SetSprite(string thisNotesCombo, Vector3 parentTransform, Vector3 fretTransform)
    {
        switch (thisNotesCombo)
        {//I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
            case "UU":
                spriteRenderer.sprite = UU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.32f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.32f));
                SetCombination(thisNotesCombo);
                break;
            case "UR":
                spriteRenderer.sprite = UR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.27f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.27f));
                SetCombination(thisNotesCombo);
                break;
            case "UL":
                spriteRenderer.sprite = UL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.22f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.22f));
                SetCombination(thisNotesCombo);
                break;
            case "UD":
                spriteRenderer.sprite = UD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.17f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.17f));
                SetCombination(thisNotesCombo);
                break;
            case "RU":
                spriteRenderer.sprite = RU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.12f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.12f));
                SetCombination(thisNotesCombo);
                break;
            case "RR":
                spriteRenderer.sprite = RR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.07f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.07f));
                SetCombination(thisNotesCombo);
                break;
            case "RL":
                spriteRenderer.sprite = RL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.02f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.02f));
                SetCombination(thisNotesCombo);
                break;
            case "RD":
                spriteRenderer.sprite = RD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.03f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.03f));
                SetCombination(thisNotesCombo);
                break;
            case "LU":
                spriteRenderer.sprite = LU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.08f));
                SetDestinationPos(new Vector2(1f, parentTransform.y -0.08f));
                SetCombination(thisNotesCombo);
                break;
            case "LR":
                spriteRenderer.sprite = LR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.13f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.13f));
                SetCombination(thisNotesCombo);
                break;
            case "LL":
                spriteRenderer.sprite = LL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.18f));
                SetDestinationPos(new Vector2(1f, parentTransform.y -0.18f));
                SetCombination(thisNotesCombo);
                break;
            case "LD":
                spriteRenderer.sprite = LD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.23f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.23f));
                SetCombination(thisNotesCombo);
                break;
            case "DU":
                spriteRenderer.sprite = DU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.28f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.28f));
                SetCombination(thisNotesCombo);
                break;
            case "DR":
                spriteRenderer.sprite = DR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.33f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.33f));
                SetCombination(thisNotesCombo);
                break;
            case "DL":
                spriteRenderer.sprite = DL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.38f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.38f));
                SetCombination(thisNotesCombo);
                break;
            case "DD":
                spriteRenderer.sprite = DD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.43f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.43f));
                SetCombination(thisNotesCombo);
                break;
        }
    }

    public Sprite GetSprite() {
        return spriteRenderer.sprite;
    }

    public void SetStartPos(Vector2 startPos)
    {
        this.startPos = new Vector3(startPos.x, startPos.y, 0f);
        transform.position = startPos;
    }

    public void SetDestinationPos(Vector2 destinationPos) {
        this.destinationPos = new Vector3(destinationPos.x, destinationPos.y, 0f);
    }

    //Setters for IMMUTABLE properties of each note. 
    //set this note's measure in the entire song
    public void SetMeasure(int measure)
    {
        this.measure = measure;
    }

    //set this note's beat in the measure
    public void SetBeat(int beat)
    {
        this.beat = beat;
    }

    public void SetCombination(string combination) {
        this.combination = combination;
    }

    public string GetCombination() {
        return combination;
    }

    //Debug function to check all properties of a note
    public void PrintEveryProperty() {
        Debug.Log("This note's combination is: " + GetCombination() + " my assigned measure is: " + measure + " my assigned beat is: " + beat);
    }
}