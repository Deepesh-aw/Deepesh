var AddDashboardLanding = angular.module('AddDashboardLanding', ['CommonAppUtility'])

AddDashboardLanding.controller('AddDashboardLanding', function ($scope, $http, $timeout, $compile, CommonAppUtilityService) {
    var table;
    var AllDataTableId = {};

    $(function () {

        LoadMilestoneData();

    });

    function LoadMilestoneData() {
        CommonAppUtilityService.CreateItem("/TIM_DashboardLanding/GetMilestoneData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $scope.MilestoneData = response.data[1];
                setTimeout(function () {
                    table = $('#tblMilestone').DataTable({
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
                }, 20);
            }
        });
    }


    $scope.BackToDashboard = function () {
        $.removeCookie('ProjectId');
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_ProjectDashboard' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.DateFormat = function (d) {
        return d.split(' ')[0];
    }

    $scope.ShowTask = function (index, MilestoneId, ProjectId) {
        $("#Task" + MilestoneId).find('i').removeClass('si si si-plus');
        $("#Task" + MilestoneId).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#Task" + MilestoneId).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            $("#Task" + MilestoneId).find('i').removeClass('si-minus si');
            $("#Task" + MilestoneId).find('i').removeClass('spinner-border spinner-border-sm');
            $("#Task" + MilestoneId).find('i').addClass('si si si-plus');
            row.child.hide();
        }
        else {
            var data = {
                'MilestoneId': MilestoneId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var TaskID = response.data[1][0].ID;
                    var Html = '<div><table class="table key-buttons text-md-nowrap " id="tblTask' + TaskID + '" style="width:82%" ><thead><tr><th scope="col" style="padding-left: 35px;">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        var i = key + 1;
                        Html += '<tr><td data-label="Task"><span class = "details-td" id = "SubTask' + value.ID + '">';
                        if (value.InternalStatus != "TaskCreated")
                            Html += '<span><i class="si si si-plus" ng-click = "ShowSubTask(' + value.ID + ', ' + value.ID + ')"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
                        else
                            Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';

                        Html += '</span >' + value.Task + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td> ';

                        if (value.InternalStatus == "TaskCreated")
                            Html += '<button class="badge badge-primary" type="button" ng-click="AddSubTask(' + value.ID + ', ' + MilestoneId + ', ' + ProjectId + ')">Add SubTask</button>';

                    });
                    Html += '</td></tr></tbody></table></div>';
                    row.child(Html).show();

                    setTimeout(function () {
                        AllDataTableId["TaskTable" + TaskID] = $('#tblTask' + TaskID + '').DataTable({
                            "paging": false,
                            "ordering": false,
                            "info": false,
                            "lengthChange": true,
                            "responsive": true,
                            bDestroy: true,
                            "bFilter": false,
                            "createdRow": function (row, data, index) {
                                $compile(row)($scope); //add this to compile the DOM
                            },
                            language: {
                                searchPlaceholder: 'Search...',
                                sSearch: '',
                                lengthMenu: '_MENU_ ',
                            }
                        });
                        AllDataTableId["TaskTable" + TaskID].buttons().container()
                            .appendTo('#tblTask_wrapper .col-md-6:eq(0)');
                        $("#Task" + MilestoneId).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#Task" + MilestoneId).find('i').addClass('si-minus si');
                    }, 2);

                }
                else {
                    $("#Task" + MilestoneId).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Task" + MilestoneId).find('i').addClass('si-minus si');
                }
            });
        }
    }

    $scope.ShowSubTask = function (index, TaskId) {
        $("#SubTask" + index).find('i').removeClass('si si si-plus');
        $("#SubTask" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#SubTask" + index).closest('tr');

        var table = tr.closest('table').attr('id');
        var row = AllDataTableId["TaskTable" + table.split('tblTask')[1]].row(tr);

        // var row = TaskTable.row(tr);

        if (row.child.isShown()) {
            $("#SubTask" + index).find('i').removeClass('si-minus si');
            $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#SubTask" + index).find('i').addClass('si si si-plus');
            row.child.hide();
        }
        else {
            var data = {
                'TaskId': TaskId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetSubTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var SubTaskID = response.data[1][0].ID;
                    var Html = '<div><table class="mg-b-0 text-md-nowrap" style="width:82%" id = "tblSubTask' + SubTaskID + '" ><thead><tr><th scope="col">SubTask</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        var i = key + 1;
                        //Html += '<tr><td  >' + i + '';
                        Html += '<tr><td data-label="SubTask" id = "SubTask' + key + '">' + value.SubTask + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td></tr > ';
                    });
                    Html += '</tbody></table></div>';
                    row.child(Html).show();

                    setTimeout(function () {
                        var SubTaskTable = $('#tblSubTask' + SubTaskID).DataTable({
                            "paging": false,
                            "ordering": false,
                            "info": false,
                            "lengthChange": true,
                            responsive: true,
                            bDestroy: true,
                            "bFilter": false,
                            "createdRow": function (row, data, index) {
                                $compile(row)($scope); //add this to compile the DOM
                            },
                            language: {
                                searchPlaceholder: 'Search...',
                                sSearch: '',
                                lengthMenu: '_MENU_ ',
                            }
                        });
                        SubTaskTable.buttons().container()
                            .appendTo('#tblTask_wrapper .col-md-6:eq(0)');
                        $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#SubTask" + index).find('i').addClass('si-minus si');
                    }, 2);

                }
                else {
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('si-minus si');
                }
            });
        }
    }

});



