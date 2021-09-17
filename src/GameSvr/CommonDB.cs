using System;
using System.Collections.Generic;
using System.IO;
using SystemModule;

namespace GameSvr
{
    /// <summary>
    /// 数据库查询类
    /// </summary>
    public class CommonDB
    {
        public bool CheckDataBase()
        {
            var dataPath = Path.Combine(M2Share.g_Config.sEnvirDir, "Data.db");
            if (!File.Exists(dataPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("数据库文件不存在,请确认文件是否存在或路径是否正确.");
                Console.ResetColor();
                return false;
            }
            Sqlite.SqlConnctionString = string.Format(Sqlite.SqlConnctionString, dataPath);
            return true;
        }
        
        public int LoadItemsDB()
        {
            int result = -1;
            int Idx;
            GameItem Item;
            const string sSQLString = "SELECT * FROM STDITEMS";
            //HUtil32.EnterCriticalSection(M2Share.ProcessHumanCriticalSection);
            try
            {
                for (var i = 0; i < M2Share.UserEngine.StdItemList.Count; i++)
                {
                    M2Share.UserEngine.StdItemList[i] = null;
                }
                M2Share.UserEngine.StdItemList.Clear();
                result = -1;
                using (var dr = Sqlite.ExecuteReader(sSQLString))
                {
                    while (dr.Read())
                    {
                        Item = new GameItem();
                        Idx = dr.GetInt32("Idx");// 序号
                        Item.Name = dr.GetString("Name");// 名称
                        Item.StdMode = dr.GetByte("StdMode");// 分类号
                        Item.Shape = dr.GetByte("Shape");// 装备外观
                        Item.Weight = dr.GetByte("Weight");// 重量
                        Item.AniCount = dr.GetByte("AniCount");
                        Item.Source = dr.GetInt16("Source");
                        Item.Reserved = dr.GetByte("Reserved");// 保留
                        Item.Looks = dr.GetUInt16("Looks");// 物品外观
                        Item.DuraMax = (ushort)dr.GetInt32("DuraMax");// 持久
                        Item.Ac = (ushort)HUtil32.Round(dr.GetInt32("AC") * (M2Share.g_Config.nItemsACPowerRate / 10));
                        Item.Ac2 = (ushort)HUtil32.Round(dr.GetInt32("AC2") * (M2Share.g_Config.nItemsACPowerRate / 10));
                        Item.Mac = (ushort)HUtil32.Round(dr.GetInt32("MAC") * (M2Share.g_Config.nItemsACPowerRate / 10));
                        Item.Mac2 = (ushort)HUtil32.Round(dr.GetInt32("MAC2") * (M2Share.g_Config.nItemsACPowerRate / 10));
                        Item.Dc = (ushort)HUtil32.Round(dr.GetInt32("DC") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Dc2 = (ushort)HUtil32.Round(dr.GetInt32("DC2") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Mc = (ushort)HUtil32.Round(dr.GetInt32("MC") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Mc2 = (ushort)HUtil32.Round(dr.GetInt32("MC2") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Sc = (ushort)HUtil32.Round(dr.GetInt32("SC") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Sc2 = (ushort)HUtil32.Round(dr.GetInt32("SC2") * (M2Share.g_Config.nItemsPowerRate / 10));
                        Item.Need = dr.GetInt32("Need");// 附加条件
                        Item.NeedLevel = dr.GetInt32("NeedLevel");// 需要等级
                        Item.Price = dr.GetInt32("Price");// 价格
                        Item.NeedIdentify = M2Share.GetGameLogItemNameList(Item.Name);
                        switch (Item.StdMode)
                        {
                            case 0:
                            case 55:
                            case 58: // 药品
                                Item.ItemType = Grobal2.ITEM_LEECHDOM;
                                break;
                            case 5:
                            case 6: // 武器
                                Item.ItemType = Grobal2.ITEM_WEAPON;
                                break;
                            case 10:
                            case 11: // 衣服
                                Item.ItemType = Grobal2.ITEM_ARMOR;
                                break;
                            case 15:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                            case 26:
                            case 51:
                            case 52:
                            case 53:
                            case 54:
                            case 62:
                            case 63:
                            case 64:
                            case 30: // 辅助物品
                                Item.ItemType = Grobal2.ITEM_ACCESSORY;
                                break;
                            default: // 其它物品
                                Item.ItemType = Grobal2.ITEM_ETC;
                                break;
                        }
                        if (M2Share.UserEngine.StdItemList.Count == Idx)
                        {
                            M2Share.UserEngine.StdItemList.Add(Item);
                            result = 1;
                        }
                        else
                        {
                            M2Share.MainOutMessage(string.Format("加载物品(Idx:{0} Name:{1})数据失败！！！", new object[] { Idx, Item.Name }));
                            result = -100;
                            return result;
                        }
                    }
                }
                M2Share.g_boGameLogGold = M2Share.GetGameLogItemNameList(Grobal2.sSTRING_GOLDNAME) == 1;
                M2Share.g_boGameLogHumanDie = M2Share.GetGameLogItemNameList(M2Share.g_sHumanDieEvent) == 1;
                M2Share.g_boGameLogGameGold = M2Share.GetGameLogItemNameList(M2Share.g_Config.sGameGoldName) == 1;
                M2Share.g_boGameLogGamePoint = M2Share.GetGameLogItemNameList(M2Share.g_Config.sGamePointName) == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return result;
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessHumanCriticalSection);
            }
            return result;
        }

        public int LoadMagicDB()
        {
            TMagic Magic;
            const string sSQLString = "select * from Magic";
            var result = -1;
            HUtil32.EnterCriticalSection(M2Share.ProcessHumanCriticalSection);
            try
            {
                M2Share.UserEngine.SwitchMagicList();
                using var dr = Sqlite.ExecuteReader(sSQLString);
                while (dr.Read())
                {
                    Magic = new TMagic
                    {
                        wMagicID = dr.GetUInt16("MagId"),
                        sMagicName = dr.GetString("MagName"),
                        btEffectType = dr.GetByte("EffectType"),
                        btEffect = dr.GetByte("Effect"),
                        wSpell = dr.GetUInt16("Spell"),
                        wPower = dr.GetUInt16("Power"),
                        wMaxPower = dr.GetUInt16("MaxPower"),
                        btJob = dr.GetByte("Job")
                    };
                    Magic.TrainLevel[0] = dr.GetByte("NeedL1");
                    Magic.TrainLevel[1] = dr.GetByte("NeedL2");
                    Magic.TrainLevel[2] = dr.GetByte("NeedL3");
                    Magic.TrainLevel[3] = dr.GetByte("NeedL3");
                    Magic.MaxTrain[0] = dr.GetInt32("L1Train");
                    Magic.MaxTrain[1] = dr.GetInt32("L2Train");
                    Magic.MaxTrain[2] = dr.GetInt32("L3Train");
                    Magic.MaxTrain[3] = Magic.MaxTrain[2];
                    Magic.btTrainLv = 3;
                    Magic.dwDelayTime = dr.GetInt32("Delay");
                    Magic.btDefSpell = dr.GetByte("DefSpell");
                    Magic.btDefPower = dr.GetByte("DefPower");
                    Magic.btDefMaxPower = dr.GetByte("DefMaxPower");
                    Magic.sDescr = dr.GetString("Descr");
                    if (Magic.wMagicID > 0)
                    {
                        M2Share.UserEngine.m_MagicList.Add(Magic);
                    }
                    else
                    {
                        Magic = null;
                    }
                    result = 1;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessHumanCriticalSection);
            }
            return result;
        }
        
        public int LoadMonsterDB()
        {
            var result = 0;
            TMonInfo Monster;
            const string sSQLString = "select * from Monster";
            HUtil32.EnterCriticalSection(M2Share.ProcessHumanCriticalSection);
            try
            {
                M2Share.UserEngine.MonsterList.Clear();
                using var dr = Sqlite.ExecuteReader(sSQLString);
                while (dr.Read())
                {
                    Monster = new TMonInfo
                    {
                        ItemList = new List<TMonItem>(),
                        sName = dr.GetString("NAME").Trim(),
                        btRace = dr.GetByte("Race"),
                        btRaceImg = dr.GetByte("RaceImg"),
                        wAppr = dr.GetUInt16("Appr"),
                        wLevel = dr.GetUInt16("Lvl"),
                        btLifeAttrib = dr.GetByte("Undead"),
                        wCoolEye = dr.GetInt16("CoolEye"),
                        dwExp = dr.GetInt32("Exp")
                    };
                    // 城门或城墙的状态跟HP值有关，如果HP异常，将导致城墙显示不了
                    if (Monster.btRace == 110 || Monster.btRace == 111)
                    {
                        // 如果为城墙或城门由HP不加倍
                        Monster.wHP = dr.GetUInt16("HP");
                    }
                    else
                    {
                        Monster.wHP = (ushort)HUtil32.Round(dr.GetInt32("HP") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    }
                    Monster.wMP = (ushort)HUtil32.Round(dr.GetInt32("MP") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wAC = (ushort)HUtil32.Round(dr.GetInt32("AC") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wMAC = (ushort)HUtil32.Round(dr.GetInt32("MAC") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wDC = (ushort)HUtil32.Round(dr.GetInt32("DC") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wMaxDC = (ushort)HUtil32.Round(dr.GetInt32("DCMAX") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wMC = (ushort)HUtil32.Round(dr.GetInt32("MC") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wSC = (ushort)HUtil32.Round(dr.GetInt32("SC") * (M2Share.g_Config.nMonsterPowerRate / 10));
                    Monster.wSpeed = dr.GetUInt16("SPEED");
                    Monster.wHitPoint = dr.GetUInt16("HIT");
                    Monster.wWalkSpeed = (ushort)HUtil32._MAX(200, dr.GetInt32("WALK_SPD"));
                    Monster.wWalkStep = (ushort)HUtil32._MAX(1, dr.GetInt32("WalkStep"));
                    Monster.wWalkWait = (ushort)dr.GetInt32("WalkWait");
                    Monster.wAttackSpeed = (ushort)dr.GetInt32("ATTACK_SPD");
                    if (Monster.wWalkSpeed < 200)
                    {
                        Monster.wWalkSpeed = 200;
                    }
                    if (Monster.wAttackSpeed < 200)
                    {
                        Monster.wAttackSpeed = 200;
                    }
                    Monster.ItemList = null;
                    M2Share.LocalDB.LoadMonitems(Monster.sName, ref Monster.ItemList);
                    M2Share.UserEngine.MonsterList.Add(Monster);
                    result = 1;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessHumanCriticalSection);
            }
            return result;
        }

    }
}