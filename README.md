# HIGHWAY&LOW

- <img width="700" height="600" alt="그림1" src="https://github.com/user-attachments/assets/68d22555-6afd-4752-8517-a0221c81074f" />

## 🎮 게임 소개
**『HIGHWAY&LOW』**는 성기사가 오토바이 타고 샷건으로 마왕을 잡기 위한 여정을 그린 3D 턴제로그라이크 덱빌딩 게임입니다.


## 🖼️ 게임 화면
<p>게임 플레이 화면입니다.</p>
- <img src="https://github.com/user-attachments/assets/7df0b24a-d2b1-4585-a91b-f0eb81d0adce" width="600"/>
- <img src="https://github.com/user-attachments/assets/9fdfdda9-fb06-4202-ab9c-d15c7f8d12c2" width="600"/>
- <img src="https://github.com/user-attachments/assets/45866d2f-0302-407e-a96e-747253371fb7" width="600"/>
- <img src="https://github.com/user-attachments/assets/a8fa2821-38e2-4375-9738-8e4fc6f39670" width="600"/>

## 🕹️ 플레이 방법

(게임 흐름)
- 타이틀 씬에서 '새로하기' 버튼을 눌러서 게임을 시작합니다.
- 게임이 시작전 덱을 골라서 진행합니다(튜토리얼 선택가능)
- 주어진 탄환들을 이용해서 화면 UI에 보여지는 탄환과 이동, 킥 버튼을 눌러서 격자맵에 있는 몬스터들을 전멸시키면 됩니다
- 몬스터들을 전부 처치시 보상획득과 상점으로 이동합니다

## 🧰 개발 환경
- **Engine**: Unity 2022.3.17f1 (LTS)
- **Language**: C#
- **IDE**: JetBrains Rider / Visual Studio 2022
- **Target**: Windows (PC) *(선택적으로 Android/iOS 확장 가능)*
- **Version Control**: Git + GitHub

---
## 🎀 턴플로우 차트
- <img width="678" height="1705" alt="TurnBasedHFSM_v0 2 drawio (1)" src="https://github.com/user-attachments/assets/e1c34aca-9132-4fee-af56-557300bf83df" />

## 🧩 게임 주요 기능

### 1) 턴시스템과 유닛 상태 관리, 애니메이션 
- HFSM과 FSM으로 턴시스템과 유닛들의 상태를 관리. 
- 추상 클래스(BaseState)로 모든 상태들이 가져야 할 기능들을 정의, 구체적인 클래스(Idle/Move/Run/Jump/Hit State)에서 추상 클래스를 정의, 상태 전환 조건에 따라 상태를 전환
- 모든 상태들을 상태 머신(StateMachine) 클래스로 관리 
- 유닛의 모든 애니메이션을 유닛별 Handler 클래스를 생성하여 관리

### 2) 모든 오브젝트 동적생성
- Resources폴더에서 주소값으로 불러오는 유니티 시스템기능을 사용하여 Manager제작하여 게임내 동적생성 오브젝트 관리
- 또한 Scene에 들어가는 대부분의 오브젝트를 Prefab화 하여 관리
- Scene전환 시 필요한 데이터나 생성관리를 DataManager, SceneManager에서 데이터 관리와 로딩순서 관리

### 3) 유닛들 공통으로 사용하는 네비게이션 시스팀(A*알고리즘, 다익스트라)
- A*와 다익스트라 알고리즘을 기용하여 유닛들이 타일맵위에서 대상을 탐색하고 경로설정하는 기능을 제작함
<img src="https://github.com/user-attachments/assets/4656abad-e944-4344-a2d1-c8f8eebc2b51" width="600"/>

### 4) UI
- UIManager를 제작하여 BaseUI를 통해 상속으로 분류 및 관리
- 옵션에서 게임속도조절과 게임 해상도크기 조절이 가능함
- 옵션에 튜토리얼 이후 플레이어가 게임 규칙에 익숙치 않을것을 대비하여 툴팁UI를 넣음
- 어느 Scene에서든 옵션은 킬수있음

- <img src="https://github.com/user-attachments/assets/64d3df34-deed-4f85-a4a8-467d4b04bf8b" width="600"/>
<img src="https://github.com/user-attachments/assets/71be619f-047c-4cd1-9f75-2d1a840e0d8b" width="600"/>


