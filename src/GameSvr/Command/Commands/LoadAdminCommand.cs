﻿using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    /// <summary>
    /// 重新加载管理员列表
    /// </summary>
    [GameCommand("LoadAdmin", "重新加载管理员列表", 10)]
    public class LoadAdminCommand : BaseCommond
    {
        [DefaultCommand]
        public void LoadAdmin(TPlayObject PlayObject)
        {
            if (PlayObject.m_btPermission < 6)
            {
                return;
            }
            //LocalDB.GetInstance().LoadAdminList();
            // UserEngine.SendServerGroupMsg(213, nServerIndex, '');
            PlayObject.SysMsg("管理员列表重新加载成功...", MsgColor.Green, MsgType.Hint);
        }
    }
}