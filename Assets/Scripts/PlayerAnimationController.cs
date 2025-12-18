using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    

    public PlayerController Cont;

    private Animator _animator;

    private void Awake()
    {
        
        _animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        bool IsGrounded=Cont.IsGrounded;
        bool isMoving=(Cont.moveDirection.magnitude >= 0.1f);
        _animator.SetBool("Isgrounded",IsGrounded);
        _animator.SetBool("IsMoving",isMoving);
        if(isMoving)
        Debug.Log("ismoving");
        
    }
}
