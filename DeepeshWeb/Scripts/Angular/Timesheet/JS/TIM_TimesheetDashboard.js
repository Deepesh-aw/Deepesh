var TimesheetDashboardApp = angular.module('TimesheetDashboardApp', ['CommonAppUtility'])

TimesheetDashboardApp.controller('TimesheetDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {
    $(function () {

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

    $scope.ProjectDetails = function () {
        var data = {
            'Client': $scope.ngddlClient
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetProjectDataByClient", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.ProjectData = response.data[1];
                $timeout(function () {

                    if ($scope.ProjectData.length == 1)
                        $('#ddlProjectName').val($scope.ProjectData[0].Id).trigger('change');
                    else
                        $('#ddlProjectName').val("").trigger('change');
                });
            }
        });
    }

    $scope.OpenAddTimesheet = function () {
        $("#AddTimesheetPopUp").modal('show');
    }
});
