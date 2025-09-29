using UnityEngine;
using UnityEngine.UI;

public class DeckSelUI : MonoBehaviour
{
    [SerializeField] private Transform deckPanelRoot;     // DeckSelPanel들의 부모
    [SerializeField] private string panelNamePrefix = "DeckSelPanel";

    [SerializeField] private Button leftBtn;              // 인스펙터에서 연결
    [SerializeField] private Button rightBtn;             // 인스펙터에서 연결

    [SerializeField] private int deckIndex = 0;
    [SerializeField] private GameObject[] deckPanels;     // Awake에서 자동 채움

    [SerializeField] private Button BasicDeck;
    [SerializeField] private Button DiamondDeck;
    [SerializeField] private Button HeartDeck;
    [SerializeField] private Button SpadeDeck;
    [SerializeField] private Button ClubDeck;

    [SerializeField] private Button PlayBtn;

    [SerializeField] private Button BackToMenuBtn;

    private void Awake()
    {
        AutoRegisterPanels();
        leftBtn.onClick.AddListener(PrevDeck);
        rightBtn.onClick.AddListener(NextDeck);
        BasicDeck.onClick.AddListener(IsBasic);
        DiamondDeck.onClick.AddListener(IsDiamond);
        HeartDeck.onClick.AddListener(IsHeart);
        SpadeDeck.onClick.AddListener(IsSpade);
        ClubDeck.onClick.AddListener(IsClub);
        PlayBtn.onClick.AddListener(OnSelectDeck);
        BackToMenuBtn.onClick.AddListener(BackToMenu);
        UpdateView();
    }

    private void OnSelectDeck()
    {
        //테스트용 디버그 로그
        /*
        Debug.Log($"[{EventType.SelectDeck}] 선택 덱: " +
        (GameManager.TurnBased.turnSettingValue.IsBasicDeck ? "Basic" :
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck ? "Diamond" :
        GameManager.TurnBased.turnSettingValue.IsHeartDeck ? "Heart" :
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck ? "Spade" :
        GameManager.TurnBased.turnSettingValue.IsClubDeck ? "Club" :
        "None"));
        */

        GameManager.Event.Publish(EventType.SelectDeck);
        GameManager.SceneLoad.LoadScene(SceneType.Test);
    }
    private void PrevDeck()
    {
        deckIndex = (deckIndex - 1 + deckPanels.Length) % deckPanels.Length;
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = false;
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = false;
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = false;
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = false;
        GameManager.TurnBased.turnSettingValue.IsClubDeck = false;
        UpdateView();
    }

    private void NextDeck()
    {
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = false;
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = false;
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = false;
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = false;
        GameManager.TurnBased.turnSettingValue.IsClubDeck = false;
        deckIndex = (deckIndex + 1) % deckPanels.Length;
        UpdateView();
    }

    private void BackToMenu()
    {
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = false;
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = false;
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = false;
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = false;
        GameManager.TurnBased.turnSettingValue.IsClubDeck = false;
        gameObject.SetActive(false);
    }

    private void IsBasic()
    {
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = true;
        //사실 다른 덱들의 불값도 false로 해줘야함(상호배제)
        //이 버튼을 눌렀을때 해당 데이터다라고 인식시켜야함
    }

    private void IsDiamond()
    {
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = true;
    }

    private void IsHeart()
    {
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = true;
    }

    private void IsSpade()
    {
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = true;
    }

    private void IsClub()
    {
        GameManager.TurnBased.turnSettingValue.IsClubDeck = true;
    }

    private void AutoRegisterPanels()
    {
        var root = deckPanelRoot != null ? deckPanelRoot : transform;
        var list = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name.StartsWith(panelNamePrefix))
                list.Add(c.gameObject);
        }
        deckPanels = list.ToArray();
    }

    private void UpdateView()
    {
        for (int i = 0; i < deckPanels.Length; i++)
            deckPanels[i].SetActive(i == deckIndex);
    }
}