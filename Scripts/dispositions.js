﻿$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var dispos = [];
			for (var i = 0; i < $scope.dispos.length; i++) {
				var ch = $scope.dispos[i].changed;
				if (ch) {
					delete $scope.dispos[i].changed;
					dispos.push($scope.dispos[i])
				}
			}
			$http.post("trservice.asmx/setdispos", angular.toJson({ "data": dispos }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}

		getDispos($scope, "dispos", $http);

		$scope.changed = false;
		$scope.enterdata = function (object, prop) {
			var oldval = object[prop];
			var newvalue = window.prompt("Please enter the value", oldval);
			if (newvalue == null || newvalue == "") {
				return;
			}
			if (newvalue != oldval) {
				object[prop] = newvalue;
				object.changed = true;
				$scope.changed = true;
			}
		}
	}]);
})