var ManagerDashboardApp = angular.module('ManagerDashboardApp', ['CommonAppUtility'])
ManagerDashboardApp.controller('ManagerDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {

    $scope.CurrentTimesheet = [];

    $scope.TimesheetLoad = function () {
        CommonAppUtilityService.CreateItem("/TIM_ManagerDashboard/LoadTimesheetData", "").then(function (response) {
            if (response.data[0] == "OK") {

                if (response.data[1].length > 0)
                    $scope.TimesheetDataPending = response.data[1];

                if (response.data[2].length > 0)
                    $scope.TimesheetDataApproved = response.data[2];


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

            }
        });
    }

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $scope.ApproveTimesheet = function (Timesheet) {
        var data = {
            'TimesheetId': Timesheet.TimesheetID
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetEditTimesheet", data).then(function (response) {
            if (response.data[1].length > 0) {
                $scope.CurrentTimesheet = response.data[1];
                $("#ViewTimesheetPopUp").modal('show');
            }
        });
    }

    $scope.TimeFormat = function (t) {
        if (t != undefined || t != null) {
            return moment(t, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
        }
    }

    $scope.FinalApproveTimesheet = function (Action) {
        if (Action == "Approve") 
            $scope.ApproveLoad = true;
        else
            $scope.RejectLoad = true;

        var data = {
            'TimesheetData': $scope.CurrentTimesheet,
            'Action': Action,
            'Descrition': $("#txtRemark").val()
        }
        CommonAppUtilityService.CreateItem("/TIM_ManagerDashboard/ApproveTimesheet", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.TimesheetLoad();
                $scope.RejectLoad = false;
                $scope.ApproveLoad = false;
                $("#ViewTimesheetPopUp").modal("hide");
            }
        });

    }
});