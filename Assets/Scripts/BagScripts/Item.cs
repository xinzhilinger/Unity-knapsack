using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Item",menuName ="Bag/New Item")]
public class Item : ScriptableObject
{
    //物体名、需要在UI中显示的图片、持有物体的数量、物体信息的描述
    public string itemName;  
    public Sprite itemImage; 
    public int itemNum;      
    [TextArea] //改变输入框格式，提示输入框容量
    public string itemInfo;   
}
