using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroupControl : MonoBehaviour {
    /// <summary>
    /// 声音源
    /// </summary>
    public AudioSource blockAudio;

    /// <summary>
    /// 方块落地事件
    /// </summary>
    public delegate void OnBlockLand();
    public event OnBlockLand LandEventHandler;

    /// <summary>
    /// 获取方格索引管理
    /// </summary>
    MyGridManager myGridManager;

    // Use this for initialization
    void Start() {
        blockAudio = GameObject.FindObjectOfType<AudioSource>().GetComponent<AudioSource>();
        myGridManager = GameObject.FindObjectOfType<MyGridManager>();

#if UNITY_EDITOR
        if (blockAudio == null) print("未找到播放音频！");
        if (myGridManager == null) print("未找到管理组件！");
#endif

        //为事件添加触发
        LandEventHandler += BlockLand;
        LandEventHandler += myGridManager.CheckBlockRow;

        StartCoroutine(MoveDownward(1.2f));    
    }

    /// <summary>
    /// 方块落地触发效果
    /// </summary>
    public void BlockLand() {
        //关闭协程
        StopAllCoroutines();
        //关闭方块组的控制组件
        this.GetComponent<BlockGroupControl>().enabled = false;

        //将子物体移到堆积方块框架下
        Transform newParent = myGridManager.transform;
        foreach (var son in transform.GetComponentsInChildren<MeshFilter>()) {
            //将方块对应位置的索引值置为1
            Vector2 index = MyGridManager.TransformConvertToIndex(son.transform);
            int x = (int)index.x;
            int y = (int)index.y;
            try {
                myGridManager.myGrid[x, y] = 1;
                myGridManager.gridBlocks[y].Add(son.transform);
                son.transform.SetParent(newParent, true);
            } catch (Exception e) {
                print(e);
            }
        }
        Destroy(this.gameObject);
    }


    /// <summary>
    /// 是否在加速下落
    /// </summary>
    bool isKeyDowning = false;
    /// <summary>
    /// 按键左右是否可用
    /// </summary>
    bool keyAorDUp = true;
    /// <summary>
    /// 按键上是否可用
    /// </summary>
    bool keyWUp = true;

    private void Update() {
        if (Input.GetKey(KeyCode.A) && keyAorDUp) {
            StartCoroutine(GetKeyAOrDDown(new Vector3(1, 0, 0)));
        }
        if (Input.GetKey(KeyCode.D) && keyAorDUp) {
            StartCoroutine(GetKeyAOrDDown(new Vector3(-1, 0, 0)));
        }
        if (Input.GetKey(KeyCode.W) && keyWUp) {
            StartCoroutine(GetKeyWDown());
        }
        if (Input.GetKey(KeyCode.S) && !isKeyDowning) {
            StartCoroutine(MoveFastDownward(0.02f));
        }
    }

    /// <summary>
    /// 按键左右的响应事件
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    IEnumerator GetKeyAOrDDown(Vector3 dir) {
        transform.position -= dir;
        keyAorDUp = false;
        if (myGridManager.NextHasBlock(transform) || !myGridManager.GroupIsInBottomLeftRightBoard(transform))   // 下一位置被占用或在边界外
            transform.position += dir;

        yield return new WaitForSeconds(.2f);
        keyAorDUp = true;
    }

    /// <summary>
    /// 按键上的响应事件，变换形状
    /// </summary>
    /// <returns></returns>
    IEnumerator GetKeyWDown() {
        if (myGridManager.lastIndex == 5) yield return new WaitForEndOfFrame();
        else {
            if (!isKeyDowning) {
                transform.localRotation *= Quaternion.Euler(0, 0, 90f);
                keyWUp = false;
                if (myGridManager.NextHasBlock(transform) || !myGridManager.GroupIsInBottomLeftRightBoard(transform)) {
                    transform.localRotation *= Quaternion.Euler(0, 0, -90f);
                }
            }
            yield return new WaitForSeconds(.4f);
            keyWUp = true;
        }
    }

    /// <summary>
    /// 方块下落
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator MoveDownward(float t) {
        transform.position -= new Vector3(0, 1, 0);
        if (myGridManager.NextHasBlock(transform) || !myGridManager.GroupIsInBottomLeftRightBoard(transform)) { // 下方位置被占用 或者 不在区域内
            transform.position += new Vector3(0, 1, 0);
            StopAllCoroutines();
            if (LandEventHandler != null) LandEventHandler();
        }
        else {
            yield return new WaitForSeconds(t);
            StartCoroutine(MoveDownward(t));
        }

    }
    /// <summary>
    /// 方块加速下落
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator MoveFastDownward(float t) {
        //StopAllCoroutines();
        isKeyDowning = true;
        transform.position -= new Vector3(0, 1, 0);
        if (myGridManager.NextHasBlock(transform) || !myGridManager.GroupIsInBottomLeftRightBoard(transform)) {
            transform.position += new Vector3(0, 1, 0);
            isKeyDowning = false;
            StopAllCoroutines();
            if (LandEventHandler != null) LandEventHandler();
        }
        yield return new WaitForSeconds(t);
        isKeyDowning = false;
    }
}
    