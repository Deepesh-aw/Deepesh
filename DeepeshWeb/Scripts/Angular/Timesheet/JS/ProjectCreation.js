var ProjectCreationApp = angular.module('ProjectCreationApp', ['ProjectCreationServiceModule'])

ProjectCreationApp.controller('ProjectCreationController', function ($scope, $http, ProjectCreationService) {

    $scope.AddProject = function () {
        var MembersText = $("#ddlMembers option:selected").map(function () { return this.text }).get().join(', ');
        var data = $("#frmProjectCreation").serialize();
        data += "&Members=" + $("#ddlMembers").val() + "&MembersText=" + MembersText;
        alert(data);
        ProjectCreationService.SaveProject(data).then(function (response) {
            if (response.status == 200) {
                alert("Done");
                //Pageredirect("/Approve");
            } else {
                alert("Error");
            }
        });

    }
});
