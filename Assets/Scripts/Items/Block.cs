using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool canSet;
    public Action DropAction = null;
    float destroyBound = -6.0f;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        transform.GetComponentInParent<BlockController>().SortAction -= CheckAndDestory;
        transform.GetComponentInParent<BlockController>().SortAction += CheckAndDestory;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canSet == false)
            return;

        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        DropAction?.Invoke();
        DropAction = null;
    }

    public void CheckAndDestory()
    {
        if (transform.position.y < destroyBound)
        {
            transform.GetComponentInParent<BlockController>().SortAction -= CheckAndDestory;
            Destroy(gameObject);
        }
    }
}
