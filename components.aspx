﻿<%@ Page Title="Components" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="components.aspx.cs" Inherits="Components" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/components_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/components_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Component name</th>
					<th>Order</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="c in comps">
					<td ng-click="enterdata(c, 'DESCR')">{{c.DESCR}}</td>
					<td ng-click="enterdata(c, 'FORDER')">{{c.FORDER}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>