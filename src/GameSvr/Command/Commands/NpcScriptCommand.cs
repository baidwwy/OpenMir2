﻿using SystemModule;
using System.IO;
using SystemModule.Common;
using GameSvr.CommandSystem;

namespace GameSvr
{
    [GameCommand("NpcScript", "重新读取面对面NPC脚本", "重新读取面对面NPC脚本", 10)]
    public class NpcScriptCommand : BaseCommond
    {
        [DefaultCommand]
        public void NpcScript(TPlayObject PlayObject)
        {
            var sScriptFileName = string.Empty;
            TMerchant Merchant;
            TNormNpc NormNpc;
            StringList LoadList;
            string sScriptLine;
            var nNPCType = -1;
            var BaseObject = PlayObject.GetPoseCreate();
            if (BaseObject != null)
            {
                for (var i = 0; i < M2Share.UserEngine.m_MerchantList.Count; i++)
                {
                    if (M2Share.UserEngine.m_MerchantList[i] == BaseObject)
                    {
                        nNPCType = 0;
                        break;
                    }
                }
                for (var i = 0; i < M2Share.UserEngine.QuestNPCList.Count; i++)
                {
                    if (M2Share.UserEngine.QuestNPCList[i] == BaseObject)
                    {
                        nNPCType = 1;
                        break;
                    }
                }
            }
            if (nNPCType < 0)
            {
                PlayObject.SysMsg("命令使用方法不正确，必须与NPC面对面，才能使用此命令!!!", TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            if (nNPCType == 0)
            {
                Merchant = (TMerchant)BaseObject;
                sScriptFileName = M2Share.g_Config.sEnvirDir + M2Share.sMarket_Def + Merchant.m_sScript + "-" + Merchant.m_sMapName + ".txt";
            }
            if (nNPCType == 1)
            {
                NormNpc = (TNormNpc)BaseObject;
                sScriptFileName = M2Share.g_Config.sEnvirDir + M2Share.sNpc_def + NormNpc.m_sCharName + "-" + NormNpc.m_sMapName + ".txt";
            }
            if (File.Exists(sScriptFileName))
            {
                LoadList = new StringList();
                try
                {
                    LoadList.LoadFromFile(sScriptFileName);
                }
                catch
                {
                    PlayObject.SysMsg("读取脚本文件错误: " + sScriptFileName, TMsgColor.c_Red, TMsgType.t_Hint);
                }
                for (var i = 0; i < LoadList.Count; i++)
                {
                    sScriptLine = LoadList[i].Trim();
                    sScriptLine = HUtil32.ReplaceChar(sScriptLine, ' ', ',');
                    PlayObject.SysMsg(i + "," + sScriptLine, TMsgColor.c_Blue, TMsgType.t_Hint);
                }
                LoadList = null;
            }
        }
    }
}