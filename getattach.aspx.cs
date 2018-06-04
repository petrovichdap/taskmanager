﻿using GTOHELPER;
using System;
using System.IO;

public partial class GetAttach : GTOHelper
{
	private string ReturnExtension(string fileExt)
	{
		switch (fileExt.ToLower())
		{
			case ".txt":
			case ".las": return "text/plain";
			case ".pdf": return "application/pdf";
			case ".swf": return "application/x-shockwave-flash";
			case ".gif": return "image/gif";
			case ".jpeg": return "image/jpg";
			case ".jpg": return "image/jpg";
			case ".png": return "image/png";
			case ".mp4": return "video/mp4";
			case ".mpeg": return "video/mpeg";
			case ".mov": return "video/quicktime";
			case ".wmv":
			case ".avi": return "video/x-ms-wmv";
			default: return "application/octet-stream";
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		int id = Convert.ToInt32(Request.QueryString["idrecord"]);
		DefectAttach d = new DefectAttach(id);
		string ext = Path.GetExtension(d.FILENAME);
		Response.ClearContent();
		Response.ClearHeaders();

		Response.ContentType = ReturnExtension(ext);
		Response.AddHeader("Content-Length", d.SIZE.ToString());
		Response.AddHeader("Content-Disposition", string.Format("filename=\"{0}\"", d.FILENAME));
		byte[] bytes = d.FileBinary();
		Response.OutputStream.Write(bytes, 0, bytes.Length);
		Response.Flush();
		Context.ApplicationInstance.CompleteRequest();
	}
}