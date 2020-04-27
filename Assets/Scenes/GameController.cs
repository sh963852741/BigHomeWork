﻿using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Gemstone gemstone;
    public int rowNum = 7; //宝石列数  
    public int columNum = 10; //宝石行数  
    public ArrayList gemstoneList; //宝石列表  
    private Gemstone currentGemstone;
    private ArrayList matchesGemstone;
    public AudioClip match3Clip;
    public AudioClip swapClip;
    public AudioClip erroeClip;

    void Start()
    {
        /* 初始化游戏，生成宝石 */
        gemstoneList = new ArrayList(); 
        matchesGemstone = new ArrayList();
        for (int rowIndex = 0; rowIndex < rowNum; rowIndex++)
        {
            ArrayList temp = new ArrayList();
            for (int columIndex = 0; columIndex < columNum; columIndex++)
            {
                Gemstone c = AddGemstone(rowIndex, columIndex);
                temp.Add(c);

            }
            gemstoneList.Add(temp);
        }

        // 开始检测匹配消除 
        if (CheckHorizontalMatches() || CheckVerticalMatches()) RemoveMatches();
    }

    /// <summary>
    /// 生成宝石
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columIndex"></param>
    /// <returns></returns>
    public Gemstone AddGemstone(int rowIndex, int columIndex)
    { 
        Gemstone c = Instantiate(gemstone) as Gemstone;
        c.transform.parent = this.transform; // 生成宝石为GameController子物体  
        c.GetComponent<Gemstone>().RandomCreateGemstoneBg();
        c.GetComponent<Gemstone>().UpdatePosition(rowIndex, columIndex);
        return c;
    }

    // Update is called once per frame  
    void Update()
    {

    }

    /// <summary>
    /// 选择宝石
    /// </summary>
    /// <param name="c"></param>
    public void Select(Gemstone c)
    {
        //Destroy (c.gameObject);
        if (currentGemstone == null) // 没有选中任何宝石
        {
            currentGemstone = c;
            currentGemstone.isSelected = true;
            return;
        }
        else // 已经选中了宝石
        {
            if (Mathf.Abs(currentGemstone.rowIndex - c.rowIndex) + Mathf.Abs(currentGemstone.columIndex - c.columIndex) == 1) // 两颗宝石距离正确
            {
                //ExangeAndMatches(currentGemstone,c);  
                StartCoroutine(ExangeAndMatches(currentGemstone, c));
            }
            else
            {
                this.gameObject.GetComponent<AudioSource>().PlayOneShot(erroeClip);
            }
            currentGemstone.isSelected = false;
            currentGemstone = null;
        }
    }

    /// <summary>
    /// 实现宝石交换并且检测匹配消除
    /// </summary>
    /// <param name="c1">第一颗宝石</param>
    /// <param name="c2">第二颗宝石</param>
    /// <returns></returns>
    IEnumerator ExangeAndMatches(Gemstone c1, Gemstone c2)
    {
        Exchange(c1, c2);
        yield return new WaitForSeconds(0.5f);
        if (CheckHorizontalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
        else
        {
            Exchange(c1, c2);
        }
    }

    /// <summary>
    /// 实现检测水平方向的匹配
    /// </summary>
    /// <returns>是否匹配</returns>
    bool CheckHorizontalMatches()
    { 
        bool isMatches = false;
        for (int rowIndex = 0; rowIndex < rowNum; rowIndex++)
        {
            for (int columIndex = 0; columIndex < columNum - 2; columIndex++)
            {
                if ((GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex, columIndex + 1).gemstoneType) 
                    && (GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex, columIndex + 2).gemstoneType))
                {
                    //Debug.Log ("发现行相同的宝石");  
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex, columIndex + 1));
                    AddMatches(GetGemstone(rowIndex, columIndex + 2));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    /// <summary>
    /// 实现检测垂直方向的匹配
    /// </summary>
    /// <returns>是否匹配</returns>
    bool CheckVerticalMatches()
    {
        bool isMatches = false;
        for (int columIndex = 0; columIndex < columNum; columIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowNum - 2; rowIndex++)
            {
                if ((GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex + 1, columIndex).gemstoneType) && (GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex + 2, columIndex).gemstoneType))
                {
                    //Debug.Log("发现列相同的宝石");  
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex + 1, columIndex));
                    AddMatches(GetGemstone(rowIndex + 2, columIndex));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    void AddMatches(Gemstone c)
    {
        if (matchesGemstone == null) matchesGemstone = new ArrayList();
        int index = matchesGemstone.IndexOf(c); //检测宝石是否已在数组当中  
        if (index == -1)
        {
            matchesGemstone.Add(c);
        }
    }

    /// <summary>
    /// 删除匹配的宝石
    /// </summary>
    void RemoveMatches()
    {  
        for (int i = 0; i < matchesGemstone.Count; i++)
        {
            Gemstone c = matchesGemstone[i] as Gemstone;
            RemoveGemstone(c);
        }
        matchesGemstone = new ArrayList();
        StartCoroutine(WaitForCheckMatchesAgain());
    }

    /// <summary>
    /// 连续检测匹配消除
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForCheckMatchesAgain()
    { 
        yield return new WaitForSeconds(0.5f);
        if (CheckHorizontalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
    }

    /// <summary>
    /// 删除宝石
    /// </summary>
    /// <param name="c"></param>
    void RemoveGemstone(Gemstone c)
    {
        //Debug.Log("删除宝石");  
        c.Dispose();
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(match3Clip);
        for (int i = c.rowIndex + 1; i < rowNum; i++)
        {
            Gemstone temGemstone = GetGemstone(i, c.columIndex);
            temGemstone.rowIndex--;
            SetGemstone(temGemstone.rowIndex, temGemstone.columIndex, temGemstone);
            temGemstone.UpdatePosition(temGemstone.rowIndex,temGemstone.columIndex);  
            //temGemstone.TweenToPostion(temGemstone.rowIndex, temGemstone.columIndex);
        }
        Gemstone newGemstone = AddGemstone(rowNum, c.columIndex);
        newGemstone.rowIndex--;
        SetGemstone(newGemstone.rowIndex, newGemstone.columIndex, newGemstone);
        newGemstone.UpdatePosition (newGemstone.rowIndex, newGemstone.columIndex);  
        //newGemstone.TweenToPostion(newGemstone.rowIndex, newGemstone.columIndex);
    }

    /// <summary>
    /// 通过行号和列号，获取对应位置的宝石 
    /// </summary>
    /// <param name="rowIndex">行号</param>
    /// <param name="columIndex">列号</param>
    /// <returns>宝石对象</returns>
    public Gemstone GetGemstone(int rowIndex, int columIndex)
    {
        ArrayList temp = gemstoneList[rowIndex] as ArrayList;
        Gemstone c = temp[columIndex] as Gemstone;
        return c;
    }

    public void SetGemstone(int rowIndex, int columIndex, Gemstone c)
    {//设置所对应行号和列号的宝石  
        ArrayList temp = gemstoneList[rowIndex] as ArrayList;
        temp[columIndex] = c;
    }

    /// <summary>
    /// 实现宝石交换位置
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    public void Exchange(Gemstone c1, Gemstone c2)
    {
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(swapClip);
        SetGemstone(c1.rowIndex, c1.columIndex, c2);
        SetGemstone(c2.rowIndex, c2.columIndex, c1);
        //交换c1，c2的行号  
        int tempRowIndex;
        tempRowIndex = c1.rowIndex;
        c1.rowIndex = c2.rowIndex;
        c2.rowIndex = tempRowIndex;
        //交换c1，c2的列号  
        int tempColumIndex;
        tempColumIndex = c1.columIndex;
        c1.columIndex = c2.columIndex;
        c2.columIndex = tempColumIndex;

        c1.UpdatePosition (c1.rowIndex, c1.columIndex);  
        c2.UpdatePosition (c2.rowIndex, c2.columIndex);  
        //c1.TweenToPostion(c1.rowIndex, c1.columIndex);
        //c2.TweenToPostion(c2.rowIndex, c2.columIndex);
    }
}