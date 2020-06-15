var ProjectCreationApp = angular.module('ProjectCreationApp', ['CommonAppUtility'])

ProjectCreationApp.controller('ProjectCreationController', function ($scope, $http, $timeout, CommonAppUtilityService) {
    var ProjectId = $.cookie("ProjectId");
    $scope.ProjectDetails = [];

    if (ProjectId != null || ProjectId != undefined) {
        CommonAppUtilityService.CreateItem("/TIM_ProjectCreation/GetEditProject", "").then(function (response) {
            if (response.data[0] == "OK") {
                
                $scope.ProjectDetails = response.data[1];
                $scope.ngtxtProjectName = $scope.ProjectDetails[0].ProjectName;
                $scope.ngtxtClientProjectManager = $scope.ProjectDetails[0].ClientProjectManager;
                $scope.ngtxtProjectStartDate = $scope.ProjectDetails[0].StartDate.split(' ')[0];
                $scope.ngtxtProjectEndDate = $scope.ProjectDetails[0].EndDate.split(' ')[0];
                $scope.ngtxtNoOfDays = $scope.ProjectDetails[0].NoOfDays;
                $scope.ngtxtDescription = $scope.ProjectDetails[0].Description;
                $timeout(function () {
                    var MemberVal = [];
                    angular.forEach($scope.ProjectDetails[0].Members, function (value, key) {
                        MemberVal.push(value);
                    });
                    $("#ddlClientName").val($scope.ProjectDetails[0].ClientName).trigger('change');
                    $("#ddlProjectType").val($scope.ProjectDetails[0].ProjectType).trigger('change');
                    $("#ddlProjectManager").val($scope.ProjectDetails[0].ProjectManager).trigger('change');
                    $("#ddlMembers").val(MemberVal).trigger('change');
                });

            }
            else {

            }
        });

        $("#btnProjectCreation").text("Update");
    }


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

        $('.date').datetimepicker().on('changeDate', function (e) {
            if ($('#txtProjectStartDate').val() != "" && $('#txtProjectEndDate').val() != "") {
                var start = moment($("#txtProjectStartDate").val(), 'DD/MM/YYYY');
                var end = moment($("#txtProjectEndDate").val(), 'DD/MM/YYYY');
                var days = end.diff(start, 'days');
                if (days <= 0) {
                    alert("End date should not be less than or equal to the start date");
                    $("#txtProjectEndDate").val('');
                    $("#txtNoOfDays").val('');
                    $scope.ngtxtNoOfDays = '';
                    return false;
                }
                $scope.ngtxtNoOfDays = days;
                $("#txtNoOfDays").val(days);
            }
        });		

    });

    function AddProject() {
        $scope.ProjectCreationLoad = true;
        var MembersText = $("#ddlMembers option:selected").map(function () { return this.text }).get().join(', ');
        var MembersCode = $("#ddlMembers option:selected").map(function () { return this.id }).get().join(', ');
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
            'MembersCodeText': MembersCode,
            'MembersText': MembersText,
            'NoOfDays': $scope.ngtxtNoOfDays,
            'ProjectManager': $scope.ngddlProjectManager,
            'ProjectType': $scope.ngddlProjectType,
        }
        if ($scope.ProjectDetails.length > 0)
            data.ID = ProjectId;

        CommonAppUtilityService.CreateItem("/TIM_ProjectCreation/SaveProject", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.ProjectCreationLoad = false;
                $('#SuccessModelProject').modal('show');
            }
            else
                $scope.ProjectCreationLoad = false;
        });
    } 

    $scope.BackToDashboard = function () {
        if ($scope.ProjectDetails.length > 0)
            $.removeCookie('ProjectId');

        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_ProjectDashboard' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }
});



