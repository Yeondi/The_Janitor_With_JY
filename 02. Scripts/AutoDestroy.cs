using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    private Animator animator;
    private float animationLength;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing from this GameObject.");
            return;
        }

        // Get the current animation clip information
        AnimatorStateInfo animationInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationLength = animationInfo.length;

        // Destroy the GameObject after the animation ends
        Destroy(gameObject, animationLength);
    }
}

