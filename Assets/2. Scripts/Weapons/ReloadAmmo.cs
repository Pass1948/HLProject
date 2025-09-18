using UnityEngine;
using UnityEngine.UI;

public class ReloadAmmo : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private AttackController magazine;
    [SerializeField] private bool autoReload = true;

    [Header("Optional: Deck List UI")]
    [SerializeField] private RectTransform deckBg;//덱 표시되는 Bg

    void Start()
    {
        if (autoReload)
        {
            Reload();
        }
        else
        {
            RefreshDeckUI();
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

        magazine.ClearMagazine();

        if (deck != null)
        {
            int need = magazine.Capacity;
            var draw = deck.DrawAmmos(need);
            magazine.AddBullets(draw);
        }

        RefreshDeckUI();
    }

    //덱 UI 새로고침
    private void RefreshDeckUI()
    {
        if (deckBg == null) 
        {
            return;
        }
        

        ClearChild(deckBg);

        if (deck != null)
        {
            foreach (var a in deck.GetDrawSnapshot())
            {
                SpawnDeckItem(deckBg, a);
            }
        }
    }

    // 프리팹 생성
    private void SpawnDeckItem(RectTransform parent, Ammo ammo)
    {
        GameObject go = GameManager.Resource.Create<GameObject>(Path.Weapon + "Bullet", parent);
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



