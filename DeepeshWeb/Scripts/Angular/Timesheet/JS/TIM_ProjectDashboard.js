var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, CommonAppUtilityService) {
    $scope.ProjectData = [];
    var table;

    $(function () {
        $scope.ProjectData = $.parseJSON($("#hdnAllProjectData").val());
        $scope.$apply();
        setTimeout(function () {
            table = $('#tblProject').DataTable({
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

        $(".details-control").on('click', function (event) {
            var tr = $(this).closest('tr');
            var row = table.row(tr);

            if (row.child.isShown()) {
                row.child.hide();
                return false;
            }
            else {
                var html = "<table class='dataTable'><tr><th>Heding</th><th>Heding</th><th>Heding</th><th>Heding</th></tr><tr><td>data</td><td>data</td><td>data</td><td>data</td></tr></table>";
                row.child(html).show();
                return false;

            }
        })

    }, 2000)

   
});
