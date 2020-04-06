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

    void Start()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // rippleParticleSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
    }

    public IEnumerator ScaleFret(float time, Vector3 scaleToSize)
    {
        //Debug.Log("Coroutine: trying to scale the fret");
        Vector3 originalScale = gameObject.transform.localScale;
        // Vector3 destinationScale = new Vector3(2.0f, 2.0f, 2.0f);
        Vector3 destinationScale = scaleToSize;

        //Color spriteColour = mySpriteRenderer.color;

        float currTime = 0f;

        do
        {
            //Debug.Log("Scaling the object up");
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            //spriteColour.a = 1 - (currTime / time);
            //mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);

        // Debug.Log("This coroutine has finished executing. Thx for ur business :) ");
        doneScaling = true;
    }

    public void SetFret(string nextSpriteCombination)
    {
        if (currPosInSong > phase1Threshold)
        {
            phase1Over = true;
        }

        // SetSprite(songSequence[currPosInSong]);
        SetSprite(nextSpriteCombination);

        currPosInSong++;
    }

    // public void RippleEffect() {
    //     rippleParticleSystem.Play();
    // }

    // public IEnumerator SetFret()
    // {
    //     for (int i = 0; i < songSequence.Length; i++)
    //     {
    //         if (i > phase1Sequence.Length)
    //         {
    //             phase1Over = true;
    //         }

    //         SetSprite(songSequence[i]);

    //         yield return new WaitForSeconds(SimpleClock.MeasureLength());
    //         Debug.Log("Changing to the next note");
    //     }
    // }



    public bool GetScaleStatus()
    {
        return doneScaling;
    }

    public bool GetPhase1Status()
    {
        return phase1Over;
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
}
