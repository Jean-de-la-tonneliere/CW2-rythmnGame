using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [SerializeField]
    private bool blockEnable;
    private bool gotInput = false;
    private bool isBlocking;

    [SerializeField]
    private float inputTimer;
    private float lastInputTime = Mathf.NegativeInfinity;

    [SerializeField]
    private Transform blockBoxPos;
    [SerializeField]
    private Vector2 blockBoxSize;
    [SerializeField]
    private LayerMask whatIsBlockable;

    private void Update()
    {
        checkBlockInput();
        CheckBlock();
    }

    private void checkBlockInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (blockEnable)
            {
                //Attempt block
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckBlock()
    {
        if(gotInput)
        {
            //perform block
            if(!isBlocking)
            {
                gotInput = false;
                isBlocking = true;
                PlayerAfterImagePool.Instance.GetFromPool();
                CheckBlockHitBox();
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            //Wait for new input
            gotInput = false;
            isBlocking = false;
        }
    }

    private void CheckBlockHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapBoxAll(blockBoxPos.position, blockBoxSize, 0f, whatIsBlockable);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Blocked");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(blockBoxPos.position, blockBoxSize);
    }
}
