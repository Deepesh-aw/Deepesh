var ProjectCreationApp = angular.module('ProjectCreationApp', ['CommonAppUtility'])

ProjectCreationApp.controller('ProjectCreationController', function ($scope, $http, CommonAppUtilityService) {
    $(function () {
        'use strict'
        //validation
        $('#frmProjectCreation').parsley().on('field:validated', function () {
            var ok = $('.parsley-error').length === 0;
            $('.bs-callout-info').toggleClass('hidden', !ok);
            $('.bs-callout-warning').toggleClass('hidden', ok);
        })
        .on('form:submit', function () {
            AddProject();
             return false;
        });
        // AmazeUI Datetimepicker
        $('#txtProjectStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtProjectEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

    });

    //$scope.AddProject = function () {
    function AddProject() {
        var MembersText = $("#ddlMembers option:selected").map(function () { return this.text }).get().join(', ');
        //var data = $("#frmProjectCreation").serialize();
        //data += "&Members=" + $("#ddlMembers").val() + "&MembersText=" + MembersText;
        //alert(data);
        var data = {
            'ProjectName': $scope.ngtxtProjectName,
            'StartDate': moment($("#txtProjectStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY"),
            'EndDate': moment($("#txtProjectEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY"),
            'ClientName': $scope.ngddlClientName,
            'ClientProjectManager': $scope.ngtxtClientProjectManager,
            'Description': $scope.ngtxtDescription,
            'Members': $("#ddlMembers").val(),
            'MembersText': MembersText,
            'NoOfDays': $scope.ngtxtNoOfDays,
            'ProjectManager': $scope.ngddlProjectManager,
            'ProjectType': $scope.ngddlProjectType,
        }

        CommonAppUtilityService.SendItem("/ProjectCreation/SaveProject", data).then(function (response) {
            if (response.status == 200) {
                alert("Done");
                //Pageredirect("/Approve");
            } else {
                alert("Error");
            }
        });
    }    
});



