var MilestoneDetailsApp = angular.module('MilestoneDetailsApp', ['CommonAppUtility'])

MilestoneDetailsApp.controller('MilestoneDetailController', function ($scope, $http, CommonAppUtilityService) {

    $scope.AddTask = function (MilestoneId) {
        $.cookie('MilestoneId', MilestoneId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }
});