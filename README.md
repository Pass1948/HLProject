# THE_DARK_HOLLOW

<img width="700" height="600" alt="image" src="https://github.com/user-attachments/assets/629ea0a2-1681-4d1b-8270-0d2f9a0ea22f" />

## 🎮 게임 소개
**『The Dark Hollow』**는 어둠에 숨어든 악마를 쓰러뜨리는 2D 플랫폼 액션 게임입니다.


## 🖼️ 게임 화면
<p>게임 플레이 화면입니다.</p>
<img src="https://github.com/user-attachments/assets/05026fd2-ae08-45b9-9d16-79ac197f4749" width="600"/>
<img src="https://github.com/user-attachments/assets/bfc54d08-9ddd-4073-8da8-5d27be39827b" width="600"/>
<img src="https://github.com/user-attachments/assets/994c7aff-935a-4815-aa35-cb7d5e57ba56" width="600"/>


## 🕹️ 플레이 방법

(게임 흐름)
- 타이틀 씬에서 게임 시작 버튼을 눌러서 게임을 시작합니다.
- 이동과 점프, 공격 키를 입력해서 몬스터를개
<p>팀장 : 정세윤 - 플레이어 구현(움직임, 상태 패턴, 애니메이션 구현) </p>
<p>팀원 : 이영신 - 보스 구현(움직임, 상태 패턴, 애니메이션 구현)</p>
<p>팀원 : 고태웅 - 씬, 오브젝트, 카메라, 적, 맵, 리소스, 사운드 매니저 구현</p>
<p>팀원 : 차광호 - UI 개발(시작 화면, 옵션, 키 설정, 인게임)</p>
<p>팀원 : 김예성 - 시스템 기획(플레이어), UI 기획(각종 화면 디자인)</p>
<p>팀원 : 박철원 - 콘텐츠 기획(레벨 디자인 및 일반, 보스 몬스터 구현)</p>


## 🧰 개발 환경
- **Engine**: Unity 2022.3.17f1 (LTS)
- **Language**: C#
- **IDE**: JetBrains Rider / Visual Studio 2022
- **Target**: Windows (PC) *(선택적으로 Android/iOS 확장 가능)*
- **Version Control**: Git + GitHub

---
## 🎀 플로우 차트
- <img width="1590" height="868" alt="image" src="https://github.com/user-attachments/assets/d46abab3-b1c6-4ecc-9de2-ce83c96a4584" />


## 🧩 게임 주요 기능

### 1) 플레이어 상태 관리, 애니메이션 
- FSM과 상태패턴으로 플레이어의 상태를 관리. 
- 추상 클래스(BaseState)로 모든 상태들이 가져야 할 기능들을 정의, 구체적인 클래스(Idle/Move/Run/Jump/Hit State)에서 추상 클래스를 정의, 상태 전환 조건에 따라 상태를 전환
- 모든 상태들을 상태 머신(StateMachine) 클래스로 관리 
- 플레이어의 모든 애니메이션을 AnimationController에서 관리

### 2) 모든 오브젝트 동적생성
- Addressable를 사용하여 게임내 동적생성 오브젝트 관리
- 또한 Scene에 들어가는 모든 오브젝트를 Prefab화 하여 관리

### 3) 보스의 다양한 패턴
- HFSM을 이용한 공격패턴 세부화 : 돌진공격, 중거리공격, 장거리공격
- 중거리공격<br><img width="300" height="200" alt="image" src="https://github.com/user-attachments/assets/b8bff08d-86e7-425f-a5d9-8ed0e471ecc0" />
- 장거리공격<br><img width="100" height="200" alt="image" src="https://github.com/user-attachments/assets/15dd45ba-faac-478b-a801-3158580245c3" />

### 4) UI
- Audio Mixer로 BGM과 SFX를 분리
- 옵션 클릭시 일시정지 처리
- PlayerInput과 드롭다운 연동으로 키 가이드를 간단히 재매핑
- 특수 게이지 테두리 변화로 사용 가능 상태를 즉시 인지하도록 설계
