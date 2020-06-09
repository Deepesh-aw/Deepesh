var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, $compile, CommonAppUtilityService) {
    $scope.ProjectData = [];
    var table;
    var MilestoneTable;
    var TaskTable;

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
            //$compile(angular.element(table))($scope);
        },20);

        //$(".details-control").on('click', function (event) {
        //    alert("hii");
        //    var tr = $(this).closest('tr');
        //    var row = table.row(tr);

        //    if (row.child.isShown()) {
        //        row.child.hide();
        //        return false;
        //    }
        //    else {
        //        var html = "<table class='dataTable'><tr><th>Heding</th><th>Heding</th><th>Heding</th><th>Heding</th></tr><tr><td>data</td><td>data</td><td>data</td><td>data</td></tr></table>";
        //        row.child(html).show();
        //        return false;

        //    }
        //})

    })

    $scope.ShowMilestone = function (index, ProjectData) {
        var tr = $("#Milestone"+index).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) 
            row.child.hide();
        else {
            var data = {
                'ProjectId': ProjectData.ID
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    //$scope.Milestone = response.data[1];
                        var Html = '<div><table class="mg-b-0 text-md-nowrap tableCss dataTable" id = "tblMilestone" ><thead><tr><th></th><th scope="col">Milestone</th><th scope="col">Description</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                        angular.forEach(response.data[1], function (value, key) {
                            //this.push(key + ': ' + value);
                            Html += '<tr><td id = "Task' + key + '">';
                            if (value.InternalStatus != "MilestoneCreated")
                            Html += '<span><i class="fa fa-plus" ng-click = "ShowTask(' + key + ', ' + value.ID + ')"  aria-hidden="true"></i></span>';

                            Html += '</td ><td data-label="Milestone">' + value.MileStone + '</td> <td data-label="Description">' + value.Description + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td></tr > ';
                        });
                        Html += '</tbody></table></div>';
                        row.child(Html).show();

                    setTimeout(function () {
                        //row.child($(".MileDiv").html()).show();
                          MilestoneTable = $('#tblMilestone').DataTable({
                            lengthChange: true,
                            responsive: true,
                              bDestroy: true,
                              "createdRow": function (row, data, index) {
                                  $compile(row)($scope); //add this to compile the DOM
                              },
                            language: {
                                searchPlaceholder: 'Search...',
                                sSearch: '',
                                lengthMenu: '_MENU_ ',
                            }
                          });
                    }, 2);
                    
                }
                   

            });
        }
    }

    //$(document).on("click", ".ClickEvent", function () {
    //    alert($(this).attr("name"));
    //});

    $scope.ShowTask = function (index, MilestoneId) {
        var tr = $("#Task" + index).closest('tr');
        var row = MilestoneTable.row(tr);

        if (row.child.isShown())
            row.child.hide();
        else {
            var data = {
                'MilestoneId': MilestoneId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                console.log(response);
                if (response.data[0] == "OK") {
                    //$scope.Milestone = response.data[1];
                    var Html = '<div><table class="mg-b-0 text-md-nowrap tableCss dataTable" id = "tblTask" ><thead><tr><th></th><th scope="col">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        //this.push(key + ': ' + value);
                        Html += '<tr><td id = "SubTask' + key + '">';
                        if (value.InternalStatus != "TaskCreated")
                            Html += '<span><i class="fa fa-plus" ng-click = "ShowSubTask(' + key + ', ' + value.ID + ')"  aria-hidden="true"></i></span>';

                        Html += '</td ><td data-label="Milestone">' + value.Task + '</td> <td data-label="Description">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td></tr > ';
                    });
                    Html += '</tbody></table></div>';
                    row.child(Html).show();

                    setTimeout(function () {
                        //row.child($(".MileDiv").html()).show();
                        TaskTable = $('#tblTask').DataTable({
                            lengthChange: true,
                            responsive: true,
                            bDestroy: true,
                            "createdRow": function (row, data, index) {
                                $compile(row)($scope); //add this to compile the DOM
                            },
                            language: {
                                searchPlaceholder: 'Search...',
                                sSearch: '',
                                lengthMenu: '_MENU_ ',
                            }
                        });
                    }, 2);

                }


            });
        }
    }


});



