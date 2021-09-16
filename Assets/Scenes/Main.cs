using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GridLayoutGroup GridRoot;
    public ColorItem GridItemTemplate;
    public List<ColorItem> SelectItems;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 400; i++)
        {
            ColorItem Item = GameObject.Instantiate<ColorItem>(GridItemTemplate);
            Item.gameObject.SetActive(true);
            Item.gameObject.transform.SetParent(GridRoot.transform);
            Item.SetPos(i % 20, i / 20);
            Item.Parent = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectItem(ColorItem item, bool IsSelect)
    {
        if (IsSelect && !SelectItems.Contains(item))
            SelectItems.Add(item);
        else if (!IsSelect && SelectItems.Contains(item))
            SelectItems.Remove(item);
    }
    public void OnClickGo()
    {
        Dictionary<int, Dictionary<string, ColorItem>> Results = PassBegin(SelectItems);
        foreach(KeyValuePair<int, Dictionary<string, ColorItem>> Result in Results)
        {
            int areaIndex = Result.Key;
            Dictionary<string, ColorItem> items = Result.Value;
            foreach (KeyValuePair<string, ColorItem> item in items)
            {
                float R = 0;
                float G = 0;
                float B = 0;
                if(areaIndex * 30 <= 255)
                {
                    R = (float)areaIndex * 30.0f / 255;
                }
                else if(areaIndex * 30 > 255 && areaIndex * 30 <= 510)
                {
                    R = 1.0f;
                    G = (float)areaIndex * 30.0f / 255;
                }
                else
                {
                    R = 1.0f;
                    G = 1.0f;
                    B = areaIndex * 30 > (255+255+255) ? 1.0f : (float)areaIndex * 30.0f / 255.0f;
                }

                item.Value.SetText(areaIndex.ToString(), new Color(R, G, B ));
            }
        }
    }
    //////////////算法///////
    /*
 *对区域进行拆分。当橡皮擦擦除后，可能原来的区域变成多个区域，需要对区域进行计算，查找出每一个区域。
 *InPoints:区域混合在一起的点集
 */
    Dictionary<int, Dictionary<string, ColorItem>> PassBegin(List<ColorItem> list)
    {
        Dictionary<int, Dictionary<string, ColorItem>> Result = new Dictionary<int, Dictionary<string, ColorItem>>();
		List<ColorItem> tempList = new List<ColorItem>(list);
		tempList.Sort((a, b) =>
		{
			if (a.y != b.y)
				return a.y - b.y;
			return a.x - b.x;
		});

		foreach(ColorItem item in tempList)
		{
			Debug.Log(item.x + ":" + item.y);
        }

        int CurrentMapIndex = 0;
        for (int i = 0; i < tempList.Count; i++)
        {
			ColorItem item = tempList[i];
            SearchInResult(item, ref Result, ref CurrentMapIndex);
        }
		return Result;
    }
    
	//进行8方向查找
	int[,] Dir = new int[8, 2] { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 } };
	//int Dir[8][2]={{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1}};

	/*
	 *对一个点进行八方向查找，如果找到相邻点已经在结果内，则放入结果，如果查找出多个结果，则合并多个结果到Index最小的结果当中。
	 *Point:起始点
	 *Result:查找点集
	 *Index:新增结果点集的index
	 */
	void SearchInResult(ColorItem CurrentItem,ref Dictionary<int, Dictionary<string, ColorItem>> Results,ref int currentIndex)
    {
        List<int> SuperPositionKeys = new List<int>();//同一点扫描到在多个区域，记录区域index，用于合并
        for (int i = 0; i < 8; i++)
        {
            int SearchX = CurrentItem.x + Dir[i, 0];
            int SearchY = CurrentItem.y + Dir[i, 1];
            string SearchKey = SearchX + ":" + SearchY;
            foreach (KeyValuePair<int, Dictionary<string, ColorItem>> SubResult in Results)
            {

                if (SubResult.Value.ContainsKey(SearchKey))
                {
                    if (!SuperPositionKeys.Contains((int)SubResult.Key))
                        SuperPositionKeys.Add(SubResult.Key);
                    if (SuperPositionKeys.Count == 1)
                    {
                        if (!SubResult.Value.ContainsKey(CurrentItem.KeyWord))
                            SubResult.Value.Add(CurrentItem.KeyWord, CurrentItem);
                    }
                }
            }
            //当结果大于等于2的时候，一已经发现需要合并的区域，所以不需要再继续查找后面位置了
            if (SuperPositionKeys.Count >= 2)
                break;
        }
        if (SuperPositionKeys.Count == 0)
        {
            Dictionary<int, Dictionary<string, ColorItem>> SubResult = new Dictionary<int, Dictionary<string, ColorItem>>();
            Dictionary<string, ColorItem> Colors = new Dictionary<string, ColorItem>();
            Colors.Add(CurrentItem.KeyWord, CurrentItem);
            Results.Add(currentIndex, Colors);
            currentIndex++;
        }

        if (SuperPositionKeys.Count > 1)
        {
            //合并入index最小的分区。
            SuperPositionKeys.Sort((a, b) => {
                return a - b;
            });
            //合并查找到的连通的多个分区
            Dictionary<string, ColorItem> ParentSubResult = Results[SuperPositionKeys[0]];
            for (int i = SuperPositionKeys.Count - 1; i >= 1; i--)
            {
                Dictionary<string, ColorItem> SubPositions = Results[SuperPositionKeys[i]];
                foreach(KeyValuePair<string, ColorItem> sub in SubPositions)
                {
                    ParentSubResult.Add(sub.Key, sub.Value);
                }
                Results.Remove(SuperPositionKeys[i]);
            }
        }
    }
    //////////////算法END///////
}
