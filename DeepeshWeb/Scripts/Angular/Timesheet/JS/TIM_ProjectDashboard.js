var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, $compile, $timeout, CommonAppUtilityService) {
    var table;
    $scope.ProjectData = [];
    var ProjectID;
    var AllDataTableId = {};
    $(function () {

        LoadProjectData();
        // AmazeUI Datetimepicker
        $('#txtProjectStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtProjectEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('.date').datetimepicker().on('changeDate', function (e) {
            if ($('#txtProjectStartDate').val() != "" && $('#txtProjectEndDate').val() != "") {
                var start = moment($("#txtProjectStartDate").val(), 'DD/MM/YYYY');
                var end = moment($("#txtProjectEndDate").val(), 'DD/MM/YYYY');
                var days = end.diff(start, 'days');
                if (days <= 0) {
                    alert("End date should not be less than or equal to the start date");
                    $("#txtProjectEndDate").val('');
                    $("#txtNoOfDays").val('');
                    $scope.ngtxtNoOfDays = '';
                    return false;
                }
                $scope.ngtxtNoOfDays = days;
                $("#txtNoOfDays").val(days);
            }
        });

        $('#AddProjectPopUp').on('hide.bs.modal', function () {
            //$("#frmProjectDashboard")[0].reset();
            $("#ddlClientName").val(null).trigger('change.select2');
            $("#ddlProjectType").val(null).trigger('change.select2');
            $("#ddlMembers").val(null).trigger('change.select2');
            $("#ddlProjectManager").val(null).trigger('change.select2');
            Array.from(document.getElementsByClassName('parsley-success')).forEach(function (el) {
                el.classList.remove('parsley-success');
            });
            //$scope.$apply();
        });

        $('#frmProjectDashboard').parsley().on('field:validated', function () {
            var ok = $('.parsley-error').length === 0;
            $('.bs-callout-info').toggleClass('hidden', !ok);
            $('.bs-callout-warning').toggleClass('hidden', ok);
        })
            .on('form:submit', function () {
                AddProject();
                return false;
            });

        //$('.main-toggle').on('click', function () {
        //    $(this).toggleClass('on');
        //})
    })

    $scope.OpenAddProjectPop = function () {
        $("#frmProjectDashboard")[0].reset();
        $scope.ProjectStatus = false;
        $("#myModalLabel").html('Add Project');
        $("#btnProjectCreation").text("Submit");
        $("#AddProjectPopUp").modal("show");

    }

    function LoadProjectData() {
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $scope.ProjectData = response.data[1];
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
                }, 20);
                //for (var i = 0; i < $scope.ProjectData.length; i++) {
                //    var Today = moment();
                //    var EndDate = moment($scope.ProjectData[i].EndDate.split(' ')[0]);
                //    $scope.timeDiff = Number(EndDate.diff(Today, 'days')) + 1;
                //    if ($scope.timeDiff < 0)
                //        $scope.ProjectData[i].LeftDays = "Date Exceeded";
                //    else
                //        $scope.ProjectData[i].LeftDays = $scope.timeDiff + " days left";
                //}

                //console.log($scope.ProjectData);
            }
        });
    }

    $scope.ShowMilestone = function (index, ProjectId) {
        $("#Milestone" + index).find('i').removeClass('si si si-plus');
        $("#Milestone" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#Milestone" + index).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            $("#Milestone" + index).find('i').removeClass('si-minus si');
            $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#Milestone" + index).find('i').addClass('si si si-plus');
            row.child.hide();
        }
        else {
            var data = {
                'ProjectId': ProjectId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var MileID = response.data[1][0].ID;
                    var Html = '<div><table class="mg-b-0 text-md-nowrap dataTable" style="width: 82%;" id="tblMilestone'+MileID+'" ><thead><tr><th scope="col" style="padding-left: 35px;">Milestone</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th></tr></thead> <tbody>';
                    angular.forEach(response.data[1], function (value, key) {
                        //this.push(key + ': ' + value);
                        var i = key + 1;
                        Html += '<tr><td data-label="Milestone"><span class = "details-td" id = "Task' + value.ID + '">';
                        if (value.InternalStatus != "MilestoneCreated") {
                            Html += '<span><i class="si si si-plus" ng-click = "ShowTask(' + value.ID + ', ' + value.ID + ', ' + ProjectId + ')"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
                        }
                        else {
                            Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';
                        }

                        Html += '</span >' + value.MileStone + '</td><td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td>';

                        if (value.InternalStatus == "MilestoneCreated")
                            Html += '<button class="badge badge-primary" type="button" ng-click="AddTask(' + value.ID + ', ' + ProjectId + ')">Add Task</button>'

                    });
                    Html += '</td></tr ></tbody></table></span>';
                    row.child(Html).show();


                    setTimeout(function () {
                        AllDataTableId["MilestoneTable" + MileID] = $('#tblMilestone' + MileID + '').DataTable({
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
                        AllDataTableId["MilestoneTable" + MileID].buttons().container()
                            .appendTo('#tblTask_wrapper .col-md-6:eq(0)');
                        $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#Milestone" + index).find('i').addClass('si-minus si');

                    }, 2);

                }
                else {
                    $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Milestone" + index).find('i').addClass('si si si-plus');
                }


            });
        }
    }

    $scope.ShowTask = function (index, MilestoneId, ProjectId) {
        $("#Task" + index).find('i').removeClass('si si si-plus');
        $("#Task" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#Task" + index).closest('tr');
        var table = tr.closest('table').attr('id');
        //var row = MilestoneTable.row(tr);
        var row = AllDataTableId["MilestoneTable" + table.split('tblMilestone')[1]].row(tr);

        if (row.child.isShown()) {
            $("#Task" + index).find('i').removeClass('si-minus si');
            $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
            $("#Task" + index).find('i').addClass('si si si-plus');
            row.child.hide();
        }
        else {
            var data = {
                'MilestoneId': MilestoneId
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var TaskID = response.data[1][0].ID;
                    var Html = '<div><table class="table key-buttons text-md-nowrap " id="tblTask' + TaskID +'" style="width:82%" ><thead><tr><th scope="col" style="padding-left: 35px;">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th></tr></thead> <tbody>';
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
                        AllDataTableId["TaskTable" + TaskID] = $('#tblTask' + TaskID +'').DataTable({
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
                        $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                        $("#Task" + index).find('i').addClass('si-minus si');
                    }, 2);

                }
                else {
                    $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Task" + index).find('i').addClass('si-minus si');
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
                    var Html = '<div><table class="mg-b-0 text-md-nowrap" style="width:82%" id = "tblSubTask' + SubTaskID +'" ><thead><tr><th scope="col">SubTask</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th></tr></thead> <tbody>';
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

    $scope.AddMilestone = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.AddTask = function (MilestoneId, ProjectId) {
        $.cookie('ProjectId', ProjectId);
        //$.cookie('MilestoneId', MilestoneId);
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

    //$scope.AddProject = function () {
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_ProjectCreation' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}

    $scope.EditProject = function (ProjectId) {
        $("#myModalLabel").html('Update Project');
        $scope.ProjectStatus = true;
        $.cookie('ProjectId', ProjectId);
        ProjectID = ProjectId;
        ////var spsite = getUrlVars()["SPHostUrl"];
        ////Url = '/TIM_ProjectCreation' + "?SPHostUrl=" + spsite;
        ////window.location.href = Url;
        CommonAppUtilityService.CreateItem("/TIM_ProjectCreation/GetEditProject", "").then(function (response) {
            if (response.data[0] == "OK") {

                $scope.ProjectDetails = response.data[1];
                $scope.ngtxtProjectName = $scope.ProjectDetails[0].ProjectName;
                $scope.ngtxtClientProjectManager = $scope.ProjectDetails[0].ClientProjectManager;
                $scope.ngtxtProjectStartDate = $scope.ProjectDetails[0].StartDate.split(' ')[0];
                $scope.ngtxtProjectEndDate = $scope.ProjectDetails[0].EndDate.split(' ')[0];
                $scope.ngtxtNoOfDays = $scope.ProjectDetails[0].NoOfDays;
                $scope.ngtxtDescription = $scope.ProjectDetails[0].Description;
                $timeout(function () {
                    var MemberVal = [];
                    angular.forEach($scope.ProjectDetails[0].Members, function (value, key) {
                        MemberVal.push(value);
                    });
                    $("#ddlClientName").val($scope.ProjectDetails[0].ClientName).trigger('change');
                    $("#ddlProjectType").val($scope.ProjectDetails[0].ProjectType).trigger('change');
                    $("#ddlProjectManager").val($scope.ProjectDetails[0].ProjectManager).trigger('change');
                    $("#ddlMembers").val(MemberVal).trigger('change');
                });
                $("#AddProjectPopUp").modal("show");

            }
            else {

            }
        });

        $("#btnProjectCreation").text("Update");
    }

    //function for Project Deletion Alert Message
    $scope.ProjectDeletionAlert = function (ProjectId) {
        swal({
            title: "Project Deletion",
            text: "Are you sure do you really want to delete this project?",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            $scope.DeleteProject(ProjectId);
        });
    }

    $scope.DeleteProject = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/DeleteProject", "").then(function (response) {
            if (response.data[0] == "OK") {
                swal("Project deleted successfully.");
                LoadProjectData();
            }
        });
    }

    function AddProject() {
        $scope.ProjectCreationLoad = true;
        var MembersText = $("#ddlMembers option:selected").map(function () { return this.text }).get().join(', ');
        var MembersCode = $("#ddlMembers option:selected").map(function () { return this.id }).get().join(', ');
        //var data = $("#frmProjectCreation").serialize();
        //data += "&Members=" + $("#ddlMembers").val() + "&MembersText=" + MembersText;
        //alert(data);
        var data = {
            'ProjectName': $scope.ngtxtProjectName,
            'StartDate': moment($("#txtProjectStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
            'EndDate': moment($("#txtProjectEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
            'ClientName': $scope.ngddlClientName,
            'ClientProjectManager': $scope.ngtxtClientProjectManager,
            'Description': $scope.ngtxtDescription,
            'Members': $("#ddlMembers").val(),
            'MembersCodeText': MembersCode,
            'MembersText': MembersText,
            'NoOfDays': $scope.ngtxtNoOfDays,
            'ProjectManager': $scope.ngddlProjectManager,
            'ProjectType': $scope.ngddlProjectType,
        }
        if ($scope.ProjectDetails.length > 0)
            data.ID = ProjectID;

        CommonAppUtilityService.CreateItem("/TIM_ProjectCreation/SaveProject", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.ProjectCreationLoad = false;
                $('#AddProjectPopUp').modal('hide');
                LoadProjectData();
            }
            else
                $scope.ProjectCreationLoad = false;
        });
    }

    $scope.GoToMilestone = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.GoToTask = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.GoToSubTask = function (ProjectId) {
        $.cookie('ProjectId', ProjectId);
        var spsite = getUrlVars()["SPHostUrl"];
        Url = '/TIM_AddSubTask' + "?SPHostUrl=" + spsite;
        window.location.href = Url;
    }

    $scope.DateFormat = function (d) {
        return d.split(' ')[0];
    }

    $scope.UpdateDate = function (d) {
        var dt = new Date(d);
        var dtm = dt.getMonth();
        var dty = dt.getFullYear();
        return dtm + "/" + dty
    }

});



