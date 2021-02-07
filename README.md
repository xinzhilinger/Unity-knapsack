### 前言：


>**背包系统：**
>
>- 背包系统是游戏中非常重要的元素，几乎每一款游戏都有背包系统，我们使用背包系统可以完成装备栏的数据管理，商店物体的数据管理等等一系列的功能，如何做好一个背包系统呢，来学习把！

本次案例是基于数据本地存储的方式设计的背包系统，首先在数据管理方面，使用`ScriptableObject`类来实创建数据仓库，可以保存在游戏系统中，重新开始后依旧可以再次加载出来该数据


**前期规划：**

为了使得游戏逻辑简洁清晰，可以使用MVC框架完成背包系统的设计，来尽量降低数据、逻辑耦合，提升不同的模块的独立性。便于程序的扩展和维护

>**什么是MVC：**
>
>- M即`Model`（模型）
>- V即`View`（视图）
>- C即`Control`（控制）
>
>**而使用MVC的目的就是将M和V进行代码分离，便于逻辑区分**

背包系统要实现的是，存在于背包内的物体可以显示在UI上，而背包内物体是动态变化的（减少或增加），所以UI可以根据这种变化而进行动态的变化，所以我们划分为如下区块：

- 首先是M（业务模型）：我们通过`ScriptableObject`类创建数据仓库来构造业务模型
- 然后是V（用户界面）：通过UI界面显示背包内所有物体
- 最后是C（逻辑控制）：获取数据仓库模型并转换到UI界面






### UI端:

**一、UI搭建**

创建一个`Panel`作为背包系统中所有物体的父元素，并为其更换一个背景图片，这样可以使得物体有容纳的空间，能够使得物体更加序列化，更规整：

![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205154857726.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

接下来需要对`MainGrid`添加组件`Grid Layout Group`组件，该组件的作用是可以是的子元素按照一定的格式排列在父元素中，这样就很容易使我们动态的显示出不定长的UI元素同时保持规整的效果，
>**关于`Grid Layout Group`的具体作用和使用方式可以查看之前文章：**
>- [Unity 几种排版方式：Layout Group](https://blog.csdn.net/xinzhilinger/article/details/111186083)

为了使得添加的子物体可以完美的匹配的每一个格子中，需要调整`Grid Layout Group`组件的相关参数，而其中主要是`padding`，`Call Size`和`Spacing`三个元素，这三个参数调整的具体内容为：

- `Padding`：类似网页设计内边距
- `Cell Size`：组中每个布局元素要使用的大小
-  `Spacing`：布局之间的元素间距


最终达到这样的效果，每添加一个图片子元素，则向后排列一位，这样可以完美的显示在格子之中：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205155322794.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)
**二、格子元素预制体制作**

为了使得背包内的物体可以在UI中显示出来，需要将这些物体做成对应的UI元素，而UI元素则有两个关键点，图标和数量，为了实现这样 的效果，首先创建一个`Image`命名为`GridPrefab`作为图标，然后为其添加`Text`子元素作为物体数量显示框：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205161205727.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)



在游戏中，我们只需要为`GridPrefab`的`Image`组件中的`Source Image`添加图片，即可实现最终的显示效果，制作完成后将`GridPrefab`拖入到`Assets`中即可创建一个预制体

由于我们需要在使用预制体时，需要修改其一些信息，比如说`Image`组件中的`Source Image`添加图片和子元素`Text`中的显示内容（物体个数），为了方便更改，使用一个脚本整合这些需要修改地方，方便其他脚本调用，脚本命名为`Grid`，并挂载到`GridPrefab`上：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public Image gridImage;
    public Text girdNum;
}
```
挂载完脚本后，将组件或元素拖入：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205161840104.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)


### 数据端：
**一，基于ScriptableObject本地保存数据**

`ScriptableObject`可以用来创建不需要挂载到场景物体上的脚本，这样可以将游戏数据保存于本地，不会在每次游戏结束时重置数据，我们可以利用这一点来设计出数据的存储仓库

>  **关于ScriptableObject的一些细节可以查看该文章：**
>  - [Unity 数据存储方式之一：ScriptableObject](https://blog.csdn.net/xinzhilinger/article/details/111180070)


创建一个存储物体数据的仓库，我们可以使用`CreateAssetMenu`来完成创建`AssetMenu` 的方法，这样就可以使用`Create`来在资源中创建定义的仓库

**具体操作流程：**

**1，物体数据仓库创建：**

首先创建一个类`Item`（用来创建一个物体（比如说一件武器）数据存储仓库）继承于`ScriptableObject`，既然是背包物体，需要存储的数据主要是UI显示需要的图标图片，物体名称和物体数量，以及物体的详细信息（本案例用不到）等：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现可以在Asset窗口创建资源文件的方法
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
```
完成脚本后保存，在`Asset`面板右键创建，即可看到`New Item`选项，点击可以创建一个物体数据仓库，并且可以填写其中的信息，也可以通过脚本来修改信息，并且是永久保存的

- 创建一个数据仓库：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205163623239.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

- 修改数据仓库信息：

![在这里插入图片描述](https://img-blog.csdnimg.cn/20210205164007600.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

**2，背包数据仓库的创建**

创建该仓库是为了存储背包中究竟有哪些物体，也就是有哪些`Item`，创建一个类命名为`MainItem`同样继承于`ScriptableObject`，并定义一个列表来背包中所有物体：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New MainItem",menuName ="Bag/New MainItem")]
public class MainItem : ScriptableObject
{
    public List<Item> itemList=new List<Item>();
}

```
**创建操作流程：**
- 在资源面板点击后创建一个`MainItem`背包物体管理仓库，用来存放背包中的`Item`数据：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210206103653167.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)
- 查看创建的数据仓库，可以发现是一个列表管理资源，我们可以将一系列的`Item`资源拖入其中，这样就可以在背包中保存所有物品的信息：


![在这里插入图片描述](https://img-blog.csdnimg.cn/20210206104230514.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)
### 游戏场景端：

**1，搭建场景**

在`Sence`面板搭建一个简单的场景，并添加一个`Capsule`作为玩家控制角色，然后添加一个`Cube`作为场景中可以捡拾的物体，当玩家捡拾到该物体后，就会在背包UI中显示出来

首先对于，玩家控制角色，为其添加`Rigidbody`组件并为其添加脚本`PlayerCtrl`来控制其移动和背包UI的显示和隐藏：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Vector3 v3;
    float speed = 2f;
    public GameObject myBagUI;
    private bool isPlay;

    // Update is called once per frame
    void Update()
    {
        //控制角色移动
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        v3 = new Vector3(x, 0, y);
        transform.Translate(v3*speed*Time.deltaTime);

        //控制UI显示或隐藏
        if(Input.GetKeyDown(KeyCode.B))
        {
            isPlay = !isPlay;
            myBagUI.SetActive(isPlay);
        }
    }
}
```
完成脚本后挂载到`Player`上，并将UI元素`MyBag`拖入：
![在这里插入图片描述](https://img-blog.csdnimg.cn/2021020611360549.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

**2，场景中物体的设计**


接下来要完成在游戏场景中的物体代码设置，为其添加脚本，在玩家碰撞到该物体时，将该物体对应的数据仓库`Item`添加到背包中的数据仓库中，如果背包中的数据仓库已经存在该物体，则将对应物体的数据仓库中的持有数量加一


>为了方便理解，可以将其看作为吃鸡游戏中地面上子弹，当玩家捡拾后，对应的角色背包中就会出现子弹，如果背包中有之前捡拾的子弹，则背包中子弹数量会增加，本案例使用Cube来代替
>

首先为Cube创建一个脚本命名为`ScenesItem`来检测碰撞，并获取该物体的数据仓库将其添加到背包数据仓库中：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesItem : MonoBehaviour
{
    //物体的数据仓库
    public Item item;
    //背包的数据仓库
    public MainItem mainItem;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name=="Player")
        {
			
            if(!mainItem.itemList.Contains(item))
            {
                mainItem.itemList.Add(item);    
            }
            item.itemNum += 1;
            Destroy(this.gameObject);
        }
    }
}

```

完成后，将脚本挂载到物体上，并拖入于该物体对应的物体数据仓库和背包数据仓库：
![在这里插入图片描述](https://img-blog.csdnimg.cn/2021020618253143.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

### 将数据显示于UI：
通过读取数据仓库的信息，将背包的数据转换到UI元素中显示出来

首先通过读取背包数据仓库中的列表信息，读取到列表中所有物体数据仓库中的信息，获取到背包仓库中存在的物体图片和数量，并显示在UI上面，创建一个脚本命名为`BagDisplayUI`并挂载到`Canvas`上，为了确保该类只有一个实例，用单例模式来实现该需求

```csharp
public class BagDisplayUI : MonoBehaviour
{
    static BagDisplayUI bagDisplayUI;
    private void Awake()
    {
        if(bagDisplayUI!=null)
        {
            Destroy(this);            
        }
        bagDisplayUI = this;
               
    }
}
```
然后定义一些我们所需要的变量，首先需要是背包数据仓库，需要通过该仓库遍历到背包中物体的数据仓库，然后就是之前做好的显示每一个物体图标和数量的UI预制体，第三个则是用来作为添加每一个UI预制体的父物体

```csharp
    //背包数据仓库、格子中物体预制体、和UI中显示物体元素的父元素
    public MainItem mainItem;
    public Grid gridPrefab;
    public GameObject myBag; 
```

定义完后，将需要的变量拖入：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210206202242332.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

接下来编辑该脚本，实现在UI显示背包物体的功能：


```csharp
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
    //每次游戏启动前，动态的更新背包UI元素
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
```
同时在每一个物体的碰撞检测脚本上调用更新UI元素的方法，这是为了在`Player`碰撞到物体时，背包数据仓库发生变化时，可以在UI元素上也同步变化：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210206203228607.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3hpbnpoaWxpbmdlcg==,size_16,color_FFFFFF,t_70)

### 总结：

通过MVC框架思想来设计背包系统，尽量保证数据、视图的分离，通过控制端来连接两者，提升独立性

而对于背包系统的主体设计，主要在于数据的保存和读取，本次案例我们采用的是本地存储的方式。在网络游戏中，如果采用数据保存在服务器的方式，可以通过读取网络发送过来的Json数据等方式来实现在UI界面的显示

无论是本地还是服务器，转换为UI界面的展示效果方法基本相同，主要是对数据的处理方式的差异



b













