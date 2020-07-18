var TimesheetReportApp = angular.module('TimesheetReportApp', ['CommonAppUtility'])

TimesheetReportApp.controller('TimesheetReportController', function ($scope, $http, $timeout, CommonAppUtilityService) {

    $(function () {
        $scope.Report = true;
        $scope.BindDataTable("tblTimesheet");

        $('#txtFromDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        })

        $('#txtToDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });
    });

    $scope.ShowTimesheet = function () {
        var data = {
            'From': moment($("#txtFromDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
            'To': moment($("#txtToDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
        }
        CommonAppUtilityService.CreateItem("/TIM_Timesheetreport/GetReportData", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.TimesheetData = response.data[1];
                $('#tblTimesheet').DataTable().clear().destroy();
                $scope.BindDataTable("tblTimesheet");
            }
        });
    }

    $scope.BindDataTable = function (tblId) {
        setTimeout(function () {
            $('#' + tblId).DataTable({
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

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $scope.TimeFormat = function (t) {
        if (t != undefined || t != null) {
            return moment(t, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
        }
    }

    $scope.ViewTimesheet = function (Timesheet) {
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
});