<a href="https://github.com/Oh123456/UnityFramework/releases"><img src="https://img.shields.io/badge/Release-0ABF53?style=flat-square&logo=GitHub_Pages&logoColor=white"/></a>

# UnityFramework
유니티 게임 개발에 있어 자주 사용되거나 제공되지 않는 컨테이너를 다른 프로젝트에서 재사용을 위해 제작 했습니다. 

# 목차
- ㅁㄴㅇㄹ

# UI <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/UIManager"><img src="https://img.shields.io/badge/GitHub_Pages-222222?style=flat-square&logo=GitHub&logoColor=white"/></a>
유니티 모바일 환경에 알 맞는 UI를 관리 밎 최적화에 초점을 둔 프레임 워크입니다. 

추후에 PC환경에도 대응할수있게 보완할 예정입니다.

AddressableSystem과 연동하여 사용하는 방법은 AddressableSystem 설명 페이지를 참고해주세요.

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

> ${\textsf{\color{#FF9800}※ 계층이 적은경우에는 SubUI를 사용하는게 성능이 안 좋을수있습니다.}}$  

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

>미리 준비된 템플릿을 제공합니다.
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


# CoroutineManager <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/Coroutine"><img src="https://img.shields.io/badge/GitHub_Pages-222222?style=flat-square&logo=GitHub&logoColor=white"/></a>
유니티에 강력한 기능중하나인 `코루틴(Coroutine)`을 `잘 못 사용`하거나 각각의 객체마다 YieldInstruction을 생성하는것은 `메모리 낭비`와 `GC 부담`이 커질수있기에 `보안`하기위해서 만들었습니다. 

${\textsf{\color{#1589F0}namespace}}$  `UnityFramework.CoroutineUtility`
```
readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
Dictionary<float, WaitForSeconds> waitForSecondDictionary = new Dictionary<float, WaitForSeconds>();
```
자주 사용되는것들은 `캐싱`하여 사용하여 GC부담과 메모리 낭비를 최소화합니다.

# Pooling <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/Pooling"><img src="https://img.shields.io/badge/GitHub_Pages-222222?style=flat-square&logo=GitHub&logoColor=white"/></a>
유니티에서는 게임진행중에 발생되는 `GC`가 `매우 치명적`이고 객체 `생성`과 `파괴`에 많은 `비용`이 들기에 Pooling 시스템은 필수입니다. 

해당 프레임워크에서는 `Class`, `Mono`, `ArrayPool(C# 기본 제공 기능)`이 제공 됩니다.

각 풀은 Dictionary 기반으로 `풀(Pool)`들을 관리합니다.

${\textsf{\color{#1589F0}namespace}}$  `UnityFramework.Pool`
## PoolManager
각각의 Pool를 사용할수있게해주는 매니저 입니다.

```
// Class
public static T GetClassObject<T>(bool isAutoActivate = true) where T : class, IPoolObject, new()
public static void SetClassObject(IPoolObject poolObject, bool isAutoDeactivate = true)

//Mono
public static T GetMonoObject<T>(IMonoPoolObject prefab, Transform parents = null,bool isAutoActivate = true) where T : MonoBehaviour, IMonoPoolObject, new()
public static void SetMonoObject<T>(IMonoPoolObject poolObject, bool isAutoDeactivate = true) where T : MonoBehaviour, IMonoPoolObject

//Array
public static ArrayPoolObject<T> GetArray<T>(int size)
public static void SetArray<T>(ref ArrayPoolObject<T> arrayPoolObject)
```
`isAutoActivate`, `isAutoDeactivate` 옵션은 Get,Set 이 이루워질때 `자동`으로 `활성화`,`비활성화` 옵션입니다. 

### PoolKey

```
public struct PoolKey : System.IEquatable<PoolKey>
{

	private readonly System.Type typeKey;
	private readonly MonoBehaviour prefab;

...
```
구조체를 사용하기에 `GC부담없이` `키 검사`가 가능합니다. 기본적을 클래스 `타입(Type)`으로 `Key`를 관리합니다. MonoPool일경우 `프리팹(Prefab)`까지 같은지 검사합니다.

### Pool
```
public abstract class Pool
{
protected Stack<IPoolObject> objects = new Stack<IPoolObject>(4);

public IPoolObject GetObject()
{
    IPoolObject poolObject = null;
    bool isValid = true;
    while (isValid)
    {
        if (objects.Count == 0)
            poolObject = CreateObject();
        else
            poolObject = objects.Pop();
        // 혹시라도 생성되있는애가 풀에 들어와 있을경우
        // 혹은 오브젝트가 널이라면
        isValid = poolObject == null ? true : poolObject.IsValid();
    }
    return poolObject;
}

public void SetObject(IPoolObject classObject)
{
    objects.Push(classObject);
}

...
```
`GetObject` 와 `SetObject`를 통하여 오브젝트를 Pooling 할수 있습니다.

### IPoolObject
```
public interface IPoolObject
{
    /// <summary>
    /// 객체가 활성화 되있는지 혹은 존재하는지
    /// </summary>
    /// <returns></returns>
    public bool IsValid();
    /// <summary>
    /// 객체 활성화
    /// </summary>
    public void Activate();
    /// <summary>
    /// 객체 비활성화
    /// </summary>
    public void Deactivate();
}
```
오브젝트 풀링을 위한 인터페이스입니다.

> 해당 프레임 워크의 Pooling은 `IPoolObject` 인터페이스 `기반`으로 `작동`하기에 Pooling할 Class에 상속이 없을시 해당 프레임 워크의 풀링 시스템을 사용할수 없습니다

### IMonoPoolObject
`GameObject`를 위한 인터페이스 입니다.
```
public interface IMonoPoolObject : IPoolObject
{
    public int KeyCode { get; set; }
}
```

`KeyCode`는 `원본`의 프리팹과 타입의 정보를 `해싱`한 값이 있어 `복사본`으로 `반환`이 가능합니다.


> [ClassPoolObject](https://github.com/Oh123456/UnityFramework/blob/main/Assets/Framework/Pooling/PoolObject.cs), [MonoPoolObject](https://github.com/Oh123456/UnityFramework/blob/main/Assets/Framework/Pooling/PoolObject.cs) 등 상위 클래스들이 준비가 되있습니다.
## ClassPool
C# 에서는 `레퍼런스 타입`들은 모두 `힙`에 할당되고 `GC` 대상이기에 자주생성되는 `Class` 마저도 `Pooling`을 해주면 성능향상에 도움이 됩니다.

## MonoPool
유니티에서 사용되는 게임 오브젝트(GameObject)을 생성하고 재사용하는 풀입니다. 유니티에서 기초적으로 사용되는 풀입니다.

## ArrayPool
C# 에서 기본재공되는 ArrayPool기을을 사용하기 쉽게 가공한것입니다.

```
public struct ArrayPoolObject<T> : System.IDisposable
{
	T[] array;

	
        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool clearArray)
        {
            ArrayPool<T>.Shared.Return(array, clearArray: clearArray);
            array = null;
        }
...
```

```
ArrayPoolObject<int> array = PoolManager.GetArray<int>(20);
for(int i = 0; i < 20; i++)
	array[i] = i;
//배열을 ArrayPool<T> 에 반환
array.Dispose();
```

`구조체`와 `IDisposable` 사용하여 `GC` 부담 적고 쉽게 `Pool`에 배열을 `반환`할수있습니다.

# FSM <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/FSM"><img src="https://img.shields.io/badge/GitHub_Pages-222222?style=flat-square&logo=GitHub&logoColor=white"/></a>

AI, 혹은 캐릭터의 상태를 관리할수있는 상태 머신입니다.

${\textsf{\color{#1589F0}namespace}}$  `UnityFramework.FSM`

## StateMachine
State관리하는 상태 머신입니다.

### StateMachineData
```
//상태머신 데이터 구조체
public struct StateMachineData
{
    public object owner;
    public Dictionary<int, State> states;
    public State currentState;
    public int defaultID;
}

```
Dictionary int Key 기반으로 State를 관리합니다. 

#### AddState
```
// 상태 추가
public void AddState(State state)
{
    if (stateMachineData.states.TryAdd(state.ID, state))
        state.SetOwnerMachine(this);
}
```
> 중복으로 키가 들어올경우 무시합니다.

#### ChangeState
```
//상태 변경
public void ChangeState(int id)
{
    // 같은 ID 제외
    if (CurrentID == id)
        return;

    // 현재 상태에서 변환 이 가능한지 검사
    if (!stateMachineData.currentState.ConditionChangeID(id))
        return;

    // 상태 존재 검사
    if (!stateMachineData.states.TryGetValue(id, out State nextState))
        return;

    stateMachineData.currentState?.Exit();
    stateMachineData.currentState = nextState;
    stateMachineData.currentState.Enter();
}
```
State가 변경시 해당 State에서 변경할려는 State로 변경이 가능한지 검사후 변경이 진행됩니다. 

#### Update
```
public abstract class StateMachine : IStateMachine
{

public void Update()
{
    stateMachineData.currentState.Update();
}

...
```
기본적으로 일반 클래스이기에 Mono의 Update에서 호출이 되야합니다.

# Collections <a href="https://github.com/Oh123456/UnityFramework/tree/main/Assets/Framework/Collections"><img src="https://img.shields.io/badge/GitHub_Pages-222222?style=flat-square&logo=GitHub&logoColor=white"/></a>
## PriorityQueue
유니티 C# 에서는 PriorityQueue가 지원을 하지 않기에 
