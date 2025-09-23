using System;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
using UGS;

[Serializable]
public class DataManager : MonoBehaviour
{
    // csv���� �ҷ�����
    [Header("CSV Files")]
    public TextAsset unitCSV;
    
    public EntityDataGroup entityDataGroup;
    
    [SerializeField] private Dictionary<UnitType, Dictionary<int, UnitData>> unitDB;

    private void Awake()
    {
        unitDB = new Dictionary<UnitType, Dictionary<int, UnitData>>();

    }

    public void Initialize()
    {
        UnityGoogleSheet.LoadAllData();
        entityDataGroup = new EntityDataGroup();
    }
    
    private void Start()
    {
        unitCSV = GameManager.Resource.Load<TextAsset>(Path.Data+"UnitData");
        
        LoadCSV();
    }

    private void LoadCSV()
    {
        string[] lines = unitCSV.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return;

        string[] headers = lines[0].Split(',');

        unitDB.Clear();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length < 2) continue;

            UnitData data = new UnitData();

            data.Type = ParseUnitType(values[0].Trim());
            int.TryParse(values[1], out data.ID);
            data.Name = values[2].Trim();
            int.TryParse(values[3], out data.Size);
            int.TryParse(values[4], out data.Attack);
            int.TryParse(values[5], out data.MinAttackRange);
            int.TryParse(values[6], out data.MaxAttackRange);
            int.TryParse(values[7], out data.MoveRange);
            int.TryParse(values[8], out data.Health);
            data.Attribute = ParseEnemyType(values[9].Trim());
            int.TryParse(values[10], out data.MaxNum);
            int.TryParse(values[11], out data.MinNum);
            int.TryParse(values[12], out data.Mulligan);
            int.TryParse(values[13], out data.Reload);
            int.TryParse(values[14], out data.MaxBullet);
            int.TryParse(values[15], out data.Additional);

            if (!unitDB.ContainsKey(data.Type))
            {
                unitDB[data.Type] = new Dictionary<int, UnitData>();
            }

            unitDB[data.Type][data.ID] = data;
            //Debug.Log($"{unitDB.Count}");
        }

    }

    private UnitType ParseUnitType(string typeStr)
    {
        string str = typeStr.ToLower();

        //Debug.Log(str);

        UnitType type = UnitType.Unknown;

        if (str == "player")
        {
            type = UnitType.Player;
        }
        else if (str == "enemy")
        {
            type = UnitType.Enemy;
        }
        else if(str == "vehicle")
        {
            type = UnitType.Vehicle;
        }

        return type;
    }

    private EnemyAttribute ParseEnemyType(string typeStr)
    {
        string str = typeStr.ToLower();
        EnemyAttribute attri = EnemyAttribute.Low;
        if(str == "low")
        {
            attri = EnemyAttribute.Low;
        }
        else if(str == "high")
        {
            attri = EnemyAttribute.High;
        }

        return attri;
    }

    public UnitData GetUnit(UnitType type, int id)
    {
        if(unitDB.TryGetValue(type, out var dict) &&
            dict.TryGetValue(id, out var data))
        {
            return data;
        }

        Debug.Log("������ ����");

        return null;
    }





}
