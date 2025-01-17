﻿using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    /// <summary>
    /// 重新加载当前12格范围内NPC
    /// </summary>
    [GameCommand("ReloadNpc", "重新加载当前9格范围内NPC", 10)]
    public class ReloadNpcCommand : BaseCommond
    {
        [DefaultCommand]
        public void ReloadNpc(string[] @Params, TPlayObject PlayObject)
        {
            var sParam = string.Empty;
            if (@Params != null)
            {
                sParam = @Params.Length > 0 ? @Params[0] : "";
            }
            IList<TBaseObject> TmpMerList = null;
            IList<TBaseObject> TmpNorList = null;
            Merchant Merchant;
            NormNpc NPC;
            if (string.Compare("all", sParam, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                M2Share.LocalDB.ReLoadMerchants();
                M2Share.UserEngine.ReloadMerchantList();
                PlayObject.SysMsg("交易NPC重新加载完成!!!", MsgColor.Red, MsgType.Hint);
                M2Share.UserEngine.ReloadNpcList();
                PlayObject.SysMsg("管理NPC重新加载完成!!!", MsgColor.Red, MsgType.Hint);
                return;
            }
            else
            {
                TmpMerList = new List<TBaseObject>();
                try
                {
                    if (M2Share.UserEngine.GetMerchantList(PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 9, TmpMerList) > 0)
                    {
                        for (var i = 0; i < TmpMerList.Count; i++)
                        {
                            Merchant = (Merchant)TmpMerList[i];
                            Merchant.ClearScript();
                            Merchant.LoadNPCScript();
                            PlayObject.SysMsg(Merchant.m_sCharName + "重新加载成功...", MsgColor.Green, MsgType.Hint);
                        }
                    }
                    else
                    {
                        PlayObject.SysMsg("附近未发现任何交易NPC!!!", MsgColor.Red, MsgType.Hint);
                    }
                    TmpNorList = new List<TBaseObject>();
                    if (M2Share.UserEngine.GetNpcList(PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 9, TmpNorList) > 0)
                    {
                        for (var i = 0; i < TmpNorList.Count; i++)
                        {
                            NPC = TmpNorList[i] as NormNpc;
                            NPC.ClearScript();
                            NPC.LoadNPCScript();
                            PlayObject.SysMsg(NPC.m_sCharName + "重新加载成功...", MsgColor.Green, MsgType.Hint);
                        }
                    }
                    else
                    {
                        PlayObject.SysMsg("附近未发现任何管理NPC!!!", MsgColor.Red, MsgType.Hint);
                    }
                }
                finally
                {
                    TmpMerList = null;
                    TmpNorList = null;
                }
            }
        }
    }
}