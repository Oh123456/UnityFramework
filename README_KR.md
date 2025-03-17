# 유니티 프레임 워크
## 개요
이 프로젝트는 Unity에서 사용 가능한 프레임워크들을 모아둔것이다.
## 목차
- 어드레서블 시스템


## 어드레서블 시스템
### 개요
어드레서블 시스템을 쉽게 사용할수있게 도와주는 프레임워크이다.

### AddressableManager
어드레서블시스템을 관리하는 매니저
&nbsp;
&nbsp;
```csharp
public event Action OnCompletedLoad;
```
한 라벨의 모든 로드가 끝났을때 호출이 된다. 
```csharp
public event Action<AddressableDownLoadData> OnDownloadDependencies;
```
한 라벨의 다운로드 크기(Addressables.GetDownloadSizeAsync) 함수 호출후 호출이된다.
```csharp
public event Action<AddressableDownLoadData> OnDownload;
```
한 라벨의 다운로드(Addressables.DownloadDependenciesAsync)가 시작이 되면 호출이된다.

```csharp
    public struct AddressableDownLoadData
    {
        public AsyncOperationHandle handle;
        public string label;
    }
```
어떤 라벨이 다운로드가 되는지 추적을 도와주는 구조체이다.
