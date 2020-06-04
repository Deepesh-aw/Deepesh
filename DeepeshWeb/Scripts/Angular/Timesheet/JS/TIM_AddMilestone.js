var AddMilestoneApp = angular.module('AddMilestoneApp', ['CommonAppUtility'])

AddMilestoneApp.controller('AddMilestoneController', function ($scope, $http, CommonAppUtilityService) {

    $scope.Milestone = [];

    $(function () {

        $scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        console.log($scope.ProjectData);
        'use strict'
        // AmazeUI Datetimepicker
        $('#txtMileStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtMileEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        
        $('.date').datetimepicker().on('changeDate', function (e)
        {
            var start = moment($("#txtMileStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtMileEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            var ProjectSdate = moment($scope.ProjectData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            //check start and end date is between project dates
            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Milestone date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Milestone date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check end date should not be less than start date				
            if ($('#txtMileStartDate').val() != "" && $('#txtMileEndDate').val() != "") {
                if (days <= 0) {
                    alert("End date shuold be less from start date");
                    $("#txtMileEndDate").val('');
                    $("#txtMileDays").val('');
                    $scope.ngtxtMileDays = '';
                    return false;
                }
                $scope.ngtxtMileDays = days;
                $("#txtMileDays").val(days);
            }
        });	

        $('input,select,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")){
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }

        });

    });

    $scope.DeleteMilestone = function (index) {
        $scope.Milestone.splice(index, 1);
    }

    function clearErrorClass() {
        $('input').removeClass("parsley-success");
        $('input').removeClass("parsley-error");
        $(".parsley-errors-list").remove();
    }

    $scope.ValidateRequest = function () {
        clearErrorClass();
        var rv = true;
        if ($scope.ngtxtMilestone == "" || $scope.ngtxtMilestone == undefined || $scope.ngtxtMilestone == null) {
            $("#txtMilestone").addClass("parsley-error"); 
            $("#txtMilestone").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileDescription == "" || $scope.ngtxtMileDescription == undefined || $scope.ngtxtMileDescription == null) {
            $("#txtMileDescription").addClass("parsley-error"); 
            $("#txtMileDescription").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileStartDate == "" || $scope.ngtxtMileStartDate == undefined || $scope.ngtxtMileStartDate == null) {
            $("#txtMileStartDate").addClass("parsley-error"); 
            $("#txtMileStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileEndDate == "" || $scope.ngtxtMileEndDate == undefined || $scope.ngtxtMileEndDate == null) {
            $("#txtMileEndDate").addClass("parsley-error"); 
            $("#txtMileEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }


    $scope.AddMilestone = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {

            var obj = {};
            obj.Milestone = $scope.ngtxtMilestone;
            obj.Description = $scope.ngtxtMileDescription;
            obj.StartDate = moment($("#txtMileStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY");
            obj.EndDate = moment($("#txtMileEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY");
            obj.NoOfDays = $scope.ngtxtMileDays;
            obj.Project = $scope.ProjectData.Id;
            obj.ProjectManager = $scope.ProjectData.ProjectManager;
            obj.MembersText = $scope.ProjectData.MembersText;
            obj.Members = $scope.ProjectData.Members;
            $scope.Milestone.push(obj);

            //clear all the fields
            $scope.ngtxtMilestone = "";
            $scope.ngtxtMileDescription = "";
            $("#txtMileStartDate").val('');
            $("#txtMileEndDate").val('');
            $("#txtMileDays").val('');
            $scope.ngtxtMileDays = '';
        }
    }

    $scope.EditMilestone = function (index) {
        $scope.ngtxtMilestone = $scope.Milestone[index].Milestone;
        $scope.ngtxtMileDescription = $scope.Milestone[index].Description;
        $scope.ngtxtMileStartDate = $scope.Milestone[index].StartDate;
        $("#txtMileStartDate").val($scope.Milestone[index].StartDate);
        $scope.ngtxtMileEndDate = $scope.Milestone[index].EndDate;
        $("#txtMileEndDate").val($scope.Milestone[index].EndDate);
        $scope.ngtxtMileDays = $scope.Milestone[index].NoOfDays;
        $scope.Milestone.splice(index, 1);
    }


    $scope.FinalAddMilestone = function() {
        var AddMilestone = new Array();
        AddMilestone = $scope.Milestone;

        CommonAppUtilityService.CreateItem("/TIM_AddMilestone/AddMilestone", AddMilestone).then(function (response) {
            if (response.data[0] == "OK")
                $('#SuccessModelMilestone').modal('show');

        });
    }

    $scope.SaveRedirect = function () {
        
    }
});



