
var TimesheetDashboardApp = angular.module('TimesheetDashboardApp', ['CommonAppUtility'])

TimesheetDashboardApp.controller('TimesheetDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {
    $scope.TimesheetData = [];
    $scope.AllTaskArr = [];
    var WorkingHour;
    $scope.ClientDetails = [];
    $scope.TimesheetArr = [];
    $scope.EditTimesheet = [];
    $scope.ngtxtTimesheetDate = moment().format("DD/MM/YYYY");
    var TimeId = "TIM-";

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

        $('#Approved').DataTable({
            responsive: true,
            language: {
                searchPlaceholder: 'Search...',
                sSearch: '',
                lengthMenu: '_MENU_ ',
            }
        });

        //$('#txtTimesheetDate').datetimepicker({
        //    minView: 2,
        //    format: 'dd-mm-yyyy',
        //    autoclose: true
        //});

        $(".AllSelect").select2({
            placeholder: 'Choose one',
            searchInputPlaceholder: 'Search',
            width: '100%',
            dropdownParent: $("#AddTimesheetPopUp")
        })
    })

    $scope.OnLoad = function () {
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/OnLoadData", "").then(function (response) {
            if (response.data[0] == "OK") {
                if (response.data[1].length > 0) {
                    $scope.AllTaskArr = response.data[1];
                    $scope.ClientDetails = [...new Map($scope.AllTaskArr.map(item => [item['Client'], item])).values()];
                }

                if (response.data[2].length > 0)
                    WorkingHour = response.data[2][0].Hour;

                if (response.data[3].length > 0)
                    $scope.TimesheetData = response.data[3];
                $('#Pending').DataTable().clear().destroy();
                    setTimeout(function () {
                        table = $('#Pending').DataTable({
                            lengthChange: true,
                            responsive: true,
                            bDestroy: true,
                            language: {
                                searchPlaceholder: 'Search...',
                                sSearch: '',
                                lengthMenu: '_MENU_ ',
                            }
                        });
                        table.buttons().container()
                            .appendTo('#tblProject_wrapper .col-md-6:eq(0)');
                    });
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
                if (e.hasOwnProperty("SubTask"))
                    $scope.AllTaskArr[i].AllTask = e.SubTask;
                else
                    $scope.AllTaskArr[i].AllTask = e.Task;
            }
            return e.MileStone == $scope.ngddlMilestone;
        })

        $timeout(function () {
            if ($scope.TaskInfo.length == 1) {
                $('#ddlTask').val("number:" + $scope.TaskInfo[0].ID).trigger('change');
            }
        });
    }

    $scope.GetPrevTimesheet = function () {
        if ($scope.ngddlTask != null || $scope.ngddlTask != undefined) {
            var data = {
                'AllTaskId': $scope.ngddlTask.ID
            }
            CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetPrevTimesheet", data).then(function (response) {
                if (response.data[0] == "OK") {
                    console.log(response);
                    if (response.data[1].count > 0) {

                    }
                    else
                        $scope.ngtxtUtilizedHours = 0;

                    $scope.ngtxtEstimatedHours = $scope.ngddlTask.NoOfDays * WorkingHour;
                }
            });
        }
        
    }

    $("#txtHours").on("keyup", function () {

        if ($scope.ngtxtHours != "" || $scope.ngtxtHours != undefined || $scope.ngtxtHours != null) {

            var RemainingHours = Number($scope.ngtxtEstimatedHours) - (Number($scope.ngtxtUtilizedHours) + Number($scope.ngtxtHours));
            if (RemainingHours > 0) {
                $("#txtRemainingHours").val(RemainingHours);
                $scope.ngtxtRemainingHours = RemainingHours;

                var UtilizedHours = Number($scope.ngtxtUtilizedHours) + Number($scope.ngtxtHours);
                $("#txtUtilizedHours").val(UtilizedHours);
            }
            else {
                $("#txtHours").val($("#txtHours").val().slice(0, -1));
                alert("Hours should not be more than estimated hours");
            }

        }

    });

    $scope.AddTimesheet = function () {
        var obj = {};
        obj.$$hashKey = $scope.ngddlTask.$$hashKey;
        obj.Description = $scope.ngtxtDescription;
        obj.Hours = $scope.ngtxtHours;
        obj.EstimatedHours = $scope.ngtxtEstimatedHours;
        obj.UtilizedHours = $("#txtUtilizedHours").val();
        obj.RemainingHours = $scope.ngtxtRemainingHours;
        obj.MileStone = $scope.ngddlTask.MileStone;
        obj.MileStoneName = $scope.ngddlTask.MileStoneName;
        obj.Project = $scope.ngddlTask.Project;
        obj.ProjectName = $scope.ngddlTask.ProjectName;
        obj.AllTask = $scope.ngddlTask.AllTask;
        obj.Client = $scope.ngddlTask.Client;
        obj.ClientName = $scope.ngddlTask.ClientName;
        obj.TimesheetAddedDate = moment($scope.ngtxtTimesheetDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");

        if ($scope.ngddlTask.hasOwnProperty("SubTask"))
            obj.SubTask = $scope.ngddlTask.ID;
        else
            obj.Task = $scope.ngddlTask.ID;

        $scope.TimesheetArr.push(obj);

        //clear all the fields
        $("#txtUtilizedHours").val('');
        $scope.ngtxtDescription = "";
        $scope.ngtxtHours = "";
        $scope.ngtxtEstimatedHours = "";
        $scope.ngtxtRemainingHours = "";

        $timeout(function () {
            $("#ddlClient").val('').trigger('change');
        }, 10);

    }

    $scope.EditTimesheet = function (i) {
        $scope.ngtxtDescription = $scope.TimesheetArr[i].Description;
        $scope.ngtxtHours = $scope.TimesheetArr[i].Hours;
        $scope.ngtxtEstimatedHours = $scope.TimesheetArr[i].EstimatedHours;
        $("#txtUtilizedHours").val($scope.TimesheetArr[i].UtilizedHours);
        $scope.ngtxtRemainingHours = $scope.TimesheetArr[i].Hours;
        $timeout(function () {
            $("#ddlClient").val("number:" + $scope.TimesheetArr[i].Client).trigger('change');
            $timeout(function () {
                $("#ddlProjectName").val("number:" +$scope.TimesheetArr[i].Project).trigger('change');
                $timeout(function () {
                    $("#ddlMilestone").val("number:" +$scope.TimesheetArr[i].MileStone).trigger('change');
                    $timeout(function () {
                        $("#ddlTask").val($scope.TimesheetArr[i].$$hashKey).trigger('change');
                        $scope.TimesheetArr.splice(i, 1);
                    }, 10);
                }, 10);
            }, 10);
        }, 10);
    }

    $scope.DeleteTimesheet = function (i) {
        $scope.TimesheetArr.splice(i, 1);
    }

    $scope.OpenAddTimesheet = function () {
        $("#AddTimesheetPopUp").modal('show');
    }

    $scope.FinalAddTimesheet = function () {
        if ($scope.TimesheetArr.length > 0) {
            $scope.TimesheetLoad = true;

            if ($scope.EditTimesheet.length > 0) {
                var data = {
                    'EmpTimesheet': $scope.TimesheetArr,
                    'Action': "Update"
                }
            }
            else {
                var data = {
                    'EmpTimesheet': $scope.TimesheetArr,
                    'Action': "Insert"
                }
            }

            //var AddSubTask = new Array();
            //AddSubTask = $scope.TimesheetArr;

            CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/AddTimesheet", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $scope.OnLoad();
                    $scope.TimesheetArrLoad = false;
                    $("#AddTimesheetPopUp").modal("hide");
                }
            });
        }
        else {

        }
    }

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $scope.EditMainTimesheet = function (Timesheet) {
        var data = {
            'TimesheetId': Timesheet.TimesheetID
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetEditTimesheet", data).then(function (response) {
            if (response.data[1].length > 0) {
                $scope.EditTimesheet = response.data[1];
                angular.forEach($scope.EditTimesheet, function (value, key) {

                    var obj = {};
                    obj.$$hashKey = value.$$hashKey;
                    obj.Description = value.Description;
                    obj.Hours = value.Hours;
                    obj.EstimatedHours = value.EstimatedHours;
                    obj.UtilizedHours = value.UtilizedHours;
                    obj.RemainingHours = UtilizedHours.RemainingHours;
                    obj.MileStone = value.MileStone;
                    obj.MileStoneName = value.MileStoneName;
                    obj.Project = value.Project;
                    obj.ProjectName = value.ProjectName;
                    obj.AllTask = value.AllTask;
                    obj.Client = value.Client;
                    obj.ClientName = value.ClientName;
                    obj.TimesheetAddedDate = moment(value.TimesheetAddedDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");

                    if (value.hasOwnProperty("SubTask"))
                        obj.SubTask = value.ID;
                    else
                        obj.Task = value.ID;

                    $scope.TimesheetArr.push(obj);
                })
                $("#AddTimesheetPopUp").modal('show');

            }
        });
    }
});
