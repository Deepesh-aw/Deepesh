
var TimesheetDashboardApp = angular.module('TimesheetDashboardApp', ['CommonAppUtility'])

TimesheetDashboardApp.controller('TimesheetDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {
    $scope.AllTaskArr = [];
    $scope.TaskArr = [];
    $scope.SubTaskArr = [];

    $scope.ClientDetails = [];

    $(function () {

        //$scope.OnLoad();

        $('#Pending').DataTable({
            responsive: true,
            language: {
                searchPlaceholder: 'Search...',
                sSearch: '',
                lengthMenu: '_MENU_ ',
            }
        });

        $('#Approved').DataTable({
            responsive: true,
            language: {
                searchPlaceholder: 'Search...',
                sSearch: '',
                lengthMenu: '_MENU_ ',
            }
        });

        $('#txtTimesheetDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

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
                if (response.data[3].length > 0) {
                    $scope.AllTaskArr = response.data[3];
                    //$scope.ClientDetails = Enumerable.from($scope.AllTaskArr).distinct("$.Client").toArray();
                    $scope.ClientDetails = [...new Map($scope.AllTaskArr.map(item => [item['Client'], item])).values()];
                }
                console.log(response);
                if (response.data[1].length > 0) {
                    $scope.TaskArr = response.data[1]; 
                }

                if (response.data[2].length > 0) {
                    $scope.SubTaskArr = response.data[2];
                }

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

    $scope.GetHours = function () {
        var data = {
            'AllTaskId' : $scope.ngddlTask
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetHour", data).then(function (response) {
            if (response.data[0] == "OK") {
                console.log("new");
                console.log(response);
            }
        });
    }

    $scope.OpenAddTimesheet = function () {
        $("#AddTimesheetPopUp").modal('show');
    }
});
