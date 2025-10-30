using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Analytics;
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

    private void OnEnable()
    {
        Analytics.CustomEvent("deck_select_enter", new Dictionary<string, object> // TODO : deck_select_enter
        {
            { "///", "///" },
        });
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
        
        Analytics.CustomEvent("run_start_click", new Dictionary<string, object> // TODO: run_start_click
        {
            { "///", "///" },
        });
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
        gameObject.transform.DOLocalMove(new Vector3(2400f, 0f, 0f), 0.8f);
        titleUI.gameObject.transform.DOLocalMove(new Vector2(300, -24.92419f), 0.8f);
        GameManager.Sound.PlayUISfx();
    }

    private void IsBasic()
    {
        Analytics.CustomEvent("deck_select_confirm", new Dictionary<string, object> // TODO : deck_select_confirm
        {
            { "///", "///" },
        });
        GameManager.TurnBased.turnSettingValue.IsBasicDeck = true;
        //사실 다른 덱들의 불값도 false로 해줘야함(상호배제)
        //이 버튼을 눌렀을때 해당 데이터다라고 인식시켜야함
        GameManager.Sound.PlayUISfx();
    }

    private void IsDiamond()
    {
        Analytics.CustomEvent("deck_select_confirm", new Dictionary<string, object> // TODO : deck_select_confirm
        {
            { "///", "///" },
        });
        GameManager.TurnBased.turnSettingValue.IsDiamondDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsHeart()
    {
        Analytics.CustomEvent("deck_select_confirm", new Dictionary<string, object> // TODO : deck_select_confirm
        {
            { "///", "///" },
        });
        GameManager.TurnBased.turnSettingValue.IsHeartDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsSpade()
    {
        Analytics.CustomEvent("deck_select_confirm", new Dictionary<string, object> // TODO : deck_select_confirm
        {
            { "///", "///" },
        });
        GameManager.TurnBased.turnSettingValue.IsSpadeDeck = true;
        GameManager.Sound.PlayUISfx();
    }

    private void IsClub()
    {
        Analytics.CustomEvent("deck_select_confirm", new Dictionary<string, object> // TODO : deck_select_confirm
        {
            { "///", "///" },
        });
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






