﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class TrackerService : WebService
{
	public TrackerService() { }
	[WebMethod(EnableSession = true)]
	public List<Tracker> getTrackers(int user)
	{
		CurrentContext.Validate();
		return Tracker.Enum(user);
	}
	[WebMethod(EnableSession = true)]
	public string getTrackerModified(int id)
	{
		CurrentContext.Validate();
		Tracker t = new Tracker(id);
		DateTime? dt = (new DefectBase()).ModTime(t.GetFilter());
		if (dt != null)
		{
			return dt.Value.ToString(IdBasedObject.defDateTimeFormat, CultureInfo.InvariantCulture);
		};
		return t.CREATED;
	}
	[WebMethod(EnableSession = true)]
	public List<Tracker> assignTracker(int id, int userid)
	{
		CurrentContext.Validate();
		Tracker t = new Tracker(id);
		t.IDCLIENT = userid;
		t.Store();
		return Tracker.Enum(CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public void delTracker(int id)
	{
		CurrentContext.Validate();
		Tracker.Delete(id);
	}
	public class TrackerResults
	{
		public TrackerResults() { }
		public List<DefectPlan> ITEMS;
		public Tracker TRACKER;
	}
	[WebMethod(EnableSession = true)]
	public TrackerResults getItems(int trackerid)
	{
		CurrentContext.Validate();
		TrackerResults res = new TrackerResults();
		res.TRACKER = new Tracker(trackerid);
		List<DefectBase> defs = (new DefectBase()).Enum(res.TRACKER.GetFilter());
		string COLORDEFS = "";
		foreach (var disp in DefectDispo.Enum())
		{
			int estim = 0;
			foreach (var def in defs)
			{
				if (def.DISPO == disp.ID.ToString())
				{
					estim += def.ESTIM;
				}
			}
			if (estim > 0)
			{
				COLORDEFS += $"{estim}:{disp.COLOR};";
			}
		}
		if (res.TRACKER.COLORDEFS != COLORDEFS)
		{
			res.TRACKER.COLORDEFS = COLORDEFS;
			res.TRACKER.Store();
		}
		res.ITEMS = DefectPlan.Convert2Plan(defs);
		return res;
	}
	[WebMethod(EnableSession = true)]
	public Tracker newTracker(string name, int user, int filter)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		CurrentContext.Validate();
		return Tracker.New(name, user, filter);
	}
	public class PublicFilter
	{
		public PublicFilter() { }
		public string NAME { get; set; }
		public int ID { get; set; }
		public bool SHARED { get; set; }
	}
	[WebMethod(EnableSession = true)]
	public List<PublicFilter> getFilters(int user)
	{
		CurrentContext.Validate();
		List<StoredDefectsFilter> sdf = StoredDefectsFilter.Enum(user);
		List<PublicFilter> lsout = new List<PublicFilter>();
		foreach (var f in sdf)
		{
			lsout.Add(new PublicFilter() { NAME = f.NAME, ID = f.ID, SHARED = f.SHARED });
		}
		return lsout;
	}
	[WebMethod(EnableSession = true)]
	public DefectPlan newTask(string summary, int trackerid)
	{
		CurrentContext.ValidateAdmin();
		if (string.IsNullOrEmpty(summary) || trackerid < 0)
			return null;
		Tracker t = new Tracker(trackerid);
		summary += "@" + t.NAME;
		Defect d = new Defect(Defect.New(summary));
		d.AddMessage("tag:" + t.GetTag(), CurrentContext.UserID);
		d.ESTIM = 1;
		d.Store();
		return new DefectPlan(d);
	}
}