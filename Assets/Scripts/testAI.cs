using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAI : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("HeavyAttack");
        }
        else
        {
            Debug.LogError("Không tìm thấy Animator trên GameObject này, cha nội!");
        }
    }

    void Update()
    {

    }
}
