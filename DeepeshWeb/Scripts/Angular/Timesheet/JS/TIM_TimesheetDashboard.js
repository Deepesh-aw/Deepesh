﻿
var TimesheetDashboardApp = angular.module('TimesheetDashboardApp', ['CommonAppUtility'])

TimesheetDashboardApp.controller('TimesheetDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {
    $scope.TimesheetData = [];
    $scope.TimesheetDataPending = [];
    $scope.TimesheetDataApproved = [];
    $scope.AllTaskArr = [];
    var WorkingHour;
    $scope.ClientDetails = [];
    $scope.TimesheetArr = [];
    $scope.EditTimesheetArr = [];
    $scope.ngtxtTimesheetDate;
    $scope.TimeId ;
    $scope.CurrentTask = [];
    var ExistUtilize = 0;

    $(function () {

        //$scope.OnLoad();

        //$('#Pending').DataTable({
        //    responsive: true,
        //    language: {
        //        searchPlaceholder: 'Search...',
        //        sSearch: '',
        //        lengthMenu: '_MENU_ ',
        //    }
        //});

        //$('#Approved').DataTable({
        //    responsive: true,
        //    language: {
        //        searchPlaceholder: 'Search...',
        //        sSearch: '',
        //        lengthMenu: '_MENU_ ',
        //    }
        //});

        $('#frmTimesheetDashboard').parsley().on('field:validated', function () {
            var ok = $('.parsley-error').length === 0;
            $('.bs-callout-info').toggleClass('hidden', !ok);
            $('.bs-callout-warning').toggleClass('hidden', ok);
        })
            .on('form:submit', function () {
                $scope.AddTimesheet();
                return false;
            });


        $('.Time').datetimepicker({
            formatViewType: 'time',
            format: 'hh:ii',
            startView: 1,
            pickDate: false,
            autoclose: true,
            //minuteStep: 60
        }).on("show", function () {
            $(".table-condensed .prev").css('visibility', 'hidden');
            $(".table-condensed .switch").text("Pick Time").css('visibility', 'hidden');
            $(".table-condensed .next").css('visibility', 'hidden');
        }).on('changeDate', function (e) {
            var From = $('#txtFromTime').val();
            var To = $('#txtToTime').val();
            var CurrTime = this.value;
            var Id = this.id;
            var Frm = moment($scope.ngtxtTimesheetDate + " " + From, 'DD-MM-YYYY hh:mm').format("hh:mm a");
            var to = moment($scope.ngtxtTimesheetDate + " " + To, 'DD-MM-YYYY hh:mm').format("hh:mm a");
            var tFrom = moment(Frm, 'h:mma');
            var tTo = moment(to, 'h:mma');

            if ($('#txtFromTime').val() != "" && $('#txtToTime').val() != "") {
                if (tTo.isBefore(tFrom)) {
                    alert("From time should be less than To time");
                    $("#" + Id).val('');
                    $("#" + Id).focus();
                    return false;
                }
                else {
                    var diff = moment.duration(tTo.diff(tFrom));
                    //var totalMin = diff.hours() * 60 + diff.minutes();
                    $scope.ngtxtHours = diff.hours() + ':' + (diff.minutes() < 10 ? "0" : '') + diff.minutes();
                    $("#txtHours").val($scope.ngtxtHours);
                    $scope.ngtxtUtilizedHours = ExistUtilize;
                    $scope.GetRemainingHorus();
                }
            }
            if ($scope.TimesheetArr.length > 0) {
                angular.forEach($scope.TimesheetArr, function (value, key) {
                    
                    var Now = moment($scope.ngtxtTimesheetDate + " " + CurrTime, 'DD-MM-YYYY hh:mm').format("hh:mm a");
                    var FromTime = moment(value.FromTime).format("hh:mm a");
                    var ToTime = moment(value.ToTime).format("hh:mm a");

                    var startTime = moment(FromTime, 'h:mma');
                    var endTime = moment(ToTime, 'h:mma');
                    var inputtedTime = moment(Now, 'h:mma');
                    var IsBetween = false;

                    

                    //IsBetween = inputtedTime.isBetween(startTime, endTime, undefined, '[]');
                    if (Id == "txtFromTime") {
                        if (inputtedTime.isSame(startTime))
                            IsBetween = inputtedTime.isBetween(startTime, endTime, undefined, '[]');
                        else
                            IsBetween = inputtedTime.isBetween(startTime, endTime, undefined, '[)');
                    }

                    if (IsBetween != true && Id == "txtToTime") {
                        if (inputtedTime.isSame(endTime))
                            IsBetween = inputtedTime.isBetween(startTime, endTime, undefined, '[]');
                        else
                            IsBetween = inputtedTime.isBetween(startTime, endTime, undefined, '(]');
                    }
                    

                    if (IsBetween == true) {
                        alert("This Time already added. Please try another one.");
                        $("#" + Id).val('');
                        $("#" + Id).focus();
                        return false;
                    }
                })
            }
        }).on('hide', function (event) {
            return false;
        });
        

        $(".AllSelect").select2({
            placeholder: 'Choose one',
            searchInputPlaceholder: 'Search',
            width: '100%',
            dropdownParent: $("#AddTimesheetPopUp")
        })

        $('#AddTimesheetPopUp').on('hide.bs.modal', function () {
            $scope.ViewTimesheet = false;
            $scope.TimeId = "TIM-" + $scope.TimesheetData.length + 1;
            $("#btnAddTimesheet").text("Submit");
            $scope.TimesheetArr.length = 0;
            $scope.EditTimesheetArr.length = 0;
            $("#txtUtilizedHours").val('');
            $scope.ngtxtDescription = undefined;
            $scope.ngtxtHours = undefined;
            $scope.ngtxtEstimatedHours = undefined;
            $scope.ngtxtRemainingHours = undefined;
            $("#txtFromTime").val('');
            $("#txtToTime").val('');

            $timeout(function () {
                $("#ddlClient").val('').trigger('change');
                $("#ddlAllTaskStatus").val('').trigger('change');

            }, 10);

            Array.from(document.getElementsByClassName('parsley-success')).forEach(function (el) {
                el.classList.remove('parsley-success');
            });

            Array.from(document.getElementsByClassName('parsley-error')).forEach(function (el) {
                el.classList.remove('parsley-error');
            });

            if ($('.parsley-required').length>0)
                $('.parsley-required').text('');
        });


    })

    $scope.OnLoad = function () {
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/OnLoadData", "").then(function (response) {
            if (response.data[0] == "OK") {

                if (response.data[3].length > 0) 
                    $scope.TimesheetDataPending = response.data[3];

                if (response.data[4].length > 0)
                    $scope.TimesheetDataApproved = response.data[4];

                $scope.TimesheetData.length = $scope.TimesheetDataPending.length + $scope.TimesheetDataApproved.length;

                var count = 1
                count = count + Number($scope.TimesheetData.length);
                $scope.TimeId = "TIM-" + count;

                $('#Pending').DataTable().clear().destroy();
                $('#Approved').DataTable().clear().destroy();

                setTimeout(function () {
                    $('#Pending').DataTable({
                        lengthChange: true,
                        responsive: true,
                        bDestroy: true,
                        language: {
                            searchPlaceholder: 'Search...',
                            sSearch: '',
                            lengthMenu: '_MENU_ ',
                        }
                    });

                    $('#Approved').DataTable({
                        lengthChange: true,
                        responsive: true,
                        bDestroy: true,
                        language: {
                            searchPlaceholder: 'Search...',
                            sSearch: '',
                            lengthMenu: '_MENU_ ',
                        }
                    });
                });

                if (response.data[1].length > 0) {
                    $scope.AllTaskArr = response.data[1];
                    $scope.ClientDetails = [...new Map($scope.AllTaskArr.map(item => [item['Client'], item])).values()];
                    if ($scope.ClientDetails.length == 1)
                        $timeout(function () {
                            $("#ddlClient").val("number:" + $scope.ClientDetails[0].Client).trigger('change');
                        })
                }

                if (response.data[2].length > 0)
                    WorkingHour = response.data[2][0].Hour;               
            }
        });
    }

    $scope.ProjectDetails = function () {
        $scope.ProjectInfo = [];
        var ProjectData = $scope.AllTaskArr.filter(function (e) {
                return e.Client == $scope.ngddlClient;
        })
        $scope.ProjectInfo = [...new Map(ProjectData.map(item => [item['Project'], item])).values()];

        $timeout(function () {
            if ($scope.ProjectInfo.length == 1) {
                $('#ddlProjectName').val("number:" + $scope.ProjectInfo[0].Project).trigger('change');
            }
        });

    }

    $scope.MilestoneDetails = function () {
        $scope.MilestoneInfo = [];
        var MileData = $scope.AllTaskArr.filter(function (e) {
            return e.Project == $scope.ngddlProjectName;
        })
        $scope.MilestoneInfo = [...new Map(MileData.map(item => [item['MileStone'], item])).values()];

        $timeout(function () {
            if ($scope.MilestoneInfo.length == 1) {
                $('#ddlMilestone').val("number:" + $scope.MilestoneInfo[0].MileStone).trigger('change');
            }
        });
    }

    $scope.TaskDetails = function () {
        $scope.TaskInfo = [];
        $scope.TaskInfo = $scope.AllTaskArr.filter(function (e, i) {
            if (e.MileStone == $scope.ngddlMilestone) {
                if (e.hasOwnProperty("SubTask") && e.SubTaskStatusName != "Approved") {
                    $scope.AllTaskArr[i].AllTask = e.SubTask;
                    return e.MileStone == $scope.ngddlMilestone;

                }
                else if (e.hasOwnProperty("Task") && e.TaskStatusName != "Approved") {
                    $scope.AllTaskArr[i].AllTask = e.Task;
                    return e.MileStone == $scope.ngddlMilestone;

                }


            }
        })

        $timeout(function () {
            if ($scope.TaskInfo.length == 1) {
                $('#ddlTask').val("number:" + $scope.TaskInfo[0].ID).trigger('change');
            }
        });
    }

    $scope.GetPrevTimesheet = function () {
        if ($scope.ngddlTask != null || $scope.ngddlTask != undefined) {

            ExistUtilize = 0;
            if ($scope.TimesheetArr.length > 0) {
                $scope.ChkUtilizeHour = $scope.TimesheetArr.filter(function (e) {
                    return e.AllTaskID == $scope.ngddlTask;
                })

                if ($scope.ChkUtilizeHour.length > 0 && $scope.ChkUtilizeHour[0].Edit != true) {
                    ExistUtilize = $scope.ChkUtilizeHour[0].UtilizedHours;
                }

                if ($scope.ChkUtilizeHour.length > 0 && $scope.ChkUtilizeHour[0].Edit == true)
                    return false;
            }

            

            $scope.ClearPopData();
            $scope.ngtxtHours = undefined;
                $scope.CurrentTask = $scope.AllTaskArr.filter(function (response) {
                    return response.ID == $scope.ngddlTask;
                })
                var data = {
                    'AllTaskId': $scope.ngddlTask
            }
            if (ExistUtilize != 0) {
                $scope.ngtxtUtilizedHours = ExistUtilize;
                $scope.ngtxtEstimatedHours = $scope.CurrentTask[0].NoOfDays * WorkingHour;
                $scope.ngtxtEstimatedHours = $scope.ngtxtEstimatedHours + ":00";
                $scope.GetRemainingHorus();
            }
            else {
                CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetPrevTimesheet", data).then(function (response) {
                    if (response.data[0] == "OK") {
                        if (response.data[1].length > 0) {
                            ExistUtilize = response.data[1][0].UtilizedHours;
                            $scope.ngtxtUtilizedHours = ExistUtilize;
                        }
                        else
                            $scope.ngtxtUtilizedHours = 0;

                        $scope.ngtxtEstimatedHours = $scope.CurrentTask[0].NoOfDays * WorkingHour;
                        $scope.ngtxtEstimatedHours = $scope.ngtxtEstimatedHours +":00";
                        $scope.GetRemainingHorus();
                    }

                });
            }
                
        }
        
    }

   // $("#txtHours").on("keyup", function () {
    $scope.GetRemainingHorus = function () {

        if ($scope.ngtxtHours != undefined && $scope.ngtxtEstimatedHours != undefined && $scope.ngtxtUtilizedHours != undefined) {
            var TotalUtilize = $scope.timeConvert($scope.ngtxtUtilizedHours, $scope.ngtxtHours, "Add");
            if ($scope.ngtxtEstimatedHours.split(':')[0] >= (Number(TotalUtilize.split(':')[0]) + Number(TotalUtilize.split(':')[1])/10)) {
                var RemainingHours = $scope.timeConvert($scope.ngtxtEstimatedHours, TotalUtilize, "Minus");
                $("#txtRemainingHours").val(RemainingHours);
                $scope.ngtxtRemainingHours = RemainingHours;

                $("#txtUtilizedHours").val(TotalUtilize);
                $scope.ngtxtUtilizedHours = TotalUtilize;
                return false;
            }
            else {
                $("#txtHours").val('');
                $("#txtToTime").val('');

                alert("Hours should not be more than estimated hours");
                return false;
            }
                
        }
    }
    //});

    $scope.timeConvert = function (start, end, action) {
        //var h = mins / 60 | 0,
        //    m = mins % 60 | 0;
        //return moment.utc().hours(h).minutes(m).format("hh:mm");

        var diff = "";
        var s = [];
        var e = [];

        if (/[:]/.test(start))
            s = start.split(':');
        else
            s[0] = start, s[1] = 0;


        if (/[:]/.test(end))
            e = end.split(':');
        else
            e[0] = end, e[1] = 0;
        
        

        if (action == "Minus") {

            min = s[1] - e[1];

            hour_carry = 0;
            if (min < 0) {
                min += 60;
                if (min.toString().length == 1)
                    min = "0" + min.toString();
                hour_carry += 1;
            }

            hour = s[0] - e[0] - hour_carry;
            hour = Math.abs(hour);
            diff = hour + ":" + min;
        }
        else if (action == "Add") {
            min = Number(e[1]) + Number(s[1]);
            hour_carry = 0;

            if (min > 59) {
                hour_carry = min / 60 | 0;
                min = min % 60 | 0;
            }

            hour = Number(e[0]) + Number(s[0]) + Number(hour_carry);

            if (min.toString().length == 1)
                min = "0" + min.toString();

            diff = hour + ":" + min;
        }

        return diff;
    }

    $scope.AddTimesheet = function () {
        var obj = {};
        obj.ID = $("#divAddTimesheet").attr("data-id");
        //obj.$$hashKey = $scope.CurrentTask.$$hashKey;
        obj.AllTaskID = $scope.CurrentTask[0].ID;

        obj.Description = $scope.ngtxtDescription;
        obj.Hours = $scope.ngtxtHours;
        obj.EstimatedHours = $scope.ngtxtEstimatedHours;
        obj.UtilizedHours = $("#txtUtilizedHours").val();
        obj.RemainingHours = $scope.ngtxtRemainingHours;
        obj.MileStone = $scope.CurrentTask[0].MileStone;
        obj.MileStoneName = $scope.CurrentTask[0].MileStoneName;
        obj.Project = $scope.CurrentTask[0].Project;
        obj.ProjectName = $scope.CurrentTask[0].ProjectName;
        obj.AllTask = $scope.CurrentTask[0].AllTask;
        obj.Client = $scope.CurrentTask[0].Client;
        obj.ClientName = $scope.CurrentTask[0].ClientName;
        obj.TimesheetID = $scope.TimeId;
        obj.TimesheetAddedDate = moment($scope.ngtxtTimesheetDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        obj.FromTime = moment($scope.ngtxtTimesheetDate + $("#txtFromTime").val(), 'DD-MM-YYYY hh:mm').format("MM-DD-YYYY hh:mm A");
        obj.ToTime = moment($scope.ngtxtTimesheetDate + $("#txtToTime").val(), 'DD-MM-YYYY hh:mm').format("MM-DD-YYYY hh:mm A");
        obj.FromTimeView = $("#txtFromTime").val();
        obj.ToTimeView = $("#txtToTime").val();
        obj.AllTaskStatus = $scope.ngddlAllTaskStatus;


        if ($scope.CurrentTask[0].hasOwnProperty("SubTask"))
            obj.SubTask = $scope.CurrentTask[0].ID;
        else
            obj.Task = $scope.CurrentTask[0].ID;


        $scope.TimesheetArr.push(obj);

        //clear all the fields
        $scope.ClearPopData();

        $timeout(function () {
            $("#ddlClient").val('').trigger('change');
            $("#ddlAllTaskStatus").val('').trigger('change');
        }, 10);

        $("#txtFromTime").val('');
        $("#txtToTime").val('');
        $scope.ngtxtHours = undefined;

        Array.from(document.getElementsByClassName('parsley-success')).forEach(function (el) {
            el.classList.remove('parsley-success');
        });
    }

    $scope.EditTimesheet = function (i) {
        $scope.TimesheetArr[i].Edit = true;
        $scope.CurrentTask[0] = $scope.TimesheetArr[i];
        $scope.ngtxtDescription = $scope.TimesheetArr[i].Description;
        $scope.ngtxtHours = $scope.TimesheetArr[i].Hours;
        $scope.ngtxtEstimatedHours = $scope.TimesheetArr[i].EstimatedHours;
        $("#txtUtilizedHours").val($scope.TimesheetArr[i].UtilizedHours);
        $scope.ngtxtRemainingHours = $scope.TimesheetArr[i].RemainingHours;
        $scope.ngtxtTimesheetDate = moment($scope.TimesheetArr[i].TimesheetAddedDate).format("DD-MM-YYYY");
        $("#txtFromTime").val($scope.TimesheetArr[i].FromTimeView);
        $("#txtToTime").val($scope.TimesheetArr[i].ToTimeView);

        $timeout(function () {
            $("#ddlClient").val("number:" + $scope.TimesheetArr[i].Client).trigger('change');
            $("#ddlAllTaskStatus").val( $scope.TimesheetArr[i].AllTaskStatus).trigger('change');
            $timeout(function () {
                $("#ddlProjectName").val("number:" +$scope.TimesheetArr[i].Project).trigger('change');
                $timeout(function () {
                    $("#ddlMilestone").val("number:" +$scope.TimesheetArr[i].MileStone).trigger('change');
                    $timeout(function () {
                        $("#ddlTask").val("number:" + $scope.TimesheetArr[i].AllTaskID).trigger('change');
                        $scope.TimesheetArr.splice(i, 1);
                    }, 10);
                }, 10);
            }, 10);
        }, 10);

        $("#divAddTimesheet").attr("data-id", $scope.TimesheetArr[i].ID); 
    }

    $scope.DeleteTimesheet = function (i) {
        $scope.TimesheetArr.splice(i, 1);
    }

    $scope.OpenAddTimesheet = function () {
        $scope.ngtxtTimesheetDate = moment().format("DD/MM/YYYY");
        $("#MilestoneTitle").text("Add Timesheet");
        $("#AddTimesheetPopUp").modal('show');
    }

    $scope.FinalAddTimesheet = function () {
        if ($scope.TimesheetArr.length > 0) {
            $scope.TimesheetLoad = true;

            if ($scope.EditTimesheetArr.length > 0) {
                $scope.ClearPrev();
            }
            

            var data = {
                'EmpTimesheet': $scope.TimesheetArr,
            }

            //var AddSubTask = new Array();
            //AddSubTask = $scope.TimesheetArr;

            CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/AddTimesheet", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $scope.OnLoad();
                    $scope.TimesheetLoad = false;
                    $("#AddTimesheetPopUp").modal("hide");
                }
            });
        }
        else {

        }
    }

    $scope.ClearPrev = function () {
        var data = {
            'ClearTimesheet': $scope.EditTimesheetArr,
        }

        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/ClearTimesheet", data).then(function (response) {
            if (response.data[0] == "OK") {

            }
            else {
                return false;
            }
        });
    }

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $scope.EditViewMainTimesheet = function (Timesheet,Action) {
        var data = {
            'TimesheetId': Timesheet.TimesheetID
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetEditTimesheet", data).then(function (response) {
            if (response.data[1].length > 0) {
                $scope.EditTimesheetArr = response.data[1];
                $scope.TimeId = $scope.EditTimesheetArr[0].TimesheetID;
                $scope.ngtxtTimesheetDate = $scope.EditTimesheetArr[0].TimesheetAddedDate.split(' ')[0];
                angular.forEach($scope.EditTimesheetArr, function (value, key) {

                    var obj = {};
                    obj.ID = value.ID;
                    obj.Description = value.Description;
                    obj.Hours = value.Hours;
                    obj.EstimatedHours = value.EstimatedHours;
                    obj.UtilizedHours = value.UtilizedHours;
                    obj.RemainingHours = value.RemainingHours;
                    obj.MileStone = value.MileStone;
                    obj.MileStoneName = value.MileStoneName;
                    obj.Project = value.Project;
                    obj.ProjectName = value.ProjectName;
                    obj.Client = value.Client;
                    obj.ClientName = value.ClientName;
                    obj.TimesheetID = value.TimesheetID;
                    obj.TimesheetAddedDate = value.TimesheetAddedDate;
                    obj.FromTime = value.FromTime;
                    obj.ToTime = value.ToTime;
                    obj.FromTimeView = moment(value.FromTime, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
                    obj.ToTimeView = moment(value.ToTime, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");

                    if (value.SubTask != 0) {
                        obj.SubTask = value.SubTask;
                        obj.AllTask = value.SubTaskName;
                        obj.AllTaskID = value.SubTask;

                    }
                    else {
                        obj.Task = value.Task;
                        obj.AllTask = value.TaskName;
                        obj.AllTaskID = value.Task;

                    }

                    obj.EmployeeName = value.EmployeeName;
                    obj.ManagerName = value.ManagerName;

                    $scope.TimesheetArr.push(obj);
                })
                if (Action == "View") {
                    $scope.ViewTimesheet = true;
                    $("#MilestoneTitle").text("View Timesheet");
                }
                else {
                    $("#MilestoneTitle").text("Edit Timesheet");
                }

                $("#btnAddTimesheet").text("Update");
                $timeout(function () {
                    $("#ddlClient").val('').trigger('change');
                }, 10);
                $("#AddTimesheetPopUp").modal('show');

            }
        });
    }

    $scope.ClearPopData = function () {
        $("#txtUtilizedHours").val('');
        $scope.ngtxtDescription = undefined;
        $scope.ngtxtEstimatedHours = undefined;
        $scope.ngtxtRemainingHours = undefined;
        $scope.ngtxtUtilizedHours = undefined;
    }


});
