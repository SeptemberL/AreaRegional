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
    //////////////�㷨///////
    /*
 *��������в�֡�����Ƥ�������󣬿���ԭ���������ɶ��������Ҫ��������м��㣬���ҳ�ÿһ������
 *InPoints:��������һ��ĵ㼯
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
    
	//����8�������
	int[,] Dir = new int[8, 2] { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 } };
	//int Dir[8][2]={{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1}};

	/*
	 *��һ������а˷�����ң�����ҵ����ڵ��Ѿ��ڽ���ڣ����������������ҳ�����������ϲ���������Index��С�Ľ�����С�
	 *Point:��ʼ��
	 *Result:���ҵ㼯
	 *Index:��������㼯��index
	 */
	void SearchInResult(ColorItem CurrentItem,ref Dictionary<int, Dictionary<string, ColorItem>> Results,ref int currentIndex)
    {
        List<int> SuperPositionKeys = new List<int>();//ͬһ��ɨ�赽�ڶ�����򣬼�¼����index�����ںϲ�
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
            //��������ڵ���2��ʱ��һ�Ѿ�������Ҫ�ϲ����������Բ���Ҫ�ټ������Һ���λ����
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
            //�ϲ���index��С�ķ�����
            SuperPositionKeys.Sort((a, b) => {
                return a - b;
            });
            //�ϲ����ҵ�����ͨ�Ķ������
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
    //////////////�㷨END///////
}
