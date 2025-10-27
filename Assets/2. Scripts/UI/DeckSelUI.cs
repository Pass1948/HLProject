using System;
using DG.Tweening;
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
    [SerializeField] private GameObject titleUI;

    [SerializeField] private Button BasicDeck;
    [SerializeField] private Button DiamondDeck;
    [SerializeField] private Button HeartDeck;
    [SerializeField] private Button SpadeDeck;
    [SerializeField] private Button ClubDeck;
    [SerializeField] private Button gamePlayButton;

    [SerializeField] private Button BackToMenuBtn;
    
    private AudioClip selectedClip;

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
        BackToMenuBtn.onClick.AddListener(BackToMenu);
        gamePlayButton.onClick.AddListener(OnGameStart);
        UpdateView();
        selectedClip = GameManager.Resource.Load<AudioClip>(Path.Sound + "LOAD_CASSETTE_08");
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
        if(!ISSelectDeck())
        {
            Debug.Log("Select Your Deck!");
            return;
        }
        GameManager.SceneLoad.LoadScene(SceneType.Test);
        GameManager.Event.Publish(EventType.SelectDeck);
        GameManager.Sound.PlaySfx(selectedClip);
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
        GameManager.Sound.PlayUISfx();
    }

    public void OnGameStart()
    {
        if(!ISSelectDeck())
        {
            Debug.Log("Select Your Deck!");
            return;
        }
        GameManager.TurnBased.turnSettingValue.isDeck = false;
        GameManager.UI.OpenUI<FadeInUI>();
        GameManager.Map.NormalStage();
        GameManager.SceneLoad.LoadScene(SceneType.Test);
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
        GameManager.Sound.PlayUISfx();
    }

    private void BackToMenu()
    {
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = false;
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = false;
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = false;
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = false;
        GameManager.TurnBased.turnSettingValue.IsClubDeck = false;
        gameObject.transform.DOMove(new Vector3(2400f, 530f, 0f), 0.8f);
        titleUI.gameObject.transform.DOMove(new Vector3(1300f, 530f, 0f), 0.8f);
        GameManager.Sound.PlayUISfx();
    }

    private void IsBasic()
    {
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = true;
        //사실 다른 덱들의 불값도 false로 해줘야함(상호배제)
        //이 버튼을 눌렀을때 해당 데이터다라고 인식시켜야함
        GameManager.Sound.PlayUISfx();
    }

    private void IsDiamond()
    {
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsHeart()
    {
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsSpade()
    {
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsClub()
    {
        GameManager.TurnBased.turnSettingValue.IsClubDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private bool ISSelectDeck()
    {
        var IsSelected = GameManager.TurnBased.turnSettingValue;
        GameManager.Sound.PlayUISfx();
        return IsSelected.IsBasicDeck || IsSelected.IsDiamondDeck || IsSelected.IsHeartDeck || IsSelected.IsSpadeDeck || IsSelected.IsClubDeck;
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

    public struct Point
    {
        public int x, y;
    }


    
    
}






