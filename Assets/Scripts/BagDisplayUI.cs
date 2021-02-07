using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagDisplayUI : MonoBehaviour
{
    //单例模式
    static BagDisplayUI bagDisplayUI;
    private void Awake()
    {
        if(bagDisplayUI!=null)
        {
            Destroy(this);
            
        }
        bagDisplayUI = this;
               
    }
    private void OnEnable()
    {
        updateItemToUI();
    }
    //背包数据仓库、格子中物体预制体、和UI中显示物体元素的父元素
    public MainItem mainItem;
    public Grid gridPrefab;
    public GameObject myBag; 

    /// <summary>
    /// 在UI中将一个物体的数据仓库显示出来
    /// </summary>
    /// <param name="item"></param>
    public static void insertItemToUI(Item item)
    {
        
        Grid grid = Instantiate(bagDisplayUI.gridPrefab, bagDisplayUI.myBag.transform);
        grid.gridImage.sprite = item.itemImage;
        grid.girdNum.text = item.itemNum.ToString();
        
    }

    /// <summary>
    /// 将背包数据仓库中所有物体显示在UI上
    /// </summary>
    public static void updateItemToUI()
    {
        for (int i = 0; i < bagDisplayUI.myBag.transform.childCount; i++)
        {
            Destroy(bagDisplayUI.myBag.transform.GetChild(i));
        }
        for (int i = 0; i < bagDisplayUI.mainItem.itemList.Count; i++)
        {
            insertItemToUI(bagDisplayUI.mainItem.itemList[i]);
        }
    }
}
