# Like Tilt To Live : 아케이드 생존 게임

이 프로젝트는 과거 모바일 아케이드 게임 **'Tilt To Live'**를 Unity로 직접 재현해본 모작 프로젝트입니다. 

## 게임 방식

이 게임에는 플레이어가 직접 쓸 수 있는 '무기'가 없습니다. 오직 플레이어를 쫓아오는 빨간 점들을 아슬아슬하게 피하는 **'회피'**가 가장 중요한 생존 방식입니다. 

### 1. 조작 방식
**'PC'** : 마우스 커서를 따라 캐릭터가 부드럽게 회전하며 이동합니다.
**'모바일'** : 스마트폰의 자이로스코프 센서를 이용해, 기기를 기울이는 방향으로 회전하며 이동합니다. 원작의 '기울여서 살아남는' 조작감을 살렸습니다.

### 2. 적
- 게임이 시작되면 적들이 플레이어를 향해 몰려듭니다.
- 시간이 지날수록 적들은 더 빨라지고, 한번에 등장하는 숫자도 늘어납니다. 때로는 플레이어 주변을 둥글게 포위하기도 하고, 거대한 벽이 되어 밀고 내려오기도 합니다.

### 3. 아이템
- 맵 곳곳에 무작위로 등장하는 아이템으로 반격이 가능합니다.
  - **폭탄** : 획득하는 순간 폭발이 일어나 해당 범위의 적을 파괴합니다.
  - **빙결** : 적들을 얼어붙여 제자리에 멈춥니다. 위급한 순간을 벗어 날 수 있습니다.

## 개발 방식

게임을 만들면서 가장 중요하게 생각했던 것은 **"수많은 적이 나와도 렉(Lag)이 걸리지 않게 하자"**와 **"코드가 복잡하게 꼬이지 않도록 규칙을 잘 정해두자"**였습니다. 

### 1. 수많은 적을 렉 없이 움직이는 방법
화면에 2,000개가 넘는 적들이 단순히 생성 삭제만 반복하면, 컴퓨터(또는 스마트폰)는 메모리를 지우고 새로 만드는 과정에서 심한 렉을 겪게 될 것입니다.
- 이를 해결하기 위해 게임이 시작되기 전 로딩 화면에서 **2,000개의 적을 미리 다 만들어두고 숨겨두는 방식(Object Pooling)**을 썼습니다.
- 또한, 적이 죽어서 리스트에서 빠질 때마다 뒤에 있는 데이터들을 앞으로 한 칸씩 당겨오는 대신, **맨 끝에 있는 데이터를 지워진 자리로 옮겨오는 방식(Swap-back 기법)**을 써서 연산 속도를 줄였습니다.

<details>
<summary>💻 관련 코드 보기 (Swap-back)</summary>

```csharp
// [EnemyManagement.cs]
if (enemy.IsDead)
{
    EnemyObjectPoolProvider.Instance.ReturnObject(enemy.gameObject);

    // 맨 마지막 원소를 지금 지울 자리로 덮어씌워 버립니다.
    int lastIndex = _activeEnemies.Count - 1;
    _activeEnemies[i] = _activeEnemies[lastIndex];
    _activeEnemies.RemoveAt(lastIndex); 
}
```
</details>

### 2. MVP 패턴을 사용하여 UI와 게임 로직 분리
보통 게임을 만들다 보면 '버튼을 눌렀을 때 작동하는 코드'와 '실제 게임의 점수가 오르는 코드'가 한곳에 뒤섞여 나중에는 손대기 어려울 정도로 복잡해지곤 합니다.
- 이 문제를 막기 위해 화면에 보여지는 부분(View)과 실제 데이터(Model), 그리고 그 둘 사이를 연결해주는 중간 다리(Presenter)를 철저하게 나누는 **MVP 아키텍처**를 도입했습니다. 
- 덕분에 UI 디자인을 바꾸거나 새로운 메뉴를 추가할 때, 실제 게임 코드는 전혀 건드리지 않아도 되는 깔끔한 구조가 되었습니다.

<details>
<summary>💻 관련 코드 보기 (MVP State Machine)</summary>

```csharp
// [PresenterProvider.cs] 상태 머신을 활용한 뷰(View) 전환
public void ChangeState(UI_STATE nextState)
{
    _currentPresenter?.Close(); // 이전 프레젠터는 닫기
    
    if(_dic_Presenter.TryGetValue(nextState, out APresenter nextPresenter))
    {
        _currentPresenter = nextPresenter;
        _currentPresenter.Open(); // 새로운 프레젠터 열기
    }
}

//[InGame_Presenter.cs] 실제 프레젠터
public InGame_Presenter(InGame_View view , PresenterProvider _provider , GameRootManager rootManager)
{
    _view = view; // View 주입받기
    _presenterProvider = _provider;
    _rootManager = rootManager;

    _model = new InGame_Model();
}

public override void Open()
{
    _model.ResetData();
    _view.UpdateTimer(_model.SurviveTime);

    _view.Open(); // View 열기 (실제 UI 작동 로직)

    _cancellToken = new CancellationTokenSource();
    StartCountdown_async(_cancellToken.Token).Forget();
}

```
</details>

### 3. 적들의 등장 패턴을 쉽게 추가하기
적들이 동그랗게 감싸거나, 일자로 내려오는 등 다양한 패턴으로 등장하는데, 이 규칙들을 하나의 스크립트에 전부 적어두면 코드가 너무 길어집니다.
- 그래서 **'전략 패턴(Strategy Pattern)'**을 활용해, 새로운 스폰 패턴(예: V자 모양, 지그재그 모양 등)을 추가하고 싶을 때는 기존 코드를 수정할 필요 없이 새로운 패턴 블록만 하나 끼워 넣으면 바로 작동하도록 만들었습니다.

<details>
<summary>💻 관련 코드 보기 (Strategy Pattern)</summary>

```csharp
// [IEnemySpawnPattern.cs] 새로운 패턴 구현시 이 인터페이스만 구현하면 됩니다.
public interface IEnemySpawnPattern
{
    void CalculatePosition(int count, Vector3 center, float spacing, List<Vector3> results);
}

// 이렇게 원형 스폰 패턴, 일자형 스폰 패턴 등을 확장할 수 있습니다.
public class CirclePattern : IEnemySpawnPattern { /* ... */ }
```
</details>

### 4. 마우스와 스마트폰 기울기 동시 지원
PC에서 개발할 때는 마우스를 쓰고, 모바일에서 테스트할 때는 기기 기울기를 써야 했습니다.
- 플레이어 캐릭터 입장에서는 "내가 마우스로 조종받는지, 기울기로 조종받는지" 모르게 만들었습니다. 단지 **"어느 방향으로 가야 해"라는 데이터만 넘겨받도록 인터페이스(IInputProvider)**로 묶어두었기 때문에, 나중에 조이스틱 조작을 추가하더라도 캐릭터의 이동 코드는 수정할 필요가 없습니다.

<details>
<summary>💻 관련 코드 보기</summary>

```csharp
// [PlayerController.cs] 마우스인지 자이로인지 몰라도 됩니다.
private IInputProvider _inputProvider;

private void FixedUpdate()
{
    // 입력 장치가 무엇이든 상관없이, 인터페이스를 통해 '어디로 가야 하는지'만 받아옵니다.
    Vector2 targetPos = _inputProvider.GetTargetPosition(_rigid.position);
    MoveToTarget(_rigid.position, targetPos);
}
```
</details>

## 앞으로 추가해야 할 것들

### 1. 더 다양한 적의 스폰 패턴
현재는 일자 벽, 원형 패턴밖에 없지만 앞으로 원작 게임답게 플레이어 긴장감을 높이는 다채로운 패턴들을 더 추가해보고 싶습니다. 이미 전략 패턴으로 구조는 탄탄하게 짜두었기 때문에, 얼마든지 쉽게 구현할 수 있을 것 같습니다.

### 2. 더 다양한 아이템 확장
현재 아이템 외에도, 원작에서의 거대한 레이저, 톱니바퀴 같은 시원시원한 무기 아이템들을 추가할 계획입니다. 

### 3. UI 디자인 수정 및 편의 기능 확장
지금까지는 게임의 핵심 로직과 아키텍처를 만드는데 집중하느라 UI가 심플한 상태입니다. 앞으로는 게임의 분위기에 어울리는 세련된 배경과 버튼 이미지들을 적용해 시각적인 완성도를 한층 끌어올릴 예정입니다. 또한, 내가 얼마나 오래 버텼는지 기록을 확인할 수 있는 '최고 점수 시스템'이나 조작감을 세밀하게 조절할 수 있는 '상세 설정 창' 같은 편의 기능들도 현재 구축해둔 MVP 아키텍처 위에 차곡차곡 쌓아 올려 볼 생각입니다.