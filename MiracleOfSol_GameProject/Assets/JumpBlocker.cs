using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class JumpBlocker : MonoBehaviour
{
    [SerializeField] private Collider BlockingCollider;

    private void OnEnable()
    {

        if(BlockingCollider == null) 
        { 
            TryToGetCollider(); 
        }

        Actions.OnRegisterJumpBlocker.InvokeAction(BlockingCollider);
    }

    private void OnDisable()
    {
        Actions.OnDeRegisterJumpBlocker.InvokeAction(BlockingCollider);
    }

    private void TryToGetCollider()
    {
        print("Jump Blocker of name " + gameObject.name + " doesn't have its collider assigned.");

        TryGetComponent(out BlockingCollider);

        if(BlockingCollider == null)
        {

            print("Jump Blocker of name " + gameObject.name + " doesn't have its collider assigned and cannot find a new one. Destroying.");
            Destroy(gameObject);

        }
    }

}
