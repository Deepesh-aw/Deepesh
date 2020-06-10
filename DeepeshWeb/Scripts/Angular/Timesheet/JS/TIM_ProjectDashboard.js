var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, $compile, CommonAppUtilityService) {
    var table;
    var MilestoneTable;
    var TaskTable;
    var SubTaskTable;

    $(function () {
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
        },20);
    })

    $scope.ShowMilestone = function (index, ProjectId) {
        $("#Milestone" + index).find('i').removeClass('fa fa-plus');
        $("#Milestone" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#Milestone"+index).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            $("#Milestone" + index).find('i').removeClass('fa fa-minus');
            $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#Milestone" + index).find('i').addClass('fa fa-plus');
            row.child.hide();
        }
        else {
            var data = {
                'ProjectId': ProjectId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var Html = '<div><table class="mg-b-0 text-md-nowrap tableCss dataTable" id = "tblMilestone" ><thead><tr><th></th><th scope="col">Milestone</th><th scope="col">Description</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        //this.push(key + ': ' + value);
                        Html += '<tr><td class = "details-td" id = "Task' + key + '">';
                        if (value.InternalStatus != "MilestoneCreated") {
                            Html += '<span><button><i class="fa fa-plus" ng-click = "ShowTask(' + key + ', ' + value.ID + ', ' + ProjectId + ')"  aria-hidden="true"></i></button></span>';
                        }
                        else
                            Html += '<button class="btn btn-info" type="button" ng-click="AddTask(' + value.ID + ', ' + ProjectId + ')">Add Task</button>'

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
                        $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#Milestone" + index).find('i').addClass('fa fa-minus');

                    }, 2);

                }
                else {
                    $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Milestone" + index).find('i').addClass('fa fa-minus');
                }
                   

            });
        }
    }

    //$(document).on("click", ".ClickEvent", function () {
    //    alert($(this).attr("name"));
    //});

    $scope.ShowTask = function (index, MilestoneId, ProjectId) {
        $("#Task" + index).find('i').removeClass('fa fa-plus');
        $("#Task" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#Task" + index).closest('tr');
        var row = MilestoneTable.row(tr);

        if (row.child.isShown()) {
            $("#Task" + index).find('i').removeClass('fa fa-minus');
            $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#Task" + index).find('i').addClass('fa fa-plus');
            row.child.hide();
        }
        else {
            var data = {
                'MilestoneId': MilestoneId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                console.log(response);
                if (response.data[0] == "OK") {
                    var Html = '<div><table class="mg-b-0 text-md-nowrap tableCss dataTable" id = "tblTask" ><thead><tr><th></th><th scope="col">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        Html += '<tr><td class = "details-td" id = "SubTask' + key + '">';
                        if (value.InternalStatus != "TaskCreated")
                            Html += '<span><button><i class="fa fa-plus" ng-click = "ShowSubTask(' + key + ', ' + value.ID + ')"  aria-hidden="true"></i></button></span>';
                        else
                            Html += '<button class="btn btn-info" type="button" ng-click="AddSubTask(' + value.ID + ', ' + MilestoneId + ', ' + ProjectId + ')">Add SubTask</button>'

                        Html += '</td ><td data-label="Milestone">' + value.Task + '</td> <td data-label="Description">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td></tr > ';
                    });
                    Html += '</tbody></table></div>';
                    row.child(Html).show();

                    setTimeout(function () {
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
                        $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#Task" + index).find('i').addClass('fa fa-minus');
                    }, 2);

                }
                else {
                    $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Task" + index).find('i').addClass('fa fa-minus');
                }
            });
        }
    }

    $scope.ShowSubTask = function (index, TaskId) {
        $("#SubTask" + index).find('i').removeClass('fa fa-plus');
        $("#SubTask" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#SubTask" + index).closest('tr');
        var row = TaskTable.row(tr);

        if (row.child.isShown()) {
            $("#SubTask" + index).find('i').removeClass('fa fa-minus');
            $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#SubTask" + index).find('i').addClass('fa fa-plus');
            row.child.hide();
        }
        else {
            var data = {
                'TaskId': TaskId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetSubTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var Html = '<div><table class="mg-b-0 text-md-nowrap tableCss dataTable" id = "tblSubTask" ><thead><tr><th></th><th scope="col">SubTask</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        Html += '<tr><td class = "details-td" id = "SubTask' + key + '">';

                        Html += '</td ><td data-label="SubTask">' + value.SubTask + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td></tr > ';
                    });
                    Html += '</tbody></table></div>';
                    row.child(Html).show();

                    setTimeout(function () {
                        SubTaskTable = $('#tblSubTask').DataTable({
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
                        $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#SubTask" + index).find('i').addClass('fa fa-minus');
                    }, 2);

                }
                else {
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('fa fa-minus');
                }
            });
        }
    }

    $scope.AddMilestone = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.AddTask = function (MilestoneId, ProjectId) {
        $.cookie('ProjectId', ProjectId);
        $.cookie('MilestoneId', MilestoneId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.AddSubTask = function (TaskId, MilestoneId, ProjectId) {
        $.cookie('TaskId', TaskId);
        $.cookie('ProjectId', ProjectId);
        $.cookie('MilestoneId', MilestoneId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddSubTask' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }


});



