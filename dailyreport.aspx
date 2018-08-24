﻿<%@ Page Title="Daily Report" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dailyreport.aspx.cs" Inherits="DailyReport" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dailyreport_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dailyreport_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<img id="pageloadnotify" src="pageloadnotify.ashx?id=<%=PageLoadNofify.NewLoad()%>" />
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-primary">
			<div class="panel-heading">
				<div class="row">
					<div class="col-sm-2">
						<h4>Daily Report for date: </h4>
					</div>
					<div class="col-sm-2">
						<input type="date" id="date" class="form-control input-sm" ng-model="today" ng-change="changeDate()" ng-disabled="!loaded()">
					</div>
					<div class="col-sm-8">
						<h4>Total persons: {{mpsusers.length}}&nbsp Online persons: {{personsOnline}}&nbsp Vacations: {{personsVacation}}&nbsp Away: {{mpsusers.length - personsOnline - personsVacation}}</h4>
					</div>
				</div>
			</div>
			<div class="panel-body">
				<table class="table table-hover table-bordered">
					<colgroup>
						<col width="10%" />
						<col width="30%" />
						<col width="30%" />
						<col width="30%" />
					</colgroup>
					<thead>
						<tr class="info">
							<th>Person</th>
							<th>Yesterday</th>
							<th>Today</th>
							<th>Plan</th>
						</tr>
					</thead>
					<tbody>
						<tr ng-repeat="u in mpsusers | orderBy : 'PERSON_NAME'">
							<td>
								<img ng-src="{{'getUserImg.ashx?id=' + u.ID}}" alt="Smile" height="20" width="20">
								<a data-toggle="tooltip" title="Click to see all user's activity." target="_blank" href="dailysearch.aspx?userid={{u.ID}}" class="btn btn-default btn-xs" role="button">{{u.PERSON_NAME}}</a>
							</td>
							<td>
								<div ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID, DATE: yesterdaystring }">
									<a href="showtask.aspx?ttid={{v.ID}}" target="_blank">
										<h3><span class="glyphicon glyphicon-plane"></span></h3>
									</a>
								</div>
								<div ng-repeat="l in u.YESTERDAY track by $index">
									<span>{{l | limitTo:80}}</span><br>
								</div>
								<hr>
								<div ng-hide="u.CREATEDTASKS1.length < 1">
									<strong>Created tasks:</strong><a ng-repeat="t in u.CREATEDTASKS1" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
								<div ng-hide="u.SCHEDULEDTASKS1.length < 1">
									<strong>Scheduled tasks:</strong><a ng-repeat="t in u.SCHEDULEDTASKS1" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
								<div ng-hide="u.MODIFIEDTASKS1.length < 1">
									<strong>Modified tasks:</strong><a ng-repeat="t in u.MODIFIEDTASKS1" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
							</td>
							<td>
								<div ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID, DATE: todaystring }">
									<a href="showtask.aspx?ttid={{v.ID}}" target="_blank">
										<h3><span class="glyphicon glyphicon-plane"></span></h3>
									</a>
								</div>
								<div ng-repeat="l in u.TODAY track by $index">
									<span>{{l | limitTo:80}}</span><br>
								</div>
								<hr>
								<div ng-hide="u.CREATEDTASKS2.length < 1">
									<strong>Created tasks:</strong><a ng-repeat="t in u.CREATEDTASKS2" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
								<div ng-hide="u.SCHEDULEDTASKS2.length < 1">
									<strong>Scheduled tasks:</strong><a ng-repeat="t in u.SCHEDULEDTASKS2" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
								<div ng-hide="u.MODIFIEDTASKS2.length < 1">
									<strong>Modified tasks:</strong><a ng-repeat="t in u.MODIFIEDTASKS2" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge">{{t}}</span></a>
								</div>
							</td>
							<td>
								<img src="IMAGES/process.gif" ng-hide="planLoaded(u.ID)" />
								<div>
									<a target='_blank' href='vacations.aspx'>
										<h3 class="vacation-box">{{getUpcomingdays(u)}}</h3>
									</a>
									<a href="showtask.aspx?ttid={{v.ID}}" target="_blank" ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID } | filter: {DATE: '!' + todaystring} | filter: {DATE: '!' + yesterdaystring}">
										<h3 class="vacation-box"><span class="glyphicon glyphicon-plane"></span></h3>
									</a>
								</div>
								<div ng-repeat="d in u.PLAN track by $index" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task {{$index < 10 ? 'task-first' : 'task-last'}}">
									<div>
										<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
											<span class="badge">{{d.ID}}</span>
										</a>
										<span class="label label-danger">{{d.ESTIM}}</span>
										<span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:80}}</span>
									</div>
								</div>
								<button data-toggle="tooltip" title="Click to see additional 10 items. Number of items is limited by 20. For more items click plan button near" onclick="moretasks(this)" type="button" class="btn btn-default btn-xs">...</button>
								<a data-toggle="tooltip" title="Click to see full plan for selected person" target="_blank" href="editplan.aspx?userid={{u.ID}}" class="btn btn-default btn-xs" role="button">Full plan...</a>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</div>
</asp:Content>
