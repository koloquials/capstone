using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFretFeedback : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    bool doneScaling = false;
    string[] phase1Sequence;
    string[] phase2Sequence;

    string[] songSequence;

    bool phase1Over;

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

    public ParticleSystem rippleParticleSystem;

    private int currPosInSong = 1;

    public int phase1Threshold = 0;

    public ScaleObject objectScalerScript;

    private Vector3 originalScale;

    void Start()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        objectScalerScript = gameObject.GetComponent<ScaleObject>();
        rippleParticleSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
    }

    void Update() {
        doneScaling = objectScalerScript.finishedScaling;
    }

    public void ScaleFret(float time, Vector3 scaleToSize) {
        StartCoroutine(objectScalerScript.Scale(time, scaleToSize));
    }

    public void SetFret(string nextSpriteCombination)
    {
        SetSprite(nextSpriteCombination);
    }

    public bool GetScaleStatus()
    {
        return doneScaling;
    }

    public void ResetFret() {
        mySpriteRenderer.color = Color.white;
        doneScaling = false;
    }

    public void SetSprite(string currentSpriteCombo)
    {
        switch (currentSpriteCombo)
        {
            case "UU":
                mySpriteRenderer.sprite = UU;
                break;
            case "UR":
                mySpriteRenderer.sprite = UR;
                break;
            case "UL":
                mySpriteRenderer.sprite = UL;
                break;
            case "UD":
                mySpriteRenderer.sprite = UD;
                break;
            case "RU":
                mySpriteRenderer.sprite = RU;
                break;
            case "RR":
                mySpriteRenderer.sprite = RR;
                break;
            case "RL":
                mySpriteRenderer.sprite = RL;
                break;
            case "RD":
                mySpriteRenderer.sprite = RD;
                break;
            case "LU":
                mySpriteRenderer.sprite = LU;
                break;
            case "LR":
                mySpriteRenderer.sprite = LR;
                break;
            case "LL":
                mySpriteRenderer.sprite = LL;
                break;
            case "LD":
                mySpriteRenderer.sprite = LD;
                break;
            case "DU":
                mySpriteRenderer.sprite = DU;
                break;
            case "DR":
                mySpriteRenderer.sprite = DR;
                break;
            case "DL":
                mySpriteRenderer.sprite = DL;
                break;
            case "DD":
                mySpriteRenderer.sprite = DD;
                break;
        }
    }

    public IEnumerator FretHit(bool hit) //Whenever the fret gets hit. Pass a bool for if the hit was accurate
    {
        if (hit) { //If it hits, set the color and increase the scale 
            originalScale = transform.localScale;
            mySpriteRenderer.color = new Color(0.26667f, 0.49020f, 0.85490f, 0.5f); //May change this and the miss color once our palette is finalized.
            StartCoroutine(objectScalerScript.Scale(0.1f, new Vector3(2.2f, 2.2f, transform.position.z)));

            yield return new WaitForSeconds(0.1f);

            mySpriteRenderer.color = Color.white;
            StartCoroutine(objectScalerScript.Scale(0.1f, originalScale)); 
        }
        else  {//On a miss, set the color and decrease the scale
            mySpriteRenderer.color = new Color(0.96078f, 0.43922f, 0.53333f, 0.8f);
        }
    }
}
