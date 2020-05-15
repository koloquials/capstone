using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HPShatter : MonoBehaviour
{
    public Animator animator;
    private AnimatorClipInfo[] animatorClipInfo;
    // public RuntimeAnimatorController animController;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    public void CallShatterAnim() {
        Debug.Log("calling the shatter coroutine");
        StartCoroutine(PlayShatterAnim());
    }

    public IEnumerator PlayShatterAnim() {
        animator.Play("Slice_Split", -1, 0f);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        this.gameObject.SetActive(false);
    }
}