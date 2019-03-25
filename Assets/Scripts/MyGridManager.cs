using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGridManager : MonoBehaviour {
    /// <summary>
    /// 存储全局方格占有的索引
    /// </summary>
    public int[,] myGrid;
    /// <summary>
    /// 按行存储新加入的方块组
    /// </summary>
    public Dictionary<int,List<Transform>> gridBlocks;
    /// <summary>
    /// 存储方格预设体
    /// </summary>
    public GameObject[] TGroup;
    /// <summary>
    /// 上一个方格类型索引值
    /// </summary>
    public int lastIndex;
    /// <summary>
    /// 下一个即将产生的方格类型索引值
    /// </summary>
    public int nextIndex;
    /// <summary>
    /// 方格出生点
    /// </summary>
    public Transform bornPoint;

    public void Awake() {
        myGrid = new int[12, 20];
        //for (var i = 0; i < 12; i++)
        //    for (var j = 0; j < 20; j++)
        //        myGrid[i,j] = 0;
        gridBlocks = new Dictionary<int,List<Transform>>(20);
        for (var m = 0; m < 20; m++)
            gridBlocks[m] = new List<Transform>();

        fullRow = new List<int>(4);
        nextIndex = Random.Range(0, TGroup.Length - 1);
    }

    public void Start() {
        CreateNextBlockGroup();
    }

    public void Update() {
        //for (int i = 0; i < 12; i++) {
        //    for (int j = 0; j < 20; j++) {
        //        print("(" + i + "," + j + ")" + " = " + myGrid[i, j]);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.M)) {
            for (int row = 0; row < 20; row++) {
                string s = "";
                for (int col = 0;  col < 12; col++) {
                    s += myGrid[col, row] + " ";
                }
                print(s);
            }
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            for (int row = 0; row < 20; row++) {
                print(gridBlocks[row].Count);
            }
        }
    }

    /// <summary>
    /// 用于将方格的世界坐标转化为索引值
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static Vector2 TransformConvertToIndex(Transform trans) {
        int x = (int)Mathf.Round(trans.position.x - (-12f));
        int y = (int)Mathf.Round(trans.position.y - (-9.5f));
        return new Vector2(x,y);
    }
    private bool canCreateNext = true;
    /// <summary>
    /// 生成下一个方块类型
    /// </summary>
    public void CreateNextBlockGroup() {

        if (canCreateNext) {
            lastIndex = nextIndex;

            Transform player = Instantiate(TGroup[nextIndex], bornPoint.position, Quaternion.Euler(0, 0, 90)).transform;
            nextIndex = Random.Range(0, TGroup.Length);

            ChangeNectBlockShowInTitle();

            if (NextHasBlock(player)) {
                while (NextHasBlock(player)) {
                    player.position += new Vector3(0, 1, 0);
                }
                if (EscapeUpBoard(player)) {
                    print("GameOver!");
                    canCreateNext = false;
                    foreach (var son in player.GetComponentsInChildren<Transform>()) {
                        if (son == player) continue;
                        if (son.position.y > 9.5f) {
                            Destroy(son.gameObject);
                        }
                    }
                }    
            }
        }  
    }
    /// <summary>
    /// 改变下一个产生的方块类型的显示
    /// </summary>
    public Transform TitleRow;
    public void ChangeNectBlockShowInTitle() {
        TitleRow.Find(TGroup[lastIndex].name).gameObject.SetActive(false);
        TitleRow.Find(TGroup[nextIndex].name).gameObject.SetActive(true);
    }

    /// <summary>
    /// 判断方块组内的成员是否都在边界内
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public bool GroupIsInBottomLeftRightBoard(Transform parent) {
        foreach (var son in parent.GetComponentsInChildren<Transform>()) {
            if (son == parent) continue;
            if (son.position.x < -12.1f || son.position.x > -0.9f) {
                return false;
            }
            if (son.position.y < -9.6f) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检测方块组所在的位置是否已有方块
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public bool NextHasBlock(Transform parent) {
        foreach (var son in parent.GetComponentsInChildren<Transform>(true)) {
            if (son == parent) continue;
            Vector2 index = MyGridManager.TransformConvertToIndex(son);
            try {
                if (myGrid[(int)index.x, (int)index.y] == 1) {  // 位置被占用
                    return true;
                }
            }
            catch (System.Exception e) {
                print(e);
            }
        }
        return false;
    }

    public bool EscapeUpBoard(Transform parent) {
        foreach (var son in parent.GetComponentsInChildren<Transform>(true)) {
            if (son == parent) continue;
            if (son.position.y > 9.5f) return true;
        }
        return false;
    }

    List<int> fullRow;
    /// <summary>
    /// 检查是否有满行的方块
    /// </summary>
    public void CheckBlockRow() {
        for (var row = 0; row < 20; row++) {
            for (var col = 0; col < 12; col++) {
                if (myGrid[col, row] != 1) break;
                if (col == 11) fullRow.Add(row);
            }
        }

        if (fullRow.Count > 0)
            StartCoroutine(XiaoChuEffect(fullRow));
        else
            CreateNextBlockGroup();
    }

    public GameObject _arrowPrefab;
    private bool _isArrowflashing = false;
    IEnumerator ShowEffect(List<int> fullRow) {
        float timer = Time.time;
        // 显示行
        List<Transform> arrow = new List<Transform>();
        foreach (var row in fullRow) {
            Transform t = Instantiate(_arrowPrefab, new Vector3(-13.5f,-9.5f + row ,0),Quaternion.identity).transform;
            arrow.Add(t);
            foreach (var block in gridBlocks[row]) {
                block.gameObject.AddComponent<Flashing>();
            }
        }
        yield return new WaitUntil(()=>Time.time - timer > 2.0f);
        foreach (var t in arrow) {
            Destroy(t.gameObject);
        }
        arrow.Clear();
        _isArrowflashing = true;
    }

    IEnumerator XiaoChuEffect(List<int> fullRow) {
        StartCoroutine(ShowEffect(fullRow));
        yield return new WaitUntil(()=> { return _isArrowflashing; });
        _isArrowflashing = false;
        //yield return new WaitUntil(FallComplete);
        int gap = 0;
        //foreach (var f in fullRow) print(f);
        //for(var i = 0;i<fullRow.Count;i++)
        //    foreach (var g in gridBlocks[fullRow[i]]) print(g.gameObject.name);
        for (var j = fullRow[0]; j < 20; j++) {
            List<Transform> row = gridBlocks[j];
            if (fullRow.Count > 0 && j == fullRow[0]) {
                gap++;
                fullRow.RemoveAt(0);

                // 删除满行方块组
                foreach (var m in row) {
                    //print(TransformConvertToIndex(m));
                    Vector2 v = TransformConvertToIndex(m);
                    myGrid[(int)(v.x), (int)(v.y)] = 0;
                    Destroy(m.gameObject);
                    //Transform temp = gridBlocks[j][m];
                    //Vector2 v = TransformConvertToIndex(temp);
                    //myGrid[(int)(v.x), (int)(v.y)] = 0;
                    //gridBlocks[j].Remove(temp);
                    //Destroy(temp.gameObject);
                    GameObject.FindObjectOfType<GameValueManager>().Goal += 1;
                }
                row.Clear();
                //foreach (Transform t in gridBlocks[j]) {
                //    Vector2 v = TransformConvertToIndex(t);
                //    myGrid[(int)(v.x), (int)(v.y)] = 0;
                //    if(gridBlocks[(int)v.y].Count > 0) gridBlocks[(int)v.y].Clear();
                //    Destroy(t.gameObject);
                //    //加分
                //    //GetGoal();
                //}
            }
            else {
                //上侧显示的方块下落
                foreach (var m in row) {
                    Vector2 v1 = TransformConvertToIndex(m);
                    myGrid[(int)(v1.x), (int)(v1.y)] = 0;
                    m.position -= new Vector3(0, gap, 0);
                    Vector2 v2 = TransformConvertToIndex(m);
                    myGrid[(int)(v2.x), (int)(v2.y)] = 1;
                    
                }
                List<Transform> temp = gridBlocks[j];
                gridBlocks[j] = gridBlocks[j - gap];
                gridBlocks[j - gap] = temp;
            }   
        }

        CreateNextBlockGroup();
    }

}
