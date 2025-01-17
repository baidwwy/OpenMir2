﻿using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    [GameCommand("BindUseItem", "", M2Share.g_sGameCommandBindUseItemHelpMsg, 10)]
    public class BindUseItemCommand : BaseCommond
    {
        [DefaultCommand]
        public void BindUseItem(string[] @Params, TPlayObject PlayObject)
        {
            var sHumanName = @Params.Length > 0 ? @Params[0] : "";
            var sItem = @Params.Length > 1 ? @Params[1] : "";
            var sType = @Params.Length > 2 ? @Params[2] : "";
            var sLight = @Params.Length > 3 ? @Params[3] : "";

            TUserItem UserItem = null;
            var nBind = -1;
            TItemBind ItemBind;
            int nItemIdx;
            int nMakeIdex;
            string sBindName;
            bool boFind;
            bool boLight;
            var nItem = M2Share.GetUseItemIdx(sItem);
            if (String.Compare(sType, "帐号", StringComparison.Ordinal) == 0)
            {
                nBind = 0;
            }
            if (String.Compare(sType, "人物", StringComparison.Ordinal) == 0)
            {
                nBind = 1;
            }
            if (String.Compare(sType, "IP", StringComparison.Ordinal) == 0)
            {
                nBind = 2;
            }
            if (String.Compare(sType, "死亡", StringComparison.Ordinal) == 0)
            {
                nBind = 3;
            }
            boLight = sLight == "1";
            if (nItem < 0 || nBind < 0 || string.IsNullOrEmpty(sHumanName) || !string.IsNullOrEmpty(sHumanName) && sHumanName[1] == '?')
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            var m_PlayObject = M2Share.UserEngine.GetPlayObject(sHumanName);
            if (m_PlayObject == null)
            {
                PlayObject.SysMsg(string.Format(M2Share.g_sNowNotOnLineOrOnOtherServer, sHumanName), MsgColor.Red, MsgType.Hint);
                return;
            }
            UserItem = m_PlayObject.m_UseItems[nItem];
            if (UserItem.wIndex == 0)
            {
                PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandBindUseItemNoItemMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                return;
            }
            nItemIdx = UserItem.wIndex;
            nMakeIdex = UserItem.MakeIndex;
            switch (nBind)
            {
                case 0:
                    boFind = false;
                    sBindName = m_PlayObject.m_sUserID;
                    HUtil32.EnterCriticalSection(M2Share.g_ItemBindAccount);
                    try
                    {
                        for (var i = 0; i < M2Share.g_ItemBindAccount.Count; i++)
                        {
                            ItemBind = M2Share.g_ItemBindAccount[i];
                            if (ItemBind.nItemIdx == nItemIdx && ItemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            ItemBind = new TItemBind();
                            ItemBind.nItemIdx = nItemIdx;
                            ItemBind.nMakeIdex = nMakeIdex;
                            ItemBind.sBindName = sBindName;
                            M2Share.g_ItemBindAccount.Insert(0, ItemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(M2Share.g_ItemBindAccount);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    M2Share.SaveItemBindAccount();
                    PlayObject.SysMsg(string.Format("%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。", M2Share.GetUseItemName(nItem),
                        M2Share.UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    m_PlayObject.SysMsg(string.Format("你的%s[%s]已经绑定到%s[%s]上了。", M2Share.GetUseItemName(nItem), M2Share.UserEngine.GetStdItemName(UserItem.wIndex),
                        sType, sBindName), MsgColor.Blue, MsgType.Hint);
                    m_PlayObject.SendMsg(m_PlayObject, Grobal2.RM_SENDUSEITEMS, 0, 0, 0, 0, "");
                    break;

                case 1:
                    sBindName = m_PlayObject.m_sCharName;
                    boFind = false;
                    HUtil32.EnterCriticalSection(M2Share.g_ItemBindCharName);
                    try
                    {
                        for (var i = 0; i < M2Share.g_ItemBindCharName.Count; i++)
                        {
                            ItemBind = M2Share.g_ItemBindCharName[i];
                            if (ItemBind.nItemIdx == nItemIdx && ItemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            ItemBind = new TItemBind();
                            ItemBind.nItemIdx = nItemIdx;
                            ItemBind.nMakeIdex = nMakeIdex;
                            ItemBind.sBindName = sBindName;
                            M2Share.g_ItemBindCharName.Insert(0, ItemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(M2Share.g_ItemBindCharName);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    M2Share.SaveItemBindCharName();
                    PlayObject.SysMsg(string.Format("%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。", M2Share.GetUseItemName(nItem),
                        M2Share.UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    m_PlayObject.SysMsg(string.Format("你的%s[%s]已经绑定到%s[%s]上了。", M2Share.GetUseItemName(nItem), M2Share.UserEngine.GetStdItemName(UserItem.wIndex),
                        sType, sBindName), MsgColor.Blue, MsgType.Hint);

                    // PlayObject.SendUpdateItem(UserItem);
                    m_PlayObject.SendMsg(m_PlayObject, Grobal2.RM_SENDUSEITEMS, 0, 0, 0, 0, "");
                    break;

                case 2:
                    boFind = false;
                    sBindName = m_PlayObject.m_sIPaddr;
                    HUtil32.EnterCriticalSection(M2Share.g_ItemBindIPaddr);
                    try
                    {
                        for (var i = 0; i < M2Share.g_ItemBindIPaddr.Count; i++)
                        {
                            ItemBind = M2Share.g_ItemBindIPaddr[i];
                            if (ItemBind.nItemIdx == nItemIdx && ItemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            ItemBind = new TItemBind();
                            ItemBind.nItemIdx = nItemIdx;
                            ItemBind.nMakeIdex = nMakeIdex;
                            ItemBind.sBindName = sBindName;
                            M2Share.g_ItemBindIPaddr.Insert(0, ItemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(M2Share.g_ItemBindIPaddr);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    M2Share.SaveItemBindIPaddr();
                    PlayObject.SysMsg(string.Format("%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。", M2Share.GetUseItemName(nItem),
                        M2Share.UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    m_PlayObject.SysMsg(string.Format("你的%s[%s]已经绑定到%s[%s]上了。", M2Share.GetUseItemName(nItem), M2Share.UserEngine.GetStdItemName(UserItem.wIndex),
                        sType, sBindName), MsgColor.Blue, MsgType.Hint);

                    // PlayObject.SendUpdateItem(UserItem);
                    m_PlayObject.SendMsg(m_PlayObject, Grobal2.RM_SENDUSEITEMS, 0, 0, 0, 0, "");
                    break;
                    //case 3:// 人物装备死亡不爆绑定
                    //    sBindName = PlayObject.m_sCharName;
                    //    M2Share.g_ItemBindDieNoDropName.__Lock();
                    //    try
                    //    {
                    //        for (var i = 0; i < M2Share.g_ItemBindDieNoDropName.Count; i++)
                    //        {
                    //            //ItemBind = M2Share.g_ItemBindDieNoDropName[i];
                    //            //if ((ItemBind.nItemIdx == nItemIdx) && (ItemBind.sBindName == sBindName))
                    //            //{
                    //            //    this.SysMsg(string.Format(M2Share.g_sGameCommandBindUseItemAlreadBindMsg, new string[] { sHumanName, sItem }), TMsgColor.c_Red, TMsgType.t_Hint);
                    //            //    return;
                    //            //}
                    //        }
                    //        ItemBind = new TItemBind
                    //        {
                    //            nItemIdx = nItemIdx,
                    //            nMakeIdex = 0,
                    //            sBindName = sBindName
                    //        };
                    //        //M2Share.g_ItemBindDieNoDropName.InsertText(0, ItemBind);
                    //    }
                    //    finally
                    //    {
                    //        M2Share.g_ItemBindDieNoDropName.UnLock();
                    //    }
                    //    M2Share.SaveItemBindDieNoDropName();// 保存人物装备死亡不爆列表
                    //    m_PlayObject.SysMsg(string.Format("%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，死亡不爆绑定到%s成功。", M2Share.GetUseItemName(nItem), M2Share.UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax, sBindName), TMsgColor.c_Blue, TMsgType.t_Hint);
                    //    PlayObject.SysMsg(string.Format("您的%s[%s]已经绑定到%s[%s]上了。", M2Share.GetUseItemName(nItem), M2Share.UserEngine.GetStdItemName(UserItem.wIndex), sType, sBindName), TMsgColor.c_Blue, TMsgType.t_Hint);
                    //    break;
            }
        }
    }
}