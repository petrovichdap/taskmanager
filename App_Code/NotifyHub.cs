﻿using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;

public class NotifyHub : Hub
{
	public static void NotifyDefectChange(int ttid)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnDefectChanged(ttid);
	}
	public static void NotifyPlanChange(int userid)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnPlanChanged(userid);
	}
	public static void NotifyRoomChange()
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnRoomChanged(Roommate.Enum());
	}
	public static void NotifyBuildChange(int id, int ttid, int userid, string message)
	{
		var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
		context.Clients.All.OnBuildChanged(id, ttid, userid, message);
	}
	public static void NotifyBuildStatusChange(int id, int ttid, int userid, string message)
	{
		string mess = $"Build Request:  {message}{Settings.CurrentSettings.GetTTAnchor(ttid)}";
		DefectUser u = new DefectUser(userid);
		MPSUser mpu = new MPSUser(u.TRID);
		TasksBot.SendMessage(mpu.CHATID, mess);
	}
	public void RequestRoomUsers()
	{
		Clients.Caller.OnRoomChanged(Roommate.Enum());
	}
	public void SendMessage(int fromID, int toID, string message)
	{
		MPSUser from = new MPSUser(fromID);
		MPSUser to = new MPSUser(toID);
		TasksBot.SendMessage(to.CHATID, $"{from.PERSON_NAME}: {message}");
	}
	public void LockTask(int ttid, string currentlock, int userid)
	{
		LockInfo li = Defect.Locktask(ttid.ToString(), currentlock, userid.ToString());
		Clients.Caller.OnLockTask(li);
	}
	public void lockTaskForce(int ttid, string currentlock, int userid)
	{
		LockInfo li = Defect.Locktask(ttid.ToString(), currentlock, userid.ToString(), true);
		Clients.Caller.OnLockTask(li);
	}
	public void UnLockTask(int ttid, string currentlock)
	{
		Defect.UnLocktask(ttid.ToString(), currentlock);
	}
}