var AddSubTaskApp = angular.module('AddSubTaskApp', ['CommonAppUtility'])

AddSubTaskApp.controller('AddSubTaskController', function ($scope, $http, $timeout, CommonAppUtilityService) {

    $scope.SubTask = [];

    $(function () {

        $scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        $scope.MilestoneData = $.parseJSON($("#hdnMilestoneData").val());
        $scope.TaskData = $.parseJSON($("#hdnTaskData").val());

        'use strict'
        // AmazeUI Datetimepicker
        $('#txtSubTaskStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtSubTaskEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });


        $('.date').datetimepicker().on('changeDate', function (e) {

            var start = moment($("#txtSubTaskStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtSubTaskEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            //check start and end date is between project dates
            var ProjectSdate = moment($scope.ProjectData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("SubTask date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("SubTask date range should be within the Project date range");
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
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check start and end date is between Task dates
            var TaskSdate = moment($scope.TaskData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var TaskEdate = moment($scope.TaskData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var tsdate = moment(TaskSdate).format('YYYY/MM/DD');
            var tedate = moment(TaskEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(tsdate, tedate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(tsdate, tedate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check end date should not be less than start date
            if ($('#txtSubTaskStartDate').val() != "" && $('#txtSubTaskEndDate').val() != "") {
                if (days <= 0) {
                    alert("End date should be less from start date");
                    $("#txtSubTaskEndDate").val('');
                    $("#txtSubTaskDays").val('');
                    $scope.ngtxtSubTaskDays = '';
                    return false;
                }
                $scope.ngtxtSubTaskDays = days;
                $("#txtSubTaskDays").val(days);
                if ($("#txtSubTaskDays").hasClass("parsley-error")) {
                    $("#txtSubTaskDays").removeClass("parsley-error");
                    $("#txtSubTaskDays").addClass("parsley-success");
                    $("#txtSubTaskDays").next().remove();
                }
            }
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

        $('input,select,textarea').keypress(function () {
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
        $.removeCookie('TaskId');
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_ProjectDashboard' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }


    $scope.DeleteSubTask = function (index) {
        $scope.SubTask.splice(index, 1);
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
        if ($scope.ngtxtSubTask == "" || $scope.ngtxtSubTask == undefined || $scope.ngtxtSubTask == null) {
            $("#txtSubTask").addClass("parsley-error");
            $("#txtSubTask").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskStartDate == "" || $scope.ngtxtSubTaskStartDate == undefined || $scope.ngtxtSubTaskStartDate == null) {
            $("#txtSubTaskStartDate").addClass("parsley-error");
            $("#txtSubTaskStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskEndDate == "" || $scope.ngtxtSubTaskEndDate == undefined || $scope.ngtxtSubTaskEndDate == null) {
            $("#txtSubTaskEndDate").addClass("parsley-error");
            $("#txtSubTaskEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngddlStatus == "" || $scope.ngddlStatus == undefined || $scope.ngddlStatus == null) {
            $("#ddlStatus").addClass("parsley-error");
            $("#ddlStatus").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngddlMember == "" || $scope.ngddlMember == undefined || $scope.ngddlMember == null) {
            $("#ddlMember").addClass("parsley-error");
            $("#ddlMember").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlMember").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskDays == "" || $scope.ngtxtSubTaskDays == undefined || $scope.ngtxtSubTaskDays == null) {
            $("#txtSubTaskDays").addClass("parsley-error");
            $("#txtSubTaskDays").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }

    $scope.AddSubTask = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {
            var obj = {};
            obj.SubTask = $scope.ngtxtSubTask;
            obj.Members = $scope.ngddlMember;
            obj.MemberTitle = $("#ddlMember option:selected").text();
            obj.StartDateView = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtSubTaskDays;
            obj.SubTaskStatus = $scope.ngddlStatus;
            obj.StatusName = $("#ddlStatus option:selected").text();
            obj.Project = $scope.ProjectData.ID;
            obj.Milestone = $scope.MilestoneData.ID;
            obj.Task = $scope.TaskData.ID;
            $scope.SubTask.push(obj);

            //clear all the fields
            $scope.ngddlMember = "";
            $scope.ngddlStatus = "";
            $scope.ngtxtSubTask = "";
            $scope.ngtxtMileDescription = "";
            $("#txtSubTaskStartDate").val('');
            $("#txtSubTaskEndDate").val('');
            $scope.ngtxtSubTaskDays = "";
            $timeout(function () {
                $("#ddlMember").trigger('change');
                $("#ddlStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditSubTask = function (index) {
        $scope.ngtxtSubTask = $scope.SubTask[index].SubTask;
        $scope.ngtxtSubTaskStartDate = $scope.SubTask[index].StartDate;
        $("#txtSubTaskStartDate").val($scope.SubTask[index].StartDate);
        $scope.ngtxtSubTaskEndDate = $scope.SubTask[index].EndDate;
        $("#txtSubTaskEndDate").val($scope.SubTask[index].EndDate);
        $scope.ngtxtSubTaskDays = $scope.SubTask[index].NoOfDays;

        $timeout(function () {
            $("#ddlMember").val($scope.SubTask[index].Members).trigger('change');
            $("#ddlStatus").val($scope.SubTask[index].SubTaskStatus).trigger('change');
            $scope.SubTask.splice(index, 1);
        }, 10);

    }

    $scope.FinalAddSubTask = function () {
        if ($scope.SubTask.length > 0) {
            $scope.SubTaskLoad = true;
            var AddSubTask = new Array();
            AddSubTask = $scope.SubTask;

            CommonAppUtilityService.CreateItem("/TIM_AddSubTask/AddSubTask", AddSubTask).then(function (response) {
                if (response.data[0] == "OK") {
                    $('#SuccessModelSubTask').modal('show');
                    $scope.SubTaskLoad = false;
                }
                else
                    $scope.SubTaskLoad = false;
            });
        }
        else
            $scope.ValidateRequest();
    }

});



