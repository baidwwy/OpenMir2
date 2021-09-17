﻿using SystemModule;
using System;
using GameSvr.CommandSystem;

namespace GameSvr
{
    /// <summary>
    /// 删除指定玩家包裹物品
    /// </summary>
    [GameCommand("DeleteItem", "删除指定玩家包裹物品", 10)]
    public class DeleteItemCommand : BaseCommond
    {
        [DefaultCommand]
        public void DeleteItem(string[] @Params, TPlayObject PlayObject)
        {
            var sHumanName = @Params.Length > 0 ? @Params[0] : "";//玩家名称
            var sItemName = @Params.Length > 1 ? @Params[1] : "";//物品名称
            var nCount = @Params.Length > 2 ? int.Parse(@Params[2]) : 0;//数量
            int nItemCount;
            GameItem StdItem;
            TUserItem UserItem;
            if (sHumanName == "" || sItemName == "")
            {
                PlayObject.SysMsg("命令格式: @" + this.Attributes.Name + " 人物名称 物品名称 数量)", TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            var m_PlayObject = M2Share.UserEngine.GetPlayObject(sHumanName);
            if (m_PlayObject == null)
            {
                PlayObject.SysMsg(string.Format(M2Share.g_sNowNotOnLineOrOnOtherServer, sHumanName), TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            nItemCount = 0;
            for (var i = m_PlayObject.m_ItemList.Count - 1; i >= 0; i--)
            {
                if (m_PlayObject.m_ItemList.Count <= 0)
                {
                    break;
                }
                UserItem = m_PlayObject.m_ItemList[i];
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if (StdItem != null && sItemName.ToLower().CompareTo(StdItem.Name.ToLower()) == 0)
                {
                    m_PlayObject.SendDelItems(UserItem);
                    m_PlayObject.m_ItemList.RemoveAt(i);
                    UserItem = null;
                    nItemCount++;
                    if (nItemCount >= nCount)
                    {
                        break;
                    }
                }
            }
        }
    }
}