using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TutorialUI : PopUpUI
{
    [System.Serializable]
    public struct TutorialPage
    {
        public string iconPath;  // 예: Path.UISprites + "Bullet_01"
        public string title;
        public string desc;
    }

    public enum Topic
    {
        Bullet, BikeControl, Move, MonsterInfo, Kick, BossInfo, Reload
    }

    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private TMP_Text descTMP;
    [SerializeField] private TMP_Text pageTMP;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;

    private const int LinesPerPage = 2; // 2줄 기준 자동 분할
    private List<TutorialPage> _pages = new List<TutorialPage>();
    private int _index;

    // ── 추가: OpenPopUI가 void 여도 Find 없이 토픽 전달하는 용도
    private static bool _hasPending;
    private static Topic _pendingTopic;

    private void Awake()
    {
        leftButton.onClick.AddListener(PrevPage);
        rightButton.onClick.AddListener(NextPage);
        closeButton.onClick.AddListener(CloseUI);
    }

    public override void CloseUI()
    {
        if (GameManager.Stage.stageId == 7001)
        {
            //TODO: tutorial1_popup_close
            CustomEvent customEvent = new CustomEvent("tutorial1_popup_close")
        {
            { "onScreen", "튜토리얼 1 시작 팝업 닫기"}
        };
            AnalyticsService.Instance.RecordEvent(customEvent);

        }
        if (GameManager.Stage.stageId == 7002)
        {

            //TODO: tutorial2_popup_close
            CustomEvent customEvent = new CustomEvent("tutorial2_popup_close")
        {
            { "onScreen", "튜토리얼 2 시작 팝업 닫기"}
        };
            AnalyticsService.Instance.RecordEvent(customEvent);
        }
        base.CloseUI();
    }

    public void Init(params Topic[] topics)
    {
        _pages.Clear();
        foreach (var t in topics)
        {
            _pages.AddRange(BuildPagesFor(t));
        }
        _index = 0;
        ApplyPage();
    }

    // ── 추가: 외부에서 한 줄로 열기 (SettingUI는 이거만 호출)
    public static void OpenWith(Topic topic)
    {
        _pendingTopic = topic;
        _hasPending = true;
        GameManager.UI.OpenPopUI<TutorialUI>(); // 생성+활성화는 UI 매니저가 담당
    }

    // ── 추가: 팝업이 실제로 열릴 때 pending 토픽으로 초기화
    protected override void OnOpen()
    {
        base.OnOpen();
        if (_hasPending)
        {
            Init(_pendingTopic);
            _hasPending = false;
            if(GameManager.Stage.stageId == 7001)
            {
                //TODO: tutorial1_popup_show
                CustomEvent customEvent = new CustomEvent("tutorial1_popup_show")
        {
            { "onScreen", "튜토리얼 1 시작 팝업 출력"}
        };
                AnalyticsService.Instance.RecordEvent(customEvent);
            }
            if (GameManager.Stage.stageId == 7002)
            {

                //TODO: tutorial2_popup_show
                CustomEvent customEvent = new CustomEvent("tutorial2_popup_show")
        {
            { "onScreen", "튜토리얼 2 시작 팝업 출력"}
        };
                AnalyticsService.Instance.RecordEvent(customEvent);
            }
        }
    }

    private void NextPage()
    {
        if (_index < _pages.Count - 1)
        {
            _index++;
            ApplyPage(); // 2/N, 3/N ...
        }
    }

    private void PrevPage()
    {
        if (_index > 0)
        {
            _index--;
            ApplyPage(); // N-1/N ...
        }
    }

    private void ApplyPage()
    {
        if (_pages == null || _pages.Count == 0)
        {
            // 빈 데이터 안전 처리
            tutorialImage.sprite = null;
            titleTMP.text = string.Empty;
            descTMP.text = string.Empty;
            pageTMP.text = string.Empty;
            return;
        }

        var p = _pages[_index];

        // 이미지(리소스 매니저)
        tutorialImage.sprite = GameManager.Resource.Load<Sprite>(p.iconPath);

        // 텍스트
        titleTMP.text = p.title;
        descTMP.text = p.desc;

        // 1/N 인디케이터
        pageTMP.text = $"{_index + 1}/{_pages.Count}";
    }

    // ──────────────────────────────────────────────────────────────
    // 주제→페이지 목록 (표 원문을 2줄씩 자동 분할; 제목/이미지 고정)
    // ──────────────────────────────────────────────────────────────
    private List<TutorialPage> BuildPagesFor(Topic topic)
    {
        string P = Path.UISprites; // 예: "UI/Sprites/"

        switch (topic)
        {
            case Topic.Bullet:
                return BuildPages(
                    P + "Bullet_01", "탄환",
            @"            탄환에는 고유한 문양과 숫자가 있습니다.
            문양에 따라 공격 범위가 달라집니다.
            탄환은 공격 시 플레이어의 공격력만큼 피해를 줍니다.
            상황에 맞게 탄환을 선택해 효율적으로 적을 공격해보세요."
                );

            case Topic.BikeControl:
                return BuildPages(
                    P + "Bike_01", "오토바이 조작 · 수리",
            @"            오토바이 조작 버튼을 클릭하면 다양한 기능을 사용할 수 있습니다.
            ‘수리’ 기능은 고장난 오토바이의 체력을 1 회복시켜 다시 기능할 수 있게 합니다.
            오토바이에 탑승하면 이동 거리가 크게 증가하지만,
            충격에 의해 쉽게 파괴될 수 있으니 피격당하지 않도록 주의하세요."
                );

            case Topic.Move:
                return BuildPages(
                    P + "Move_01", "이동",
            @"            이동 버튼을 클릭하면, 플레이어가 가진 이동 거리만큼 전진합니다.
            이동은 공격, 발차기, 수리를 하기 전에 한 턴에 한 번만 사용할 수 있습니다."
                );

            case Topic.MonsterInfo:
                return BuildPages(
                    P + "Monster_01", "몬스터의 정보와 속성",
            @"            몬스터를 클릭하면 상세 정보를 확인할 수 있습니다.
            ‘High’ 몬스터는 자신보다 높은 숫자의 탄환으로,
            ‘Low’ 몬스터는 낮은 숫자의 탄환으로 공격해야 피해를 입힙니다."
                );

            case Topic.Kick:
                return BuildPages(
                    P + "Kick_01", "발차기",
            @"            발차기 버튼을 사용해 몬스터를 강하게 공격할 수 있습니다.
            타격을 받은 몬스터는 자세가 흐트러지며 속성이 뒤바뀝니다.
            보유한 탄환으로 처치할 수 없는 몬스터는,
            발차기를 통해 공격 가능하게 만들 수 있습니다."
                );

            case Topic.BossInfo:
                return BuildPages(
                    P + "Boss_01", "보스 정보 확인",
            @"            보스 몬스터를 클릭하여 보스 몬스터의 추가 정보를 확인할 수 있습니다.
            보스 몬스터마다 고유의 패턴을 가지고 있어
            그 특징을 공략하는 것이 보스 공략의 기초입니다."
                );

            case Topic.Reload:
                return BuildPages(
                    P + "Reload_01", "재장전",
            @"            ‘재장전’ 기능은 현재 보유한 탄환을 모두 버리고,
            탄환집에서 새로운 탄환을 꺼내옵니다.
            남은 탄환이 없는데 적이 남아 있다면 게임에서 패배합니다.
            재장전은 신중하게 사용하세요."
                );
        }

        return new List<TutorialPage>();
    }

    // BuildPage : desc를 줄바꿈 기준으로 2줄씩 쪼개 페이지 리스트 생성
    // 코드에서 들여쓴 공백은 각 라인 Trim()으로 제거되어 UI에는 영향 없음
    private List<TutorialPage> BuildPages(string iconPath, string title, string desc)
    {
        var list = new List<TutorialPage>();

        if (string.IsNullOrWhiteSpace(desc))
        {
            list.Add(new TutorialPage { iconPath = iconPath, title = title, desc = string.Empty });
            return list;
        }

        var lines = desc.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i += LinesPerPage)
        {
            int take = Mathf.Min(LinesPerPage, lines.Length - i);

            string chunk = lines[i].Trim(); // ← 앞/뒤 공백 제거
            for (int j = 1; j < take; j++)
                chunk += "\n" + lines[i + j].Trim();

            list.Add(new TutorialPage { iconPath = iconPath, title = title, desc = chunk });
        }

        return list;
    }

    public  void OpenTR1()
    {
        GameManager.UI.OpenPopUI<TutorialUI>();
        var ui = GameManager.UI.GetPopUI<TutorialUI>();
        ui?.Init(Topic.Bullet, Topic.BikeControl, Topic.Move, Topic.MonsterInfo);
    }

    public  void OpenTR2()
    {
        GameManager.UI.OpenPopUI<TutorialUI>();
        var ui = GameManager.UI.GetPopUI<TutorialUI>();
        ui?.Init(Topic.Kick, Topic.BossInfo, Topic.Reload);
    }
}