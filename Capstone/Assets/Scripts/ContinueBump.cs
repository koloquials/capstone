using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueBump : MonoBehaviour //Put this on the continue prompt
{
    float bumpScale = 1f;

    bool apex = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!apex)
        {
            if(bumpScale < 1.15f)
            {
                bumpScale += 6*Time.deltaTime;
            }
            else
            {
                apex = true;
            }
        }
        else
        {
            if(bumpScale > 1f)
            {
                bumpScale -= 6*Time.deltaTime;
            }
            else
            {
                bumpScale = 1f;
            }
        }
        transform.localScale = new Vector3(bumpScale, bumpScale, 1);
    }

    public void bump()
    {
        apex = false;
        bumpScale = 0.85f;
        transform.localScale = new Vector3(bumpScale, bumpScale, 1);
    }
}
