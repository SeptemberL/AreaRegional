using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorItem : MonoBehaviour
{
    public Main Parent;
    public int x;
    public int y;
    public Text TextDesc;
    public Image ImageColor;

    bool IsSelect = false;
    public string KeyWord;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        IsSelect = !IsSelect;
        if (IsSelect)
        {
            ImageColor.color = Color.blue;
            TextDesc.color = Color.white;
        }
        else
        {
            ImageColor.color = Color.white;
            TextDesc.color = Color.black;
        }
        Parent.SelectItem(this, IsSelect);
        TextDesc.text = x + ":" + y;
    }

    public void SetPos(int x, int y)
    {
        this.x = x;
        this.y = y;
        TextDesc.text = x + ":" + y;
        KeyWord = TextDesc.text;
    }

    public void SetText(string text, Color color)
    {
        TextDesc.text = text;
        ImageColor.color = color;
        TextDesc.color = Color.gray;
    }
}
