﻿var AddTaskApp = angular.module('AddTaskApp', ['CommonAppUtility'])

AddTaskApp.controller('AddTaskController', function ($scope, $http, $timeout, CommonAppUtilityService) {

    $scope.Task = [];

    $(function () {

        $scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        $scope.MilestoneData = $.parseJSON($("#hdnMilestoneData").val());
        $scope.TaskData = $.parseJSON($("#hdnTaskData").val());
        angular.forEach($scope.TaskData, function (value, key) {
            var obj = {};
            obj.Task = value.Task;
            obj.Members = value.Members;
            obj.MemberTitle = value.MembersName;
            obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = value.NoOfDays;
            obj.TaskStatus = value.TaskStatus;
            obj.StatusName = value.TaskStatusName;
            obj.Project = $scope.ProjectData.ID;
            obj.Milestone = $scope.MilestoneData.ID;
            $scope.Task.push(obj);
        });
        $scope.$apply();

        // AmazeUI Datetimepicker
        $('#txtTaskStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtTaskEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });


        $('.date').datetimepicker().on('changeDate', function (e) {

            var start = moment($("#txtTaskStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtTaskEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            //check start and end date is between project dates
            var ProjectSdate = moment($scope.ProjectData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Task date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Task date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check start and end date is between milestone dates
            var MilestoneSdate = moment($scope.MilestoneData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var MilestoneEdate = moment($scope.MilestoneData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var msdate = moment(MilestoneSdate).format('YYYY/MM/DD');
            var medate = moment(MilestoneEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("Task date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("Task date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check end date should not be less than start date
            if ($('#txtTaskStartDate').val() != "" && $('#txtTaskEndDate').val() != "") {
                if (days <= 0) {
                    alert("End date shuold be less from start date");
                    $("#txtTaskEndDate").val('');
                    $("#txtTaskDays").val('');
                    $scope.ngtxtTaskDays = '';
                    return false;
                }
                $scope.ngtxtTaskDays = days;
                $("#txtTaskDays").val(days);
                if($("#txtTaskDays").hasClass("parsley-error")){
                    $("#txtTaskDays").removeClass("parsley-error");
                    $("#txtTaskDays").addClass("parsley-success");
                    $("#txtTaskDays").next().remove();
                }
            }
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

        $('input,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

        $('.select2').on('select2:selecting', function (e) {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).next().children().first().children().css("border-color", "#22c03c");
                $(this).siblings("li").remove();
            }
        });
    });

    $scope.BackToDashboard = function () {
        $.removeCookie('ProjectId');
        $.removeCookie('MilestoneId');
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_ProjectDashboard' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.DeleteTask = function (index) {
        $scope.Task.splice(index, 1);
    }

    function clearErrorClass() {
        $('input').removeClass("parsley-success");
        $('input').removeClass("parsley-error");
        $('#ddlMember').next().children().first().children().css("border-color", "#e1e5ef");
        $("#ddlStatus").next().children().first().children().css("border-color", "#e1e5ef");
        $(".parsley-errors-list").remove();
    }

    $scope.ValidateRequest = function () {
        clearErrorClass();
        var rv = true;
        if ($scope.ngtxtTask == "" || $scope.ngtxtTask == undefined || $scope.ngtxtTask == null)
        {
            $("#txtTask").addClass("parsley-error");
            $("#txtTask").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtTaskStartDate == "" || $scope.ngtxtTaskStartDate == undefined || $scope.ngtxtTaskStartDate == null)
        {
            $("#txtTaskStartDate").addClass("parsley-error");
            $("#txtTaskStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

		if($scope.ngtxtTaskEndDate == "" || $scope.ngtxtTaskEndDate == undefined || $scope.ngtxtTaskEndDate == null)
		{
            $("#txtTaskEndDate").addClass("parsley-error");
            $("#txtTaskEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngddlMember == "" || $scope.ngddlMember == undefined || $scope.ngddlMember == null)
        {
            $("#ddlMember").addClass("parsley-error");
            $("#ddlMember").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlMember").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngddlStatus == "" || $scope.ngddlStatus == undefined || $scope.ngddlStatus == null)
        {
            $("#ddlStatus").addClass("parsley-error");
            $("#ddlStatus").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtTaskDays == "" || $scope.ngtxtTaskDays == undefined || $scope.ngtxtTaskDays == null)
        {
            $("#txtTaskDays").addClass("parsley-error");
            $("#txtTaskDays").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }

    $scope.AddTask = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {
            var obj = {};
            obj.Task = $scope.ngtxtTask;
            obj.Members = $scope.ngddlMember;
            obj.MemberTitle = $("#ddlMember option:selected").text();
            obj.StartDateView = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtTaskDays;
            obj.TaskStatus = $scope.ngddlStatus;
            obj.StatusName = $("#ddlStatus option:selected").text();
            obj.Project = $scope.ProjectData.ID;
            obj.Milestone = $scope.MilestoneData.ID;
            $scope.Task.push(obj);

            //clear all the fields
            $scope.ngddlMember = "";
            $scope.ngddlStatus = "";
            $scope.ngtxtTask = "";
            $scope.ngtxtMileDescription = "";
            $("#txtTaskStartDate").val('');
            $("#txtTaskEndDate").val('');
            $scope.ngtxtTaskStartDate = "";
            $scope.ngtxtTaskEndDate = "";
            $scope.ngtxtTaskDays = "";
            $timeout(function () {
                $("#ddlMember").trigger('change');
                $("#ddlStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditTask = function (index) {
        $scope.ngtxtTask = $scope.Task[index].Task;
        $scope.ngtxtTaskStartDate = $scope.Task[index].StartDate;
        $("#txtTaskStartDate").val($scope.Task[index].StartDate);
        $scope.ngtxtTaskEndDate = $scope.Task[index].EndDate;
        $("#txtTaskEndDate").val($scope.Task[index].EndDate);
        $scope.ngtxtTaskDays = $scope.Task[index].NoOfDays;

        $timeout(function () {
            $("#ddlMember").val($scope.Task[index].Members).trigger('change');
            $("#ddlStatus").val($scope.Task[index].TaskStatus).trigger('change');
            $scope.Task.splice(index, 1);
        }, 10);

    }

    $scope.FinalAddTask = function () {
        if ($scope.Task.length > 0) {
            $scope.TaskLoad = true;
            var AddTask = new Array();
            AddTask = $scope.Task;

            CommonAppUtilityService.CreateItem("/TIM_AddTask/AddTask", AddTask).then(function (response) {
                if (response.data[0] == "OK") {
                    $('#SuccessModelTask').modal('show');
                    $scope.TaskLoad = false;
                }
                else 
                    $scope.TaskLoad = false;
            });
        }
        else
            $scope.ValidateRequest();
    }

});



