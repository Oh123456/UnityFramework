# UnityFramework
유니티 게임 개발에 있어 자주 사용되거나 제공되지 않는 컨테이너를 재사용을 위해 제작 했습니다. 

# 목차
- ㅁㄴㅇㄹ

# UI <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/UIManager"><img src="https://img.shields.io/badge/Git-F05032?style=flat-square&logo=GitURL&logoColor=white"/></a>
유니티 모바일 환경에 알 맞는 UI를 관리 밎 최적화에 초점을 둔 프레임 워크입니다. 

추후에 PC환경에도 대응할수있게 보완할 예정입니다.

${\textsf{\color{#1589F0}namespace}}$  `UnityFramework.UI`

## UIBase
```
[SerializeField] protected Canvas canvas;
[SerializeField] private GraphicRaycaster graphicRaycaster;


protected virtual void Show()
{
    if (isShow)
        return;

    canvas.enabled = true;
    if (graphicRaycaster != null)
        graphicRaycaster.enabled = true;
    isShow = true;
    if (!gameObject.activeSelf)
        gameObject.SetActive(true);
    OnShow?.Invoke();   
}

protected virtual void Hide()
{
    if (!isShow)
        return;

    canvas.enabled = false;
    if (graphicRaycaster != null)
        graphicRaycaster.enabled = false;
    isShow = false;
    OnHide?.Invoke();
}

```
유니티 UI 최적화를 위해 기본적으로 `캔버스(Canvas)`를 `OnOff` 방식을 사용합니다. 

유니티에서는 캔버스(Canvas)의 `게임 오브젝트(GameObject)`가 비활성화 상태에서 `활성화` 상태로 변경될때 `정점(vertex)`들을 `전부 다시 그리기`때문에 `자주 OnOff`되는 UI는 `캔버스(Canvas)` `비활성화` 하는 방법이 더욱 효과적입니다.

```
protected virtual void Close()
{
    if (!isShow)
        return;
    gameObject.SetActive(false);
    OnClose?.Invoke();
}
```
물론 게임 오브젝트(GameObject)를 비활성화 하는경우가 더 효율적일수있기에 비활성화 하는 기능도 존재합니다.

기본적으로 `외부에서` `Show`, `Hdie`, `Close` 가 `불가능`하고 `UIManager`를 통해 `Show`, `Hide`, `Close` 가 `가능`합니다.
```
public event System.Action OnShow;
public event System.Action OnHide;
public event System.Action OnClose;
```
각 `Show` , `Hide` , `Close` 될때 이벤트가 `호출`되어 타이밍에 맞게 동작이 가능합니다.

### MainUIBase
```
public class MainUIBase : UIBase
{
    public void AddListener(UIManager.UIController uIController)
    {
        uIController.Show = Show;
        uIController.Hide = Hide;
        uIController.Close = Close;
    }

    /// <summary>
    /// 백버튼을 눌렀을때 다른 SubUI가 꺼져야하는 경우
    /// </summary>
    /// <returns>true MainUI가 Hide 가능할때</returns>
    public virtual bool ExecuteButton()
    {
        return true;
    }
}
```
메인 으로사용될 Base 입니다. 제일 최상위 Canvas에 단하나만 존재해야합니다. 

### SubUI
```
public class SubUI : UIBase , ISubUI
{
    enum ShowType
    {
        Auto,
        Custom,
    }

    [SerializeField] UIBase parentUI;
    [SerializeField] ShowType showType = ShowType.Auto;

    protected override void Reset()
    {
        base.Reset();
        parentUI = FindParentUIBase();
    }

    protected override void Initialize()
    {
        base.Initialize();  
        if (showType == ShowType.Auto)
            parentUI.OnShow += base.Show;
        parentUI.OnHide += base.Hide;
    }

    public new void Show()
	{ 
		base.Show();
	}

    public new void Hide()
    {
        base.Hide();
    }

    public new void Close()
    { 
        base.Close();
    }


    public UIBase FindParentUIBase()
    {
        return UIUtils.FindParentIndependentUIBase(transform as RectTransform);
    }

    public bool IsIndependent()
    {
        return canvas.overrideSorting;  
    }
} 

```
유니티 `UI 최적화`를위해 만들어졌습니다. UI의 `요소 하나`가 바뀌면 해당 `캔버스(Canvas)의 모든 UI`가 `다시 드로우`가 됩니다. 자주 변경이되는 UI와 정적인 UI를 분리할때 사용됩니다. 

예외적으로 `외부`에서 `Show`, `Hide`,`Close` 가 `가능`합니다.

※ 계층이 적은경우에는 SubUI를 사용하는게 성능이 안 좋을수있습니다.

## UIManager
전반적으로 MainUIBase를 상속받은 UI들을 관리하는 클래스입니다.

```
private Dictionary<System.Type, UIBase> uis = new Dictionary<System.Type, UIBase>();
```
Dictionary 를 통하여 `한번이라도` Show 한 UI는 계속 `데이터를 캐싱` 합니다.

```
private Stack<UIController> showUIStack = new Stack<UIController>(4);
```
`Stack`을 사용하여 `가장 최근`에 Show된 객체를 Hide하는 `모바일 친화적` 방식입니다.

```
public T Show<T>(string name, int sortOrder = 0) where T : MainUIBase
{
    T ui = GetCachedUI<T>(name);
    ui.SetSortOrder(sortOrder);

    UIController uIController = GetUIController();
    uIController.Initialize(ui);
    uIController.Show();
    showUIStack.Push(uIController);
    return ui;
}

public void Hide()
{

    if (GetActiveUIController(out UIController uIController))
    {
        uIController.Hide();
        uIController.Release();
        controllerPool.Push(uIController);
    }
  
}

public void Close()
{
    if (GetActiveUIController(out UIController uIController))
    {
        uIController.Hide();
        uIController.Close();
        uIController.Release();
        controllerPool.Push(uIController);
    }
}

```
Show 하면 `캐싱된 데이터`가 없다면 `Resources 폴더` 내부에 `이름 기반`으로 로드해서 복제 합니다. `Stack`을 이용하기에 `Hide`, `Close` 할시 `가장 최근에` Show된 UI가 먼저 `Hide`, `Close` 가 됩니다.

```
public void UnloadUIs(Scene scene)
{
    while (showUIStack.Count > 0)
    {
        UIController uIController = showUIStack.Pop();
        uIController.Release();
        controllerPool.Push(uIController);
    }
}

```
`Scene 전환`시 `복제된 UI`들 `파괴`되기에 `Stack`을 `비움`으로써 `예외를 예방`합니다.

## IOSSafeArea 
```
void ApplySafeArea()
{
    Rect safeArea = Screen.safeArea;

    Vector2 anchorMin = safeArea.position;
    Vector2 anchorMax = safeArea.position + safeArea.size;

    if (ignoreBottom)
        anchorMin.y = 0.0f;

    anchorMin.x /= Screen.width;
    anchorMin.y /= Screen.height;
    anchorMax.x /= Screen.width;
    anchorMax.y /= Screen.height;



    rectTransform.anchorMin = anchorMin;
    rectTransform.anchorMax = anchorMax;
}
```
SafeArea의 RectTransform을 조절하여 하위 자식들의 캔버스 영역을 조절합니다. 

미리 준비된 템플릿을 제공합니다.
![ex](https://github.com/user-attachments/assets/59a14186-17bf-4358-9a39-2b3a8eeb6d36)
## BackgroundClickHandler
모바일 환경이나 PC 환경에서도 팝업 이미지가 뜬고난뒤 딤처리된 배경을 클릭할때 해당 최상위 팝업이 꺼지는 기능입니다. 
```
public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler 
{
    [SerializeField] UIBase cotnrolUIbase;

    private void Reset()
    {
        cotnrolUIbase = UIUtils.FindParentUIBase(transform as RectTransform);
    }

    private void Start()
    {
        if (cotnrolUIbase == null)
            Destroy(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cotnrolUIbase is ISubUI subUIBase)
        {
            subUIBase.Hide();
            return;
        }
        UIManager.Instance.Hide();
    }
}
```
IPointerClickHandler를 통한 클릭 감지후 Hide를 시도합니다.










# Collections
## PriorityQueue
유니티 C# 에서는 PriorityQueue가 지원을 하지 않기에 
