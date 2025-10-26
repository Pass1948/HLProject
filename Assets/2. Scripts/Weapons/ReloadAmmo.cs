using System;
using UnityEngine;
using UnityEngine.UI;

public class ReloadAmmo : MonoBehaviour
{
    Deck deck;
    [SerializeField] private AttackController magazine;
    [SerializeField] private bool autoReload = true;

    [SerializeField] private RectTransform deckBg;//덱 표시되는 Bg

    private int maxReloads = 4;
    private int usedReloads = 0;

    public int Remaining => Mathf.Max(0, maxReloads - usedReloads);
    public bool CanReload => Remaining > 0;

    public event Action<int, int> ReloadChange;

    private void Awake()
    {
        deck = GetComponent<Deck>();
    }
    void Start()
    {
        FirstReload();
    }

    public void FirstReload()
    {
        if (deck != null)
        {
            if (GameManager.TurnBased.turnSettingValue.isTutorial == true)
                return;
            else
            {
                int need = magazine.Capacity;
                var draw = deck.DrawAmmos(need);
                magazine.AddBullets(draw);
            }
        }
    }

    public void Reload()
    {
        if (magazine == null)
        {
            Debug.LogError("Not found magazine");
            //TODO: 나중에는 에러를 지우고 UI 작업 필요(JBS)
            return;
        }

        if (magazine.IsBtnSel)
        {
            Debug.Log("탄환 선택 풀고 재장전하시오");
            ReloadChange?.Invoke(Remaining, maxReloads);
            return;
        }

        if (!CanReload)
        {
            Debug.LogWarning("탄약없음!");
            //탄약없을때도 UI갱신시키기
            ReloadChange?.Invoke(Remaining, maxReloads);
            return;
        }

        magazine.ClearMagazine();

        if (deck != null)
        {
                int need = magazine.Capacity;
                var draw = deck.DrawAmmos(need);
                magazine.AddBullets(draw);
        }

        //사용탄약 증가
        usedReloads = Mathf.Min(usedReloads + 1, maxReloads);

        RefreshDeckUI();
    }

    //덱 UI 새로고침
    public void RefreshDeckUI()
    {
        if (deckBg == null) 
        {
            //탄환집 클릭안하면 Bg가 안나오는데 남은 Reload숫자는 갱신해야하니까
            ReloadChange?.Invoke(Remaining, maxReloads);
            return;
        }

        ClearChild(deckBg);

        if (deck != null)
        {
            foreach (var a in GameManager.ItemControl.drawPile)
            {
                SpawnDeckItem(deckBg, a);
            }
        }

        ReloadChange?.Invoke(Remaining, maxReloads);
    }

    // 프리팹 생성
    private void SpawnDeckItem(RectTransform parent, Ammo ammo)
    {
        GameObject go = GameManager.Resource.Create<GameObject>(Path.UI + "Bullet", parent);
        var view = go.GetComponent<BulletView>();
        if (view)
        {
            view.ammo = ammo;
            view.RefreshLabel();
        }

        var btn = go.GetComponentInChildren<Button>(true);
        if (btn)
        {
            // 덱에 있는 탄환 클릭 불가
            btn.interactable = false;
        } 
    }

    private void ClearChild(RectTransform rt)
    {
        if (!rt)
        {
            return;
        }
        for (int i = rt.childCount - 1; i >= 0; i--)
        {
            Destroy(rt.GetChild(i).gameObject);
        }
            
    }
}



