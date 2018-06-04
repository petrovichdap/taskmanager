﻿using System;
using System.Globalization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

public class LockInfo
{
	public LockInfo(string lockedby, string globallock)
	{
		this.lockedby = lockedby;
		this.globallock = globallock;
	}
	public string lockedby { get; set; }
	public string globallock { get; set; }
}
public class LockEvent
{
	public LockEvent(string id)
	{
		Prolongate();
		lockid = id;
		usr = CurrentContext.User.EMAIL;
	}
	public void Prolongate()
	{
		locktime = DateTime.Now;
	}
	public bool Obsolete
	{
		get
		{
			return DateTime.Now.Subtract(locktime).TotalSeconds > 30;
		}
	}
	DateTime locktime { get; set; }
	public string lockid { get; set; }
	public string usr { get; set; }
}
public class DefectBase : IdBasedObject
{
	protected static string _idRec = "idRecord";
	protected static string _ID = "DefectNum";
	protected static string _Summ = "Summary";
	protected static string _Disp = "idDisposit";
	protected static string _Est = "Estim";
	protected static string _Order = "iOrder";
	protected static string _AsUser = "idUsr";
	static protected string _Seve = "idSeverity";
	protected static string _Tabl = "[TT_RES].[DBO].[DEFECTS]";

	public int ID
	{
		get { return this[_ID] == DBNull.Value ? -1 : Convert.ToInt32(this[_ID]); }
		set { this[_ID] = Convert.ToInt32(value); _id = ID.ToString(); }
	}
	public int IDREC
	{
		get { return this[_idRec] == DBNull.Value ? 0 : Convert.ToInt32(this[_idRec]); }
		set { this[_idRec] = Convert.ToInt32(value); }
	}
	public string SUMMARY
	{
		get { return this[_Summ].ToString(); }
		set { this[_Summ] = value; }
	}
	public string DISPO
	{
		get { return this[_Disp].ToString(); }
		set { this[_Disp] = Convert.ToInt32(value); }
	}
	public int ESTIM
	{
		get { return this[_Est] == DBNull.Value ? 0 : Convert.ToInt32(this[_Est]); }
		set
		{
			if (value == 0)
			{
				this[_Est] = DBNull.Value;
			}
			else
			{
				this[_Est] = value;
			}
		}
	}
	public int ORDER
	{
		get { return this[_Order] == DBNull.Value ? -1 : Convert.ToInt32(this[_Order]); }
		set
		{
			if (value == -1)
			{
				this[_Order] = DBNull.Value;
			}
			else
			{
				this[_Order] = value;
			}
		}
	}
	public string AUSER
	{
		get { return this[_AsUser] == DBNull.Value ? "" : this[_AsUser].ToString(); }
		set { this[_AsUser] = Convert.ToInt32(value); }
	}

	static string[] _allcols = new string[] { _ID, _Summ, _idRec, _Disp, _Est, _Order, _AsUser, _Seve };
	static string[] _allcolsNames = new string[] { _ID, "Summary", _idRec, "Disposition", "Estimation", "Schedule Order", "Assigned User", "Severity" };

	public DefectBase()
		: base(_Tabl, _allcols, "0", _ID, false)
	{
	}
	public DefectBase(int ttid)
		: base(_Tabl,
					_allcols,
					ttid.ToString(),
					_ID)
	{
	}
	public DefectBase(string table, string[] columns, string id, string pcname = "ID", bool doload = true)
		: base(table, columns, id, pcname, doload)
	{
	}
	public List<DefectBase> EnumPlan(int userid)
	{
		List<int> wl = DefectDispo.EnumWorkable();
		string w_where = string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", wl));
		if (wl.Count < 1)
		{
			w_where = "";
		}

		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is not null) {3}) ORDER BY {2} DESC", _AsUser, userid, _Order, w_where);
		foreach (DataRow r in GetRecords(where))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
	public List<DefectBase> EnumUnPlan(int userid)
	{
		List<int> wl = DefectDispo.EnumWorkable();
		string w_where1 = string.Format(" AND  ({0} in ({1}))", _Disp, string.Join(",", wl));
		if (wl.Count < 1)
		{
			w_where1 = "";
		}

		List<int> pl = DefectSeverity.EnumPlanable();
		string w_where2 = string.Format(" AND  ({0} in ({1}))", _Seve, string.Join(",", pl));
		if (pl.Count < 1)
		{
			w_where2 = "";
		}

		List<DefectBase> ls = new List<DefectBase>();
		string where = string.Format(" WHERE (({0} = {1}) AND ({2} is null) {3} {4}) ORDER BY {5} DESC", _AsUser, userid, _Order, w_where1, w_where2, _ID);
		foreach (DataRow r in GetRecords(where))
		{
			DefectBase d = new DefectBase();
			d.Load(r);
			ls.Add(d);
		}
		return ls;
	}
}
public class Defect : DefectBase
{
	static ConcurrentDictionary<string, LockEvent> locker = new ConcurrentDictionary<string, LockEvent>();
	static Object thisLock = new Object();

	static protected string _Desc = "DESCR";
	static protected string _Specs = "ReproSteps";
	static protected string _Type = "idType";
	static protected string _Prod = "idProduct";
	static protected string _Ref = "Reference";
	static protected string _Prio = "idPriority";
	static protected string _Comp = "idCompon";
	static protected string _Seve = "idSeverity";
	static protected string _Date = "dateEnter";
	static protected string _Crea = "idCreateBy";
	static string[] _allcols = new string[] { _ID, _Specs, _Summ, _Desc, _idRec, _Type, _Prod, _Ref, _Disp, _Prio, _Comp, _Seve, _Date, _Crea, _Est, _Order, _AsUser };
	static string[] _allcolsNames = new string[] { _ID, "Specification", "Summary", "Description", _idRec, "Type", "Product", "Reference", "Disposition", "Priority", "Component", "Severity", "Date", "Created By", "Estimation", "Schedule Order", "Assigned User" };
	public static string _RepTable = "[TT_RES].[DBO].[REPORTBY]";

	public static void UnLocktask(string ttid, string lockid)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.lockid == lockid)
				{
					locker.TryRemove(ttid, out ev);
				}
			}
		}
	}
	public static bool Locked(string ttid)
	{
		if (!locker.Keys.Contains(ttid))
			return false;

		LockEvent ev = locker[ttid];
		return !ev.Obsolete;
	}
	public static LockInfo Locktask(string ttid, string lockid)
	{
		lock (thisLock)
		{
			if (locker.Keys.Contains(ttid))
			{
				LockEvent ev = locker[ttid];
				if (ev.Obsolete)
				{
					LockEvent newev = new LockEvent(lockid);
					locker[ttid] = newev;
					return new LockInfo(newev.usr, newev.lockid);
				}
				else
				{
					if (ev.lockid == lockid)
					{
						ev.Prolongate();
						locker[ttid] = ev;
					}
					return new LockInfo(ev.usr, ev.lockid);
				}
			}
			else
			{
				LockEvent ev = new LockEvent(lockid);
				locker[ttid] = ev;
				return new LockInfo(ev.usr, ev.lockid);
			}
		}
	}
	public static int GetRepRecByTTID(int id)
	{
		return Convert.ToInt32(GetRecdata(_RepTable, _idRec, "IDDEFREC", id));
	}
	protected override void OnChangeColumn(string col, string val)
	{
		for (int i = 0; i < _allcols.Length; i++)
		{
			if (_allcols[i] == col)
			{
				DefectHistory.AddHisotoryByTask(IDREC, _allcolsNames[i] + " changed.");
				return;
			}
		}
		base.OnChangeColumn(col, val);
	}
	protected override void OnProcessComplexColumn(string col, string val)
	{
		if (col == _Desc)
		{
			string sql = string.Format("UPDATE {2} SET DESCRPTN = {0} WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val, _id, _RepTable, _Tabl);
			SQLExecute(sql);
			return;
		}
		if (col == _Specs)
		{
			string sql = string.Format("UPDATE {2} SET REPROSTEPS = {0} WHERE IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {1})", val, _id, _RepTable, _Tabl);
			SQLExecute(sql);
			return;
		}
		else if (col == _Est)
		{
			DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.estimated, "", ESTIM);
			return;
		}
		else if (col == _AsUser)
		{
			DefectEvent.AddEventByTask(IDREC, DefectEvent.Eventtype.assigned, "", -1, Convert.ToInt32(AUSER));
			return;
		}
		throw new Exception("Unsupported column!");
	}
	protected override string OnTransformCol(string col)
	{
		if (col == _Desc)
		{
			return string.Format("(SELECT R.DESCRPTN FROM {2} R WHERE R.IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {0})) {1}", _id, _Desc, _RepTable, _Tabl);
		}
		else if (col == _Specs)
		{
			return string.Format("(SELECT R.REPROSTEPS FROM {2} R WHERE R.IDDEFREC = (SELECT IDRECORD FROM {3} D WHERE D.DEFECTNUM = {0})) {1}", _id, _Specs, _RepTable, _Tabl);
		}
		return base.OnTransformCol(col);
	}
	protected override bool IsColumnComplex(string col)
	{
		if (col == _Est || col == _AsUser)
			return true;

		return base.IsColumnComplex(col);
	}
	public string DESCR
	{
		get { return this[_Desc].ToString(); }
		set { this[_Desc] = value; }
	}
	public string SPECS
	{
		get { return this[_Specs].ToString(); }
		set { this[_Specs] = value; }
	}
	public string DATE
	{
		get { return (this[_Date] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(this[_Date])).ToString(defDateFormat, CultureInfo.InvariantCulture); }
		set { this[_Date] = Convert.ToDateTime(value, CultureInfo.InvariantCulture); }
	}
	public string CREATEDBY
	{
		get { return this[_Crea].ToString(); }
		set { this[_Crea] = Convert.ToUInt32(value); }
	}
	public string SEVE
	{
		get { return this[_Seve].ToString(); }
		set { this[_Seve] = Convert.ToInt32(value); }
	}
	public string COMP
	{
		get { return this[_Comp].ToString(); }
		set { this[_Comp] = Convert.ToInt32(value); }
	}
	public string PRIO
	{
		get { return this[_Prio].ToString(); }
		set { this[_Prio] = Convert.ToInt32(value); }
	}
	public string REFERENCE
	{
		get { return this[_Ref].ToString(); }
		set { this[_Ref] = value; }
	}
	public string TYPE
	{
		get { return this[_Type].ToString(); }
		set { this[_Type] = Convert.ToInt32(value); }
	}
	public string PRODUCT
	{
		get { return this[_Prod].ToString(); }
		set { this[_Prod] = Convert.ToInt32(value); }
	}
	public Defect()
		: base(_Tabl, _allcols, "0", _ID, false)
	{
	}
	public Defect(int ttid)
		: base(_Tabl,
					_allcols,
					ttid.ToString(),
					_ID)
	{
	}
	public static int GetIDbyTT(int tt)
	{
		return Convert.ToInt32(GetRecdata(_Tabl, _idRec, _ID, tt));
	}
	public static List<Defect> Enum()
	{
		List<Defect> ls = new List<Defect>();
		foreach (int ttid in EnumRecords(_Tabl, _ID, new string[] { _ID }, new object[] { 55555 }))
		{
			Defect d = new Defect(ttid);
			ls.Add(d);
		}
		return ls;
	}
}