
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
    $scope.TimeId;
    $scope.CurrentTask = [];
    var ExistUtilize = 0;
    $scope.OtherClient = false;
    $scope.ValidateField = true;
    $scope.UploadFiles = [];
    $scope.TimesheetFiles = [];
    $scope.PrevTimesheetFiles = [];
    $scope.TempPrevTimesheetFiles = [];
    var DeleteTimesheet = [];
    var DeleteTimesheetDoc = [];
    $scope.MinTimesheetEdit;

    $(function () {
        $(".overlay").hide();

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
            $(this).data('val', $(this).val());
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

                    if ($("#divAddTimesheet").attr("data-id") != "0") {

                        var FromPrev = $("#divAddTimesheet").attr("data-from");
                        var ToPrev = $("#divAddTimesheet").attr("data-to");

                        var PrevFrm = moment($scope.ngtxtTimesheetDate + " " + FromPrev, 'DD-MM-YYYY hh:mm').format("hh:mm a");
                        var Prevto = moment($scope.ngtxtTimesheetDate + " " + ToPrev, 'DD-MM-YYYY hh:mm').format("hh:mm a");
                        var PrevtFrom = moment(PrevFrm, 'h:mma');
                        var prevtTo = moment(Prevto, 'h:mma');
                        var prevdiff = moment.duration(prevtTo.diff(PrevtFrom));


                        var TotalUtilize;
                        var RemainingHours;

                        var difference = prevdiff.subtract(diff);
                        var strDiff = Math.abs(difference._data.hours) + ":" + Math.abs(difference._data.minutes);

                        if (difference._data.hours < 0 || difference._data.minutes < 0) {
                            TotalUtilize = $scope.timeConvert($scope.ngtxtUtilizedHours, strDiff, "Add");
                        }
                        else {
                            TotalUtilize = $scope.timeConvert($scope.ngtxtUtilizedHours, strDiff, "Minus");
                        }

                        RemainingHours = $scope.timeConvert($scope.ngtxtEstimatedHours, TotalUtilize, "Minus");
                        $timeout(function () {
                            $scope.ngtxtRemainingHours = RemainingHours;
                            $scope.ngtxtUtilizedHours = TotalUtilize;
                            return false;
                        })

                    }
                    else {
                        $scope.GetRemainingHorus();
                    }

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
            $("#txtTimesheetDate").attr("data-PatentID", "0");
            $(".AllSelect").attr("disabled", false);
            DeleteTimesheet.length = 0;
            DeleteTimesheetDoc.length = 0;
            $scope.PrevTimesheetFiles.length = 0;
            $scope.TimesheetFiles.length = 0;
            $scope.UploadFiles.length = 0;
            $scope.TempPrevTimesheetFiles.length = 0;
            $scope.ViewTimesheet = false;
            //$scope.TimeId = "TIM-" + $scope.TimesheetData.length + 1;
            $("#btnAddTimesheet").text("Submit");
            $scope.TimesheetArr.length = 0;
            $scope.EditTimesheetArr.length = 0;
            $scope.ClearPopData();

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

            if ($('.parsley-required').length > 0)
                $('.parsley-required').text('');


        });


    })

    $scope.TimesheetDashboardLoad = function () {
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/TimesheetLoadData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $('#Pending').DataTable().clear().destroy();
                $('#Approved').DataTable().clear().destroy();
                $('#Rejected').DataTable().clear().destroy();

                $scope.TimesheetData = response.data[1];

                if (response.data[2].length > 0) {
                    $scope.AllTaskArr = response.data[2];
                    $scope.ClientDetails = [...new Map($scope.AllTaskArr.map(item => [item['Client'], item])).values()];
                    $scope.ClientDetails.push({ "Client": 0, "ClientName": "Other" });
                }

                if (response.data[3].length > 0) {
                    $scope.TimesheetSettingData = response.data[3][0];
                    $scope.TimeId = "TIM-" + $scope.TimesheetSettingData.TimesheetCount;
                    WorkingHour = Number($scope.TimesheetSettingData.WorkingHour);
                    $scope.MinTimesheetEdit = Number($scope.TimesheetSettingData.MinTimesheetEditDays);
                }

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

                    $('#Rejected').DataTable({
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

            }
        });
    }

    $scope.GetClientData = function () {
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/OnLoadData", "").then(function (response) {
            if (response.data[0] == "OK") {

                //if (response.data[3].length > 0)
                //    $scope.TimesheetData.length = [...new Map(response.data[3].map(item => [item['TimesheetID'], item])).values()].length;
                //else
                //    $scope.TimesheetData.length = 0;

                //var count = 1
                //count = Number(count) + Number($scope.TimesheetData.length);
                //$scope.TimeId = "TIM-" + count;



                //if (response.data[2].length > 0)
                //    WorkingHour = response.data[2][0].Hour;



                $scope.ngtxtTimesheetDate = moment().format("DD/MM/YYYY");
                $("#MilestoneTitle").text("Add Timesheet");
                $scope.LoadAddTimesheet = false;
                $("#AddTimesheetPopUp").modal('show');
            }
            else {
                alert("Something went wrong. Please try after some time.");
                $scope.LoadAddTimesheet = false;
            }

        });
    }

    $scope.ProjectDetails = function () {
        $scope.ClearPopData();
        $(".otherValidate").val('');
        $scope.ProjectInfo = [];
        var OtherClientEntry = $("#ddlClient option:selected").text();
        if (OtherClientEntry == "Other") {
            $scope.OtherClient = true;
            $scope.ngtxtEstimatedHours = "0:00";
            if ($scope.ValidateField == true)
                $scope.ngtxtUtilizedHours = 0;

            $(".otherValidate").prop("required", true);
            $(".NormalValidate").prop("required", false);
            $('.NormalValidate').next().text('');
        }
        else {
            $scope.OtherClient = false;
            $(".otherValidate").prop("required", false);
            $(".NormalValidate").prop("required", true);
            $('.otherValidate').next().text('');

            var ProjectData = $scope.AllTaskArr.filter(function (e) {
                return e.Client == $scope.ngddlClient;
            })
            $scope.ProjectInfo = [...new Map(ProjectData.map(item => [item['Project'], item])).values()];

            $timeout(function () {
                if ($scope.ProjectInfo.length == 1) {
                    $('#ddlProjectName').val("number:" + $scope.ProjectInfo[0].Project).trigger('change');
                }
                $(".AllSelect").select2({
                    placeholder: 'Choose one',
                    searchInputPlaceholder: 'Search',
                    width: '100%',
                    dropdownParent: $("#AddTimesheetPopUp")
                })
            });
        }
    }

    $scope.MilestoneDetails = function () {
        $scope.ClearPopData();
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
        $scope.ClearPopData();
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
                        ExistUtilize = $scope.ChkUtilizeHour[$scope.ChkUtilizeHour.length-1].UtilizedHours;
                }

                if ($scope.ChkUtilizeHour.length > 0 && $scope.ChkUtilizeHour[0].Edit == true) {
                    ExistUtilize = $scope.ChkUtilizeHour[0].UtilizedHours;
                    return false;
                }

            }

            $scope.ClearPopData();
            //$scope.ngtxtHours = undefined;
            $scope.CurrentTask = $scope.AllTaskArr.filter(function (response) {
                return response.ID == $scope.ngddlTask;
            })

            if (ExistUtilize != 0 ) {
                $scope.ngtxtUtilizedHours = ExistUtilize;
                $scope.ngtxtEstimatedHours = $scope.CurrentTask[0].NoOfDays * WorkingHour;
                $scope.ngtxtEstimatedHours = $scope.ngtxtEstimatedHours + ":00";
                $scope.GetRemainingHorus();
            }
            else {
                var data = {
                    'AllTaskId': $scope.ngddlTask,
                    'TimesheetAddedDate': moment($scope.ngtxtTimesheetDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
                    'ParentID': $("#txtTimesheetDate").attr("data-PatentID"),
                }
                CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetPrevTimesheet", data).then(function (response) {
                    if (response.data[0] == "OK") {
                        if (response.data[1].length > 0) {
                            ExistUtilize = $scope.timeConvert(response.data[1][0].UtilizedHours, ExistUtilize, "Add");
                            //ExistUtilize = response.data[1][0].UtilizedHours;
                            $scope.ngtxtUtilizedHours = ExistUtilize;
                        }
                        else
                            $scope.ngtxtUtilizedHours = ExistUtilize;

                        $scope.ngtxtEstimatedHours = $scope.CurrentTask[0].NoOfDays * WorkingHour;
                        $scope.ngtxtEstimatedHours = $scope.ngtxtEstimatedHours + ":00";
                        $scope.GetRemainingHorus();
                    }

                });
            }

        }

    }

    // $("#txtHours").on("keyup", function () {
    $scope.GetRemainingHorus = function () {
        if ($scope.OtherClient == false) {
            if ($scope.ngtxtHours != undefined && $scope.ngtxtEstimatedHours != undefined && $scope.ngtxtUtilizedHours != undefined) {
                var TotalUtilize = $scope.timeConvert($scope.ngtxtUtilizedHours, $scope.ngtxtHours, "Add");
                if ($scope.ngtxtEstimatedHours.split(':')[0] >= (Number(TotalUtilize.split(':')[0]) + Number(TotalUtilize.split(':')[1]) / 10)) {
                    var RemainingHours = $scope.timeConvert($scope.ngtxtEstimatedHours, TotalUtilize, "Minus");
                    $timeout(function () {
                        $scope.ngtxtRemainingHours = RemainingHours;
                        $scope.ngtxtUtilizedHours = TotalUtilize;
                        return false;
                    })
                }
                else {
                    var RemainingHours = $scope.timeConvert($scope.ngtxtEstimatedHours, TotalUtilize, "Exceed");
                    $timeout(function () {
                        $scope.ngtxtRemainingHours = RemainingHours;
                        $scope.ngtxtUtilizedHours = TotalUtilize;
                        return false;
                    })
                    //$("#txtHours").val('');
                    //$("#txtToTime").val('');
                    //alert("Hours should not be more than estimated hours");
                    //return false;
                }

            }
        }
        else {
            $timeout(function () {
                $scope.ngtxtRemainingHours = "0:00";
                $scope.ngtxtUtilizedHours = $scope.ngtxtHours;
            })
        }
    }
    //});

    $scope.GetTotalHours = function () {
        var Hours = 0, Minute = 0;
        angular.forEach($scope.TimesheetArr, function (value, key) {
            Hours = Number(Hours) + Number(value.Hours.split(':')[0]);
            Minute = Number(Minute) + Number(value.Hours.split(':')[1]);
            hour_carry = 0;

            if (Minute > 59) {
                hour_carry = Minute / 60 | 0;
                Minute = Minute % 60 | 0;
            }

            Hours = Hours + Number(hour_carry);

            if (Minute.toString().length == 1)
                Minute = "0" + Minute.toString();
        });
        return Hours + ":" + Minute;
    }

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



        if (action == "Exceed") {
            let valuestart = moment.duration("20:00", "HH:mm");
            let valuestop = moment.duration("23:15", "HH:mm");
            let difference = valuestop.subtract(valuestart);
            //alert(difference);

            min = s[1] - e[1];
            if (min.toString().length == 1)
                min = "0" + min.toString();

            hour = s[0] - e[0];
            diff = "-" + Math.abs(hour) + ":" + Math.abs(min);

        }
        else if (action == "Minus") {

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

    $("#uploadFiles").change(function () {
        var file = $("#uploadFiles")[0].files[0];
        var reader = new FileReader();
        var fileByteArray = [];
        reader.onload = function (event) {
            var temp = {};
            temp.Name = file.name;
            temp.file = file;
            temp.Flag = "New";
            temp.Byte = reader.result;
            //array = new Uint8Array(temp.Byte);
            //for (var i = 0; i < array.length; i++) {
            //    fileByteArray.push(array[i]);
            //}
            //temp.ByteArr.fileByteArray;
            //temp.ByteArr = new Uint8Array(temp.Byte);
            $scope.UploadFiles.push(temp);
            $scope.$apply();
        }
        reader.readAsArrayBuffer(file);
    });

    $scope.RemoveTimesheetFiles = function (index) {
        $scope.UploadFiles.splice(index, 1);
        $scope.TimesheetFiles.splice(index, 1);
    }

    $scope.HoursAlterDifferent = function (before, after) {
        let valuestart = moment.duration(before, "HH:mm");
        let valuestop = moment.duration(after, "HH:mm");
        let difference = valuestop.subtract(valuestart);
        //alert(difference._data.hours + ":" + difference._data.minutes);
        return difference._data.hours + ":" + difference._data.minutes;

    }

    var count = 0;
    $scope.AddTimesheet = function () {
        count++;
        var obj = {};

        if ($scope.OtherClient == true) {
            obj.OtherClientStatus = true;
            obj.OtherClient = $("#ddlClient option:selected").text();
            obj.ClientName = $("#ddlClient option:selected").text();

            obj.OtherProject = $scope.ngtxtProject;
            obj.ProjectName = $scope.ngtxtProject;

            obj.OtherMilestone = $scope.ngtxtMilestone;
            obj.MileStoneName = $scope.ngtxtMilestone;

            obj.OtherTask = $scope.ngtxtTask;
            obj.AllTask = $scope.ngtxtTask;

        }
        else {
            obj.OtherClientStatus = false;
            obj.Client = $scope.CurrentTask[0].Client;
            obj.ClientName = $scope.CurrentTask[0].ClientName;
            obj.Project = $scope.CurrentTask[0].Project;
            obj.ProjectName = $scope.CurrentTask[0].ProjectName;
            obj.MileStone = $scope.CurrentTask[0].MileStone;
            obj.MileStoneName = $scope.CurrentTask[0].MileStoneName;
            //obj.AllTaskID = $scope.CurrentTask[0].ID;
            obj.AllTaskID = $scope.ngddlTask;
            obj.AllTask = $scope.CurrentTask[0].AllTask;

            if ($scope.CurrentTask[0].hasOwnProperty("SubTask"))
                obj.SubTask = $scope.CurrentTask[0].ID;
            else
                obj.Task = $scope.CurrentTask[0].ID;
        }

        obj.ID = $("#divAddTimesheet").attr("data-id");
        //obj.$$hashKey = $scope.CurrentTask.$$hashKey;

        obj.Description = $scope.ngtxtDescription;
        obj.Hours = $scope.ngtxtHours;
        obj.EstimatedHours = $scope.ngtxtEstimatedHours;
        obj.UtilizedHours = $("#txtUtilizedHours").val();
        obj.RemainingHours = $scope.ngtxtRemainingHours;
        obj.TimesheetID = $scope.TimeId;
        obj.TimesheetAddedDate = moment($scope.ngtxtTimesheetDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        obj.FromTime = moment($scope.ngtxtTimesheetDate + $("#txtFromTime").val(), 'DD-MM-YYYY hh:mm').format("MM-DD-YYYY hh:mm A");
        obj.ToTime = moment($scope.ngtxtTimesheetDate + $("#txtToTime").val(), 'DD-MM-YYYY hh:mm').format("MM-DD-YYYY hh:mm A");
        obj.FromTimeView = $("#txtFromTime").val();
        obj.ToTimeView = $("#txtToTime").val();
        obj.AllTaskStatus = $scope.ngddlAllTaskStatus;
        obj.AllTaskStatusText = $("#ddlAllTaskStatus option:selected").text();
        obj.Flag = "New";
        obj.MinAlterDay = $scope.MinTimesheetEdit;
        obj.OldUtilizedHours = $("#divAddTimesheet").attr("data-oldutilize");
        
        if ($scope.UploadFiles.length > 0) {
            var Doc = [];
            angular.forEach($scope.UploadFiles, function (value, key) {
                var CommonObj = {};
                CommonObj.Name = count+"_"+value.Name;
                CommonObj.File = value.file;
                obj.FileName = value.Name;
                $scope.TimesheetFiles.push(CommonObj);

                
                Doc.push({ 'FileName': value.Name });
                obj.Files = Doc;
            });
        }

        if ($scope.TempPrevTimesheetFiles.length > 0) {
            var PrevDoc = [];
            angular.forEach($scope.TempPrevTimesheetFiles, function (value, key) {
                var temp = {};
                temp.ID = value.ID;
                temp.Name = value.Name;
                temp.LID = value.LID;
                temp.FileName = value.FileName;
                temp.Delete = value.Delete;
                PrevDoc.push(temp);
                obj.PrevFiles = PrevDoc;

            });
        }
               
        obj.FileCount = count;

        if ($("#divAddTimesheet").attr("data-oldutilize") != obj.UtilizedHours) {
            obj.AlterUtilizeHour = $scope.HoursAlterDifferent($("#divAddTimesheet").attr("data-oldutilize"), obj.UtilizedHours);
        }        


        $scope.TimesheetArr.push(obj);
        $scope.UploadFiles.length = 0;
        $scope.TempPrevTimesheetFiles.length = 0;




        //clear all the fields
        $scope.ClearPopData();

        $timeout(function () {
            $("#ddlClient").val('').trigger('change');
            $("#ddlAllTaskStatus").val('').trigger('change');
        }, 10);

       

        Array.from(document.getElementsByClassName('parsley-success')).forEach(function (el) {
            el.classList.remove('parsley-success');
        });

        
        $("#divAddTimesheet").attr("data-id", "0");
        $("#divAddTimesheet").attr("data-from", "0");
        $("#divAddTimesheet").attr("data-to", "0");
        $("#divAddTimesheet").attr("data-UtilizedHours", "0");
        $("#divAddTimesheet").attr("data-oldutilize", "0"); 

        $(".AllSelect").attr("disabled", false);

    }

    $scope.EditTimesheet = function (i) {
        $scope.ValidateField = false;
        $scope.TimesheetArr[i].Edit = true;
        $scope.CurrentTask[0] = $scope.TimesheetArr[i];
        $scope.ngtxtDescription = $scope.TimesheetArr[i].Description;
        $scope.ngtxtHours = $scope.TimesheetArr[i].Hours;
        $scope.ngtxtEstimatedHours = $scope.TimesheetArr[i].EstimatedHours;
        $scope.ngtxtUtilizedHours = $scope.TimesheetArr[i].UtilizedHours;
        //$("#txtUtilizedHours").val($scope.TimesheetArr[i].UtilizedHours);
        $scope.ngtxtRemainingHours = $scope.TimesheetArr[i].RemainingHours;
        $scope.ngtxtTimesheetDate = moment($scope.TimesheetArr[i].TimesheetAddedDate).format("DD-MM-YYYY");
        $("#txtFromTime").val($scope.TimesheetArr[i].FromTimeView);
        $("#txtToTime").val($scope.TimesheetArr[i].ToTimeView);

        if ($scope.TimesheetFiles.length > 0) {
            angular.forEach($scope.TimesheetFiles, function (value, key) {
                var FileName = value.Name.split('_')[0];
                if ($scope.TimesheetArr[i].FileCount == FileName) {
                    var CommonObj = {};
                    CommonObj.Name = value.Name.substring(FileName.length + 1);
                    CommonObj.file = value.File;
                    $scope.UploadFiles.push(CommonObj);
                }

            });
        }

        if ($scope.TimesheetArr[i].hasOwnProperty("PrevFiles")) {
            if ($scope.TimesheetArr[i].PrevFiles.length > 0) {
                $scope.TempPrevTimesheetFiles = $scope.TimesheetArr[i].PrevFiles;
            }
        }
        
        
        if ($scope.TimesheetArr[i].OtherClientStatus == true) {
            $scope.OtherClient = true;

            $timeout(function () {
                $scope.ngtxtProject = $scope.TimesheetArr[i].OtherProject;
                $scope.ngtxtMilestone = $scope.TimesheetArr[i].OtherMilestone;
                $scope.ngtxtTask = $scope.TimesheetArr[i].OtherTask;

                $("#ddlClient").val("number:" + 0).trigger('change');
                $("#ddlAllTaskStatus").val($scope.TimesheetArr[i].AllTaskStatus).trigger('change');
                $scope.TimesheetArr.splice(i, 1);
                $scope.ValidateField = true;
            }, 20);
        }
        else {
            $timeout(function () {
                $("#ddlClient").val("number:" + $scope.TimesheetArr[i].Client).trigger('change');
                $("#ddlAllTaskStatus").val($scope.TimesheetArr[i].AllTaskStatus).trigger('change');
                $timeout(function () {
                    $("#ddlProjectName").val("number:" + $scope.TimesheetArr[i].Project).trigger('change');
                    $timeout(function () {
                        $("#ddlMilestone").val("number:" + $scope.TimesheetArr[i].MileStone).trigger('change');
                        $timeout(function () {
                            $("#ddlTask").val("number:" + $scope.TimesheetArr[i].AllTaskID).trigger('change');
                            $scope.TimesheetArr.splice(i, 1);
                            $scope.ValidateField = true;
                        }, 10);
                    }, 10);
                }, 10);
            }, 10);
        }

       
        $("#divAddTimesheet").attr("data-id", $scope.TimesheetArr[i].ID); 
        $("#divAddTimesheet").attr("data-utilize", $scope.TimesheetArr[i].UtilizedHours);
        $("#divAddTimesheet").attr("data-from", $scope.TimesheetArr[i].FromTimeView); 
        $("#divAddTimesheet").attr("data-to", $scope.TimesheetArr[i].ToTimeView); 
        $("#divAddTimesheet").attr("data-oldutilize", $scope.TimesheetArr[i].OldUtilizedHours); 

        if ($("#divAddTimesheet").attr("data-id") > 0)
            $(".AllSelect").attr("disabled", true);
    }

    $scope.DeleteTimesheet = function (i) {
        if ($scope.TimesheetArr[i].Flag == "Old") {
            var obj = {};
            obj.ID = $scope.TimesheetArr[i].ID;
            obj.Hours = $scope.TimesheetArr[i].Hours;
            obj.Client = $scope.TimesheetArr[i].Client;
            obj.Project = $scope.TimesheetArr[i].Project;
            obj.MileStone = $scope.TimesheetArr[i].MileStone;
            obj.TimesheetAddedDate = $scope.TimesheetArr[i].TimesheetAddedDate;
            if ($scope.TimesheetArr[i].hasOwnProperty("SubTask")) {
                obj.SubTask = $scope.TimesheetArr[i].SubTask;
            }
            else {
                obj.Task = $scope.TimesheetArr[i].Task;
            }


            DeleteTimesheet.push(obj);
            if ($scope.TimesheetArr[i].PrevFiles.length > 0) {
                angular.forEach($scope.TimesheetArr[i].PrevFiles, function (value, key) {
                    var obj = {};
                    obj.ID = value.ID;
                    DeleteTimesheetDoc.push(obj);
                });
            }
        }
        $scope.TimesheetArr.splice(i, 1);
    }

    $scope.RemovePrevDoc = function (index) {
        $scope.TempPrevTimesheetFiles[index].Delete = "Yes";
        var obj = {};
        obj.ID = $scope.TempPrevTimesheetFiles[index].ID;
        DeleteTimesheetDoc.push(obj);
    }

    $scope.OpenAddTimesheet = function () {
        $scope.LoadAddTimesheet = true;
        $scope.GetClientData();
    }
    
    $scope.FinalAddTimesheet = function () {
        if ($scope.TimesheetArr.length > 0) {
            $scope.TimesheetLoad = true;
            $(".overlay").show();

            //var data = {
            //    'EmpTimesheet': $scope.TimesheetArr,
            //}

            var fileData = new FormData();
            for (var i = 0; i < $scope.TimesheetFiles.length; i++) {
                fileData.append($scope.TimesheetFiles[i].name, $scope.TimesheetFiles[i].File, $scope.TimesheetFiles[i].Name);
            }

            fileData.append('TimesheetDetails', JSON.stringify($scope.TimesheetArr));

            fileData.append('DeleteDocument', JSON.stringify(DeleteTimesheetDoc));

            fileData.append('DeleteTimesheet', JSON.stringify(DeleteTimesheet));

            //CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/AddTimesheet", data).then(function (response) {
            CommonAppUtilityService.DataWithFile("/TIM_TimesheetDashboard/AddTimesheet", fileData).then(function (response) {
                if (response[0] == "OK") {
                    $scope.TimesheetDashboardLoad();
                    $scope.TimesheetLoad = false;
                    $("#AddTimesheetPopUp").modal("hide");
                    $(".overlay").hide();
                }
                else {
                    alert("Something went wrong. Please try after some time.");
                    $scope.TimesheetLoad = false;
                    $(".overlay").hide();
                    $("#AddTimesheetPopUp").modal("hide");
                }
            });
        }
        else {
            alert("Please add at least one record");
        }
    }

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $scope.EditViewMainTimesheet = function (Timesheet, Action, Type) {

        var data = {
            'TimesheetId': Timesheet.TimesheetID
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetEditTimesheet", data).then(function (response) {
            if (response.data[1].length > 0) {
                $scope.EditTimesheetArr = response.data[1];
                $scope.TimeId = $scope.EditTimesheetArr[0].TimesheetID;
                $scope.ngtxtTimesheetDate = $scope.EditTimesheetArr[0].TimesheetAddedDate.split(' ')[0];
                $("#txtTimesheetDate").attr("data-PatentID", $scope.EditTimesheetArr[0].ParentID); 
                angular.forEach($scope.EditTimesheetArr, function (value, key) {

                    var obj = {};
                    if (value.OtherClient == "") {
                        obj.OtherClientStatus = false;
                        obj.MileStone = value.MileStone;
                        obj.MileStoneName = value.MileStoneName;
                        obj.Project = value.Project;
                        obj.ProjectName = value.ProjectName;
                        obj.Client = value.Client;
                        obj.ClientName = value.ClientName;

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
                    }
                    else {
                        obj.OtherClientStatus = true;
                        obj.OtherClient = value.OtherClient;
                        obj.ClientName = value.OtherClient;

                        obj.OtherProject = value.OtherProject;
                        obj.ProjectName = value.OtherProject;

                        obj.OtherMilestone = value.OtherMilestone;
                        obj.MileStoneName = value.OtherMilestone;

                        obj.OtherTask = value.OtherTask;
                        obj.AllTask = value.OtherTask;
                    }
                    obj.ID = value.ID;
                    obj.AllTaskStatus = value.AllTaskStatus;
                    obj.AllTaskStatusText = value.AllTaskStatusName;
                    obj.Description = value.Description;
                    obj.Hours = value.Hours;
                    obj.EstimatedHours = value.EstimatedHours;
                    obj.UtilizedHours = value.UtilizedHours;
                    obj.RemainingHours = value.RemainingHours;
                    obj.TimesheetID = value.TimesheetID;
                    obj.TimesheetAddedDate = moment(value.TimesheetAddedDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");

                    obj.FromTime = moment(value.FromTime, 'DD-MM-YYYY hh:mm:ss').format("MM-DD-YYYY hh:mm A");
                    obj.ToTime = moment(value.ToTime, 'DD-MM-YYYY hh:mm:ss').format("MM-DD-YYYY hh:mm A");
                    obj.FromTimeView = moment(value.FromTime, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
                    obj.ToTimeView = moment(value.ToTime, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");

                    obj.ModifyDate = value.ModifyDate;
                    obj.ModifyName = value.ModifyName;
                    obj.StatusName = value.StatusName;
                    obj.EmployeeName = value.EmployeeName;
                    obj.ManagerName = value.ManagerName;
                    obj.ApprovedDate = value.ApproveDate;
                    obj.ApproveDescription = value.ApproveDescription;
                    obj.RejectDescription = value.RejectDescription;
                    obj.Flag = "Old";
                    obj.OldUtilizedHours = value.UtilizedHours;
                    //Document code
                    var Doc = [];
                    obj.PrevFiles = Doc;
                    angular.forEach(response.data[2], function (value, key) {
                        if (value.LID == obj.ID) {
                            var temp = {};
                            temp.ID = value.ID;
                            temp.Name = value.Name;
                            temp.LID = value.LID;
                            temp.FileName = value.DocumentPath;
                            temp.Delete = "No";
                            Doc.push(temp);
                            obj.PrevFiles = Doc;
                        }

                    });

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
            else {
               
            }
        });
    }

    $scope.ClearPopData = function () {
        if ($scope.ValidateField == true) {
            $scope.ngtxtDescription = undefined;
            $scope.ngtxtEstimatedHours = undefined;
            $scope.ngtxtRemainingHours = undefined;
            $scope.ngtxtUtilizedHours = undefined;
            $scope.ngtxtHours = undefined;
            $scope.ngtxtRemainingHours = undefined;

            $("#txtFromTime").val('');
            $("#txtToTime").val('');
            //$("#txtUtilizedHours").val('');
            $("#txtHours").val('');
            //$("#txtRemainingHours").val('');

            $scope.ngtxtProject = undefined;
            $scope.ngtxtMilestone = undefined;
            $scope.ngtxtTask = undefined;
        }

    }

});
