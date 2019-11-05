﻿<%@ Page Title="Tracker" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="tracker.aspx.cs" Inherits="TrackerPage" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectVerControl.ascx" TagName="defVer" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectUsrControl.ascx" TagName="defUsr" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/tracker_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/tracker_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script <%="src='" + Settings.CurrentSettings.CHARTSJSCDN.ToString() + "Chart.bundle.min.js'" %>></script>
	<script src="scripts/userimg.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="trackers" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(Tracker.Enum(CurrentContext.TTUSERID))%>' />
	<div ng-app="mpsapplication" ng-controller="mpscontroller" ng-cloak>
		<div class="row">
			<div class="btn-group mx-auto">
				<button type="button" class="btn btn-outline-light text-dark" data-toggle="dropdown">
					<h2><i class="fas fa-file-contract"></i>&nbsp;{{pageName}}</h2>
				</button>
				<div class="dropdown-menu">
					<a ng-repeat="t in trackers" href="?id={{t.ID}}" class="dropdown-item">{{t.NAME}}</a>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="col-md-3">
				<div class="shadow mb-2 mt-2">
					<canvas id="chartpie" width="1400" height="1400"></canvas>
				</div>
			</div>
			<div class="col-md-6 mb-2">
				<input ng-model="newtask" type="text" class="form-control form-control-sm" onkeydown="return event.key != 'Enter';" ng-keyup="messageKey($event)" ng-show="isadmin && simpleTracker">
				<div class="list-group shadow">
					<div ng-repeat="d in defects" class="list-group-item p-1" ng-style="{{d.DISPO | getDispoColorById:this}}">
						<uc:defNum runat="server" />
						<uc:defEst runat="server" />
						<span data-toggle="tooltip" title="{{d.SUMMARY}}" ng-bind-html="d.SUMMARY | sumFormat | limitTo:135"></span>
						<uc:defUsr onchange="console.log('ddd')" ng-show="isadmin" runat="server" class="float-right" />
						<uc:defVer runat="server" />
					</div>
				</div>
			</div>
			<div class="col-md-3">
				<div class="card shadow mb-2">
					<div class="card-body text-center">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=150&ttid=' + pageLogo()}}" alt="Smile" height="150" width="150" />
					</div>
				</div>
				<div class="shadow">
					<div class="list-group mb-2">
						<div ng-repeat="t in trackers" class="d-flex mb-2 ">
							<a href="?id={{t.ID}}" class="list-group-item list-group-item-action shadow">{{t.NAME}}</a>
							<div class="btn-group" ng-show="isadmin">
								<button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
									<img data-toggle="tooltip" title="<img src='{{'getUserImg.ashx?sz=150&ttid=' + t.IDCLIENT}}' />" class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=25&ttid=' + t.IDCLIENT}}" alt="Smile" height="25" width="25" />
								</button>
								<div class="dropdown-menu dropdown-menu-right">
									<div class="dropdown-item" ng-repeat="u in users | filter : {ACTIVE: true}" style="cursor: pointer" ng-click="assignToClient(t.ID, u.ID)">
										<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&ttid=' + u.ID}}" alt="Smile" height="20" width="20" />
										{{u.FULLNAME}}
									</div>
								</div>
							</div>
							<button ng-show="isadmin" type="button" class="close" ng-click="delTracker(t.ID)">&times;</button>
						</div>
						<div class="d-flex">
							<div class="btn-group flex-fill" ng-show="isadmin">
								<button type="button" class="btn btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
									Add new track list
								</button>
								<div class="dropdown-menu">
									<a class="dropdown-item" href ng-repeat="f in filters" ng-click="addTracker(f.ID, f.NAME)" style="cursor: pointer">
										<i class="fas fa-user-friends text-danger" ng-show="f.SHARED"></i>
										<i class="fas fa-user" ng-hide="f.SHARED"></i>
										{{f.NAME}}
									</a>
								</div>
							</div>
							<button type="button" class="btn btn-outline-secondary flex-fill" ng-click="addTracker()">Simple Tracker</button>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
