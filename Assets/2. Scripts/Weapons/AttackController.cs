using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private RectTransform slotContainer;
    [SerializeField] private RectTransform discardBg;
    int attacked = 0;
    private RectTransform bullet;
    private Button selectedAmmoBtn;
    private Image selectBulletBg;

    private Color bgNormal = new Color(0f, 0f, 0f, 1f);
    private Color bgSel = new Color(1f, 0f, 0f, 1f);

    private AudioClip reroadSound;
    private AudioClip reroadCancelSound;

    [SerializeField] private int AmmoCount = 6;
    List<Ammo> handAmmoList = new List<Ammo>();



    public int Capacity => AmmoCount;
    public Ammo fireAmmo;
    //UI에서 쓰는 선택상태
    public bool IsBtnSel => (selectedAmmoBtn != null) && (bullet != null);

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.PlayerAttack, Fire);
        GameManager.Event.Subscribe(EventType.CancelAmmo, CancelAmmo);
        reroadSound = GameManager.Resource.Load<AudioClip>(Path.Sound + "CASSETTE_RATTLE_12");
        reroadCancelSound = GameManager.Resource.Load<AudioClip>(Path.Sound + "LOAD_CASSETTE_08");
        GameManager.Event.Subscribe(EventType.EmptyAmmo, EmptyAmmo);
        attacked = GameManager.Unit.Player.playerModel.attack;
    }
    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.PlayerAttack, Fire);
        GameManager.Event.Unsubscribe(EventType.CancelAmmo, CancelAmmo);
        GameManager.Event.Unsubscribe(EventType.EmptyAmmo, EmptyAmmo);
    }

    public void CancelAmmo()
    {
        if (selectBulletBg) selectBulletBg.color = bgNormal;
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;
    }


    //탄환버튼 OnClick
    public void SelectAmmo(Button btn)
    {
        GameManager.Unit.Player.playerModel.attack = attacked;
        if (!GameManager.Mouse.isShowRange) return;
        if (btn == null)
        {
            return;
        }
        
        //이미 사용된 슬롯 클릭 방지
        if (!btn.interactable)
        {
            return;
        }

        //같은 탄환을 다시 클릭시 선택해제
        if (selectedAmmoBtn == btn)
        {
            if (selectBulletBg) selectBulletBg.color = bgNormal;
            selectedAmmoBtn = null;
            selectBulletBg = null;
            bullet = null;
            
            GameManager.Map.attackRange.ClearAttackType();
            GameManager.Mouse.IsAttacking = false;
            GameManager.Sound.PlaySfx(reroadSound);
            return;
        }

        //이전 선택 복구
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }

        //배경 이미지 가져오고 저장
        //BulletView가 붙은 부모를 찾아 RectTransform을 정확히 집는다
        var slotView = btn.GetComponentInParent<BulletView>();
        var slotRoot = slotView ? (RectTransform)slotView.transform : null;
        var bg = slotView ? slotView.bulletBg : null;

        if(slotRoot == null)
        {
            return;
        }

        //디스카드쪽 선택 금지
        if(discardBg != null && slotRoot.parent == discardBg)
        {
            return;
        }

        selectedAmmoBtn = btn;
        bullet = slotRoot;
        selectBulletBg = bg;

        var selectAmmo = bullet.GetComponent<BulletView>();
        GameManager.TurnBased.turnSettingValue.fireAmmo = selectAmmo.ammo;
        // 버튼을 누르면 범위 선택
        BulletView bulletView = bullet.GetComponent<BulletView>();
        
        if (bulletView != null)
        {
            Suit suit = bulletView.ammo.suit;
            int rank = bulletView.ammo.rank;

            AddBulletAttack(bulletView);
            AddBulletAttack2(bulletView);
            AddBulletAttack3(bulletView);
            GameManager.Map.attackRange.SetAttackRange(suit, rank);
            GameManager.Mouse.HidePlayerRange();
            GameManager.Mouse.IsAttacking = true;
        }

        if (selectBulletBg)
        {
            selectBulletBg.color = bgSel;
        }
        GameManager.Sound.PlaySfx(reroadCancelSound);
    }
    // 탄환 속성별 효과 메서드
    void AddBulletAttack(BulletView bulletView)
    {
        var suit = bulletView.ammo.suit;
        int baseAttack = GameManager.Unit.Player.playerModel.attack;
        bool isBonusApplied =
      (GameManager.ItemControl.diamod && suit == Suit.Diamond) ||
      (GameManager.ItemControl.heart && suit == Suit.Heart) ||
      (GameManager.ItemControl.spade && suit == Suit.Spade) ||
      (GameManager.ItemControl.club && suit == Suit.Club);
        GameManager.Unit.Player.playerModel.attack = isBonusApplied ? baseAttack + 1 : baseAttack;
    }

    void AddBulletAttack2(BulletView bulletView)
    {
        var rank = bulletView.ammo.rank;
        int baseAttack = GameManager.Unit.Player.playerModel.attack;
        bool isBonusApplied =
      (GameManager.ItemControl.ace && rank == 1) ||
      (GameManager.ItemControl.king && rank == 13);
        GameManager.Unit.Player.playerModel.attack = isBonusApplied ? baseAttack + 2 : baseAttack;
    }

    void AddBulletAttack3(BulletView bulletView)
    {
        if(!GameManager.ItemControl.handSame) return;

            int baseAttack = GameManager.Unit.Player.playerModel.attack;
            for (int i = 0; i < handAmmoList.Count; i++)
            {
                if (bulletView.ammo == handAmmoList[i])
                {
                    baseAttack += 1;
                }
            }
            GameManager.Unit.Player.playerModel.attack = baseAttack;
    }


    public void Fire()
    {
        if (!selectedAmmoBtn || !bullet)
        {
            return;
        }

        //발사되고 배경색 초기화
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }

        var view = bullet.GetComponent<BulletView>();

        //디스카드랑 덱에서는 버튼 비활성
        var btn = bullet.GetComponentInChildren<Button>(true);
        if (btn)
        {
            btn.interactable = false;
        }
        if (view)
        {
            view.bulletBg.color = Color.black;
        }

        //발사된 탄환 Discard
        if (discardBg != null)
        {
            bullet.SetParent(discardBg, false);
            bullet.localScale = Vector3.one;
            view.RefreshLabel();
            for (int i = 0; i < handAmmoList.Count; i++)
            {
                if(view.ammo == handAmmoList[i])
                {
                    handAmmoList.Remove(handAmmoList[i]);
                }
            }
        }
        else
        {
            Debug.LogError("Not Found discardBg");
        }

        //상태 정리
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;
        Debug.Log("Fire");
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
    }

    //탄창 비우고 탄 리스트 반환
    public List<Ammo> ClearMagazine()
    {
        var result = new List<Ammo>();

        //선택된 탄환 색 되돌리기
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }
        
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            var slot = (RectTransform)slotContainer.GetChild(i);
            var view = slot.GetComponent<BulletView>();
            if (view && view.ammo != null)
            {
                result.Add(view.ammo);
            }

            //클릭 불가 처리
            var btn = slot.GetComponentInChildren<Button>(true);
            if (btn)
            {
                btn.interactable = false;
            }

            view?.SetBgColor(Color.black);
            view.RefreshLabel();

            //DiscardBg로 부모 변경
            if (discardBg != null)
            {
                slot.SetParent(discardBg, false);
                slot.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("discardBg is null");
                Destroy(slot.gameObject);
            }
        }

        // 선택 상태 리셋
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;


        return result;
    }

    // 덱에서 받은 Ammo 리스트로 채우기
    public void AddBullets(List<Ammo> ammos)
    {
        if (ammos != null)
        {
            foreach (var a in ammos)
            {
               SpawnOne(a);
                handAmmoList.Clear();
               handAmmoList.Add(a);
            }
        }
    }

    //버튼 연결
    private void SpawnOne(Ammo ammo)
    {
        //여기서 딱히 Bullet이 Null인지 체크할필요가 없어짐
        if (slotContainer == null)
        {
            Debug.LogError("Not Found slotContainer");
            return;
        }

        GameObject go = GameManager.Resource.Create<GameObject>(Path.UI + "Bullet", slotContainer);
        go.transform.localScale = Vector3.one;

        var view = go.GetComponent<BulletView>();
        if (view)
        {
            view.ammo = ammo;//어떤 탄인지
            view.SetBgColor(bgNormal);
            view.RefreshLabel();
        }
      

        var btn = go.GetComponentInChildren<Button>(true);
        if(btn)
        {
            btn.onClick.AddListener(() => SelectAmmo(btn));
        }
    }

    public void EmptyAmmo()
    {
        if (slotContainer.childCount <= 0 && GameManager.ItemControl.drawPile.Count <= 0)
        {
            ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
            backUI.GetResultType(ResultType.Over);
            backUI.OpenUI();
        }
    }

    public void Unselect()
    {
        if(selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;

        GameManager.Map.attackRange.ClearAttackType();
        GameManager.Mouse.IsAttacking = false;
    }

}
