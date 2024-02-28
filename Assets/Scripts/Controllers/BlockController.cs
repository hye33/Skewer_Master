using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BlockController : MonoBehaviour
{
    UI_GamePopup gameUI;

    Vector2 minBounds;
    Vector2 maxBounds;
    GameObject nextBlock;
    GameObject topBlock;
    Sprite[] sprites;

    int themeidx = 0;
    string themeName;
    int THEME_BLOCK_COUNT;

    Vector2 InitPos;
    int dir = 1;
    float moveSpeed = 6.5f;
    float dropSpeed = 12.0f;
    float sortSpeed = 5.0f;

    public bool isDrop;

    Vector2 centerPoint;
    float goodBound = 0.6f;
    float perfectBound = 0.3f;

    float sortBound = -2.0f;
    private bool needSort;
    private bool isSorting;
    public Action SortAction; // 정렬 끝난 후 block destory 확인 위함

    GameObject firstBlock;
    Vector3 firstBlockPos = new Vector3(0, -5.6f, 0);

    float gameOverTime = 1.2f;

    Coroutine co;
    Coroutine sortCo;

    void InitBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        centerPoint = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        InitPos = mainCamera.ViewportToWorldPoint(new Vector2(0, 0.8f));
    }

    void Init()
    {
        gameUI = FindObjectOfType<UI_GamePopup>().GetComponent<UI_GamePopup>();
        gameUI.TouchPanel -= TouchPanel;
        gameUI.TouchPanel += TouchPanel;

        themeidx = Managers.Game.SelectTheme;
        themeName = Enum.GetName(typeof(ThemeName), themeidx);
        sprites = Resources.LoadAll<Sprite>($"Images/Blocks/{themeName}");
        THEME_BLOCK_COUNT = sprites.Length;
    }

    void Start()
    {
        InitBounds();
        Init();
        SetFirstBlock();
        MadeNextBlock();
    }

    private void SetFirstBlock()
    {
        firstBlock = Managers.Resource.Instantiate("Items/Block", gameObject.transform);
        firstBlock.transform.position = firstBlockPos;
        firstBlock.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
    public void MadeNextBlock()
    {
        int nextIdx = UnityEngine.Random.Range(0, THEME_BLOCK_COUNT);
        nextBlock = Managers.Resource.Instantiate("Items/Block", gameObject.transform);
        nextBlock.GetComponent<SpriteRenderer>().sprite = sprites[nextIdx];
        nextBlock.transform.position = InitPos;

        nextBlock.GetComponent<Block>().DropAction -= SetNextBlock;
        nextBlock.GetComponent<Block>().DropAction += SetNextBlock;

        co = StartCoroutine(CoMoveNextBlock());
    }

    #region BlockMove
    private IEnumerator CoMoveNextBlock()
    {
        while (true)
        {
            if (nextBlock.transform.position.x < minBounds.x)
                dir = 1;
            else if (nextBlock.transform.position.x > maxBounds.x)
                dir = -1;

            Vector2 newPos;
            newPos.x = nextBlock.transform.position.x + (dir * moveSpeed * Time.deltaTime);
            newPos.y = InitPos.y;
            nextBlock.transform.position = newPos;

            yield return null;
        }
    }

    public void TouchPanel()
    {
        StopCoroutine(co);
        CheckBlockCoor();
        co = StartCoroutine(CoDropNextBlock());

        // 공중에서 블록끼리 부딪혀서 생기는 에러 안정성 강화 작업
        nextBlock.GetComponent<Block>().canSet = true;
    }

    private IEnumerator CoDropNextBlock()
    {
        while (true)
        {
            Vector2 newPos;
            newPos.x = nextBlock.transform.position.x;
            newPos.y = nextBlock.transform.position.y - (dropSpeed * Time.deltaTime);
            nextBlock.transform.position = newPos;

            yield return null;
        }
    }

    public void SetNextBlock()
    {
        StopCoroutine(co); // co : CoDropNextBlock

        topBlock = nextBlock;

        CheckTopCoor();
        MadeNextBlock();
    }

    private IEnumerator CoSortBlocks()
    {
        isSorting = true;
        while (needSort)
        {
            foreach (Transform child in transform)
            {
                Vector2 newPos;
                newPos.x = child.position.x;
                newPos.y = child.position.y - (sortSpeed * Time.deltaTime);
                child.position = newPos;
            }
            CheckTopCoor();
            yield return null;
        }
        SortAction.Invoke();
        isSorting = false;
    }
    #endregion

    private void CheckTopCoor()
    {
        if (topBlock.transform.position.y < sortBound)
        {
            if (needSort == true && isSorting == false)
                StopCoroutine(sortCo);
            needSort = false;
        }
        else
        {
            needSort = true;
            if (isSorting == false)
                sortCo = StartCoroutine(CoSortBlocks());
        }
    }

    public void CheckBlockCoor()
    {
        float x = nextBlock.transform.position.x;

        if (x > (centerPoint.x - goodBound) && x < (centerPoint.x + goodBound))
        {
            if (x > (centerPoint.x - perfectBound) && x < (centerPoint.x + perfectBound))
                gameUI.SetPerfectBound();
            else
                gameUI.SetGoodBound();
        }
        else
        {
            nextBlock.GetComponent<BoxCollider2D>().enabled = false;
            Invoke("SetFail", gameOverTime);          
        }

    }

    private void SetFail()
    {
        StopCoroutine(co); // co : CoDropNextBlock
        nextBlock.GetComponent<Block>().CheckAndDestory();
        gameUI.SetFail();
    }
}
