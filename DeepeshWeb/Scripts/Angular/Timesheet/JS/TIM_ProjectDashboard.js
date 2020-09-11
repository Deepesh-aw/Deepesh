var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, $compile, $timeout, $rootScope, CommonAppUtilityService) {
    var table;
    var TaskTable;
    $rootScope.ProjectPopData = [];
    $scope.ProjectDetails = [];
    $rootScope.ProjectData = [];
    $rootScope.Milestone = [];
    $rootScope.Task = [];
    $rootScope.SubTask = [];
    $rootScope.EditMilestone = [];
    $rootScope.EditTask = [];
    $rootScope.EditSubTask = [];
    $rootScope.PrevUploadFiles = [];
    $scope.UploadFiles = [];
    $scope.CurrentTimesheet = [];
    var ProjectID;
    var AllDataTableId = {};

    $(function () {

        $(".overlay").hide();

        $('input').on("input", function () {
            //alert(this.id);
            if (this.value.length > 220) {
                alert("Maximum length for the field is 220 characters.");
                $("#" + this.id).val(this.value.slice(0, -1));
                return false;
            }
        });

        $(".AddProjectSelect2").select2({
            placeholder: 'Choose one',
            searchInputPlaceholder: 'Search',
            width: '100%',
            dropdownParent: $("#AddProjectPopUp")
        })

        $(".TaskSelect2").select2({
            placeholder: 'Choose one',
            searchInputPlaceholder: 'Search',
            width: '100%',
            dropdownParent: $("#AddTaskPopUp")
        })

        $(".SubTaskSelect2").select2({
            placeholder: 'Choose one',
            searchInputPlaceholder: 'Search',
            width: '100%',
            dropdownParent: $("#AddSubTaskPopUp")
        })

        // AmazeUI Datetimepicker
        $('.main-toggle').on('click', function () {
            $(this).toggleClass('on');
            if ($(this).hasClass("main-toggle on"))
                $scope.ngProjectDelete = false;
            else
                $scope.ngProjectDelete = true;
        })


        $('.AddProjectDate').datetimepicker().on('changeDate', function (e) {
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
        }).on('hide', function (event) {
            return false;
        });

        $('#AddProjectPopUp').on('hide.bs.modal', function () {
            $scope.ngtxtProjectName = "";
            $scope.ngtxtClientProjectManager = "";
            $scope.ngtxtProjectStartDate = "";
            $scope.ngtxtProjectEndDate = "";
            $scope.ngtxtNoOfDays = "";
            $scope.ngtxtDescription = "";

            $rootScope.PrevUploadFiles.length = 0;
            $scope.UploadFiles.length = 0;


            Array.from(document.getElementsByClassName('parsley-success')).forEach(function (el) {
                el.classList.remove('parsley-success');
            });
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
        //$('#testPop1').addClass('active');
        $scope.ProjectDetails.length = 0;
        setTimeout(function () {
            $("#ddlMembers").val(' ').trigger('change');
        }, 20);
        $(".parsley-required").remove();
        $("#frmProjectDashboard")[0].reset();
        $scope.ProjectStatus = false;
        $("#myModalLabel").html('Add Project');
        $("#btnProjectCreation").text("Submit");
        $("#ddlClientName").val(null).trigger('change.select2');
        $("#ddlProjectType").val(null).trigger('change.select2');
        //$("#ddlMembers").val(' ').trigger('change');
        $("#ddlProjectManager").val(null).trigger('change.select2');
        $("#AddProjectPopUp").modal("show");

    }

    $rootScope.LoadDashboard = function () {
        $rootScope.LoadProjectData();
        $rootScope.LoadDatePicker();
    }

    $rootScope.LoadDatePicker = function () {
        $('#txtProjectStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        })

        $('#txtProjectEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

    }

    $rootScope.LoadTaskTab = function () {
        $('#tblTask').DataTable().clear().destroy();
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $rootScope.TaskData = response.data[2];
                setTimeout(function () {
                    TaskTable = $('#tblTask').DataTable({
                        lengthChange: true,
                        responsive: true,
                        bDestroy: true,
                        language: {
                            searchPlaceholder: 'Search...',
                            sSearch: '',
                            lengthMenu: '_MENU_ ',
                        }
                    });
                    TaskTable.buttons().container()
                        .appendTo('#tblProject_wrapper .col-md-6:eq(0)');
                }, 2);
            }
        });
    }

    $rootScope.LoadProjectData = function () {
        $('#tblProject').DataTable().clear().destroy();
        $('#tblTask').DataTable().clear().destroy();
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $rootScope.ProjectData = response.data[1];
                $rootScope.TaskData = response.data[2];
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

                    TaskTable = $('#tblTask').DataTable({
                        lengthChange: true,
                        responsive: true,
                        bDestroy: true,
                        language: {
                            searchPlaceholder: 'Search...',
                            sSearch: '',
                            lengthMenu: '_MENU_ ',
                        }
                    });
                    TaskTable.buttons().container()
                        .appendTo('#tblProject_wrapper .col-md-6:eq(0)');
                }, 2);
            }
            else {
                $('#tblProject').DataTable({});
            }
        });
    }

    $rootScope.ShowMilestone = function (index, ProjectData) {
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
                'ProjectId': ProjectData.ID
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $rootScope.BindMilestoneTable(response, ProjectData, row);
                    $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Milestone" + index).find('i').addClass('si-minus si');

                }
                else {
                    $("#Milestone" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Milestone" + index).find('i').addClass('si si si-plus');
                }


            });
        }
    }

    $rootScope.BindMilestoneTable = function (response, ProjectData, row) {
        var Action = 'View';
        var className = 'si si-eye text-success mr-2';
        var MileID = response.data[1][0].ID;
        $scope.Project = ProjectData;
        
        var Html = '<div><table class="mg-b-0 text-md-nowrap dataTable" style="width: 82%;" id="tblMilestone' + MileID + '" ><thead><tr><th scope="col" style="padding-left: 35px;">Milestone</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th><th scope="col">Completed Timesheet</th></tr></thead> <tbody>';
        angular.forEach(response.data[1], function (value, key) {
            //this.push(key + ': ' + value);
            $scope["Mile" + value.ID] = value;
            $rootScope.MileRow = row;
            var i = key + 1;
            Html += '<tr><td data-label="Milestone"><span class = "details-td" id = "Task' + value.ID + '">';
            if (value.InternalStatus != "MilestoneCreated") {
                Html += '<span><i class="si si si-plus" ng-click = "ShowTask(' + value.ID + ', Mile' + value.ID + ', Project)"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
            }
            else {
                Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';
            }

            Html += '</span >' + value.MileStone + '</td><td data-label="Start Date">' + value.StartDate + '</td> <td data-label="Estimated End Date">' + value.EndDate + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td>';

            if (value.InternalStatus == "MilestoneCreated")
                Html += '<i class="fa fa-plus text-primary mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Add Task"  ng-click="OpenAddTaskPop(Mile' + value.ID + ', Project, MileRow)"></i>';
            else {
                Html += '<i class="si si-pencil text-primary mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Edit Task" ng-click="OpenEditTaskPop(Mile' + value.ID + ', Project, MileRow)"></i>';
                Html += '&nbsp;<i class="si si-trash text-danger mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Delete Task" ng-click="DeleteTask(Mile' + value.ID + ', Project, MileRow)"></i>';

            }

            Html += '<td><i id="Load' + value.ID + '" class="si si-eye text-success mr-2" data-toggle="tooltip" data-placement="top" data-original-title="View" ng-click="GetCompletedTimesheet(' + value.ID + ',\'' + Action + '\' ,\'' + className + '\')"></i></td>';

        });
        Html += '</td></tr></tbody></table></span>';
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

        }, 2);
    }

    $scope.GetCompletedTimesheet = function (MilestoneId, Action, iclass) {
        $("#Load" + MilestoneId).removeClass(iclass);
        $("#Load" + MilestoneId).addClass('spinner-border spinner-border-sm');
        var data = {
            'MilestoneId': MilestoneId
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetCompletedTimesheet", data).then(function (response) {
            if (response.data[0] == "OK") {
                if (response.data[1].length > 0) {
                    $scope.CurrentTimesheet = response.data[1];
                    angular.forEach($scope.CurrentTimesheet, function (Parentvalue, Parentkey) {
                        //Document code
                        var Doc = [];
                        Parentvalue.PrevFiles = Doc;
                        angular.forEach(response.data[2], function (value, key) {
                            if (value.LID == Parentvalue.ID) {
                                var temp = {};
                                //temp.ID = value.ID;
                                temp.Name = value.Name;
                                //temp.LID = value.LID;
                                temp.FileName = value.DocumentPath;
                                temp.Delete = "No";
                                Doc.push(temp);
                                Parentvalue.PrevFiles = Doc;
                            }

                        });

                    });
                    if (Action == "View") {
                        $scope.Report = true;
                    }
                    else
                        $scope.Report = false;

                    $("#Load" + MilestoneId).removeClass('spinner-border spinner-border-sm');
                    $("#Load" + MilestoneId).addClass(iclass);

                    $("#ViewTimesheetPopUp").modal('show');
                }
                else {
                    alert("There is no completed timesheet for this milestone");
                    $("#Load" + MilestoneId).removeClass('spinner-border spinner-border-sm');
                    $("#Load" + MilestoneId).addClass(iclass);
                }
                
            }
            
        });
    }

    $scope.TimeFormat = function (t) {
        if (t != undefined || t != null) {
            return moment(t, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
        }
    }

    $rootScope.ShowTask = function (index, Milestone, Project) {
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
                'MilestoneId': Milestone.ID,
                'Members': Project.Members,
                'ProjectManager': Project.ProjectManager

            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $rootScope.BindTask(response, Milestone, Project, row);
                    $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Task" + index).find('i').addClass('si-minus si');

                }
                else {
                    $("#Task" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#Task" + index).find('i').addClass('si-minus si');
                }
            });
        }
    }

    $rootScope.BindTask = function (response, Milestone, Project, row) {
        var TaskID = response.data[1][0].ID;
       $rootScope.TaskMilestone = Milestone;
       $rootScope.TaskProjectData = Project;
       $rootScope.TaskRow = row;

        var Html = '<div><table class="table key-buttons text-md-nowrap " id="tblTask' + TaskID + '" style="width:82%" ><thead><tr><th scope="col" style="padding-left: 35px;">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Status</th><th scope="col">Action</th></tr></thead> <tbody>';
        angular.forEach(response.data[1], function (value, key) {
            var i = key + 1;
            $scope["Task" + value.ID] = value;
            Html += '<tr><td data-label="Task"><span class = "details-td" id = "SubTask' + value.ID + '">';
            if (value.InternalStatus != "TaskCreated")
                Html += '<span><i class="si si si-plus" ng-click = "ShowSubTask(' + value.ID + ', ' + value.ID + ', Project)"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
            else
                Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';

            Html += '</span >' + value.Task + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate + '</td> <td data-label="Estimated End Date">' + value.EndDate + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td data-label="Status">' + value.TaskStatusName + '</td><td> ';

            if (value.InternalStatus == "TaskCreated") {
                Html += '<i class="fa fa-plus text-primary mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Add Subtask" ng-click="OpenAddSubTaskPop(Task' + value.ID + ', TaskMilestone, TaskProjectData, TaskRow)"></i>';
            }
            else {
                Html += '<i class="si si-pencil text-primary mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Edit Subtask" ng-click="EditSubTaskPop(Task' + value.ID + ', TaskMilestone, TaskProjectData, TaskRow)"></i>';
                Html += '&nbsp;<i class="si si-trash text-danger mr-2" data-toggle="tooltip" title="" data-placement="top" data-original-title="Delete Subtask" ng-click="DeleteSubTask(Task' + value.ID + ', TaskMilestone, TaskProjectData, TaskRow)"></i>';
            }

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
        }, 2);
    }

    $scope.ShowSubTask = function (index, TaskId, Project) {
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
                'TaskId': TaskId,
                'Members': Project.Members,
                'ProjectManager': Project.ProjectManager
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetSubTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $scope.BindSubTask(response, row);
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('si-minus si');
                }
                else {
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('si-minus si');
                }
            });
        }
    }

    $scope.ShowSubTaskMain = function (index, TaskId) {
        $("#SubTask" + index).find('i').removeClass('si si si-plus');
        $("#SubTask" + index).find('i').addClass('spinner-border spinner-border-sm');

        var tr = $("#SubTask" + index).closest('tr');

        var table = tr.closest('table').attr('id');
        var row = TaskTable.row(tr);

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
                    $scope.BindSubTask(response, row);
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('si-minus si');
                }
                else {
                    $("#SubTask" + index).find('i').removeClass('spinner-border spinner-border-sm');
                    $("#SubTask" + index).find('i').addClass('si-minus si');
                }
            });
        }
    }

    $scope.BindSubTask = function (response, row ) {
        var SubTaskID = response.data[1][0].ID;
        var Html = '<div><table class="mg-b-0 text-md-nowrap" style="width:82%" id = "tblSubTask' + SubTaskID + '" ><thead><tr><th scope="col">SubTask</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Status</th></tr></thead> <tbody>';
        angular.forEach(response.data[1], function (value, key) {
            var i = key + 1;
            //Html += '<tr><td  >' + i + '';
            Html += '<tr><td data-label="SubTask" id = "SubTask' + key + '">' + value.SubTask + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate + '</td> <td data-label="Estimated End Date">' + value.EndDate + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td data-label="Status">' + value.SubTaskStatusName + '</td></tr > ';
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
        }, 2);
    }

    $scope.AddMilestonePopUp = function (Project) {
        //$.cookie('ProjectId', ProjectId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        //$("#frmProjectDashboard")[0].reset();
        var data = {
            'ProjectId': Project.ID
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectDoc", data).then(function (response) {
            if (response.data[0] == "OK") {
                if (response.data[1].length > 0) {
                    angular.forEach(response.data[1], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
            }
        });
        $rootScope.ProjectPopData = Project;
        $("#AddMilestonePopUp").modal("show");


    }

    $scope.OpenAddTaskPop = function (Milestone, Project, MileTableRow) {
        //$.cookie('ProjectId', ProjectId);
        //$.cookie('MilestoneId', MilestoneId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        var data = {
            'ProjectId': Project.ID,
            'Members': Project.Members,
            'ProjectManager': Project.ProjectManager
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectDoc", data).then(function (response) {
            if (response.data[0] == "OK") {
                if (response.data[1].length > 0) {
                    angular.forEach(response.data[1], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }

                $scope.MembersArr = response.data[2];
            }
        });
        $rootScope.MiletableRow = MileTableRow;
        $rootScope.ProjectPopData = Project;
        $rootScope.MilestonePopData = Milestone;
        $("#AddTaskPopUp").modal("show");

    }

    $scope.OpenAddSubTaskPopMain = function (TaskData) {
        var data = {
            'ProjectId': TaskData.Project,
            'MilestoneId': TaskData.MileStone,
            'TaskId': TaskData.ID
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddSubTaskMain", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.MembersSubTaskArr = response.data[4];
                $rootScope.ProjectPopData = response.data[1];
                $rootScope.MilestonePopData = response.data[2];
                $rootScope.TaskPopData = TaskData;
                if (response.data[3].length > 0) {
                    angular.forEach(response.data[3], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                $("#AddSubTaskPopUp").modal("show");
            }
        });
    }

    $scope.OpenAddSubTaskPop = function (Task, Milestone, Project, TaskTableRow) {
        //$.cookie('TaskId', TaskId);
        //$.cookie('ProjectId', ProjectId);
        //$.cookie('MilestoneId', MilestoneId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddSubTask' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        var data = {
            'ProjectId': Project.ID,
            'Members': Project.Members,
            'ProjectManager': Project.ProjectManager
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectDoc", data).then(function (response) {
            if (response.data[0] == "OK") {
                if (response.data[1].length > 0) {
                    angular.forEach(response.data[1], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                $scope.MembersSubTaskArr = response.data[2];
            }
        });
        $rootScope.TaskRow = TaskTableRow;
        $rootScope.ProjectPopData = Project;
        $rootScope.MilestonePopData = Milestone;
        $rootScope.TaskPopData = Task;
        $("#AddSubTaskPopUp").modal("show");

    }

    $scope.EditProject = function (ProjectId) {
        $(".parsley-required").remove();
        $("#myModalLabel").html('Update Project');
        $scope.ProjectStatus = true;
        $.cookie('ProjectId', ProjectId);
        ProjectID = ProjectId;
        ////var spsite = getUrlVars()["SPHostUrl"];
        ////Url = '/TIM_ProjectCreation' + "?SPHostUrl=" + spsite;
        ////window.location.href = Url;
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetEditProject", "").then(function (response) {
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

                if (response.data[2].length > 0) {
                    angular.forEach(response.data[2], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        temp.Delete = "No";
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                

                $("#AddProjectPopUp").modal("show");

            }
            else {

            }
        });

        $("#btnProjectCreation").text("Update");
    }

    $scope.DateFormat = function (d) {
        if (d != undefined || d != null) {
            return d.split(' ')[0];
        }
    }

    $("#uploadFiles").change(function () {

        var file = $("#uploadFiles")[0].files[0];
        var reader = new FileReader();

        reader.onload = function (event) {
            var temp = {};
            temp.Name = file.name;
            temp.file = file;
            temp.Flag = "New";
            $scope.UploadFiles.push(temp);
            $scope.$apply();
        }
        reader.readAsArrayBuffer(file);
    });

    $scope.Remove = function (index) {
        $scope.UploadFiles.splice(index, 1);
    }

    $scope.RemovePrev = function (index) {
        $rootScope.PrevUploadFiles[index].Delete = "Yes";
    }

    function AddProject() {
        var msg = "Project added successfully";
        $scope.ProjectCreateLoad = true;
        $(".overlay").show();

        var MembersText = $("#ddlMembers option:selected").map(function () { return this.text }).get().join(', ');
        var MembersCode = $("#ddlMembers option:selected").map(function () { return this.id }).get().join(', ');
        var MembersEmail = $("#ddlMembers option:selected").map(function () { return this.dataset.email }).get().join('; ');
        var ProjectManagerEmail = $("#ddlProjectManager option:selected").attr('data-email');
        var ProjectManager = $("#ddlProjectManager option:selected").text();
        //var data = $("#frmProjectCreation").serialize();
        //data += "&Members=" + $("#ddlMembers").val() + "&MembersText=" + MembersText;
        //alert(MembersEmail + ";" + ProjectManagerEmail);

        var data = {
            'ProjectName': $scope.ngtxtProjectName,
            'StartDate': moment($("#txtProjectStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
            'EndDate': moment($("#txtProjectEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss"),
            'ClientName': $scope.ngddlClientName,
            'ClientProjectManager': $scope.ngtxtClientProjectManager,
            'Description': $scope.ngtxtDescription,
            'Members': $("#ddlMembers").val(),
            'MembersCodeText': MembersCode,
            'MembersEmail': MembersEmail + ";" + ProjectManagerEmail,
            'MembersText': MembersText,
            'NoOfDays': $scope.ngtxtNoOfDays,
            'ProjectManager': $scope.ngddlProjectManager,
            'ProjectType': $scope.ngddlProjectType,
            'ProjectManagerName': ProjectManager,
        }
        
        

        if ($scope.ngProjectDelete == true)
            data.InternalStatus = "ProjectDeleted";

        //new code start
        var fileData = new FormData();

        if ($scope.ProjectDetails.length > 0) {
            data.ID = ProjectID;  
            msg = "Project updated successfully";
        }

        var PrevDocArr = [];
        angular.forEach($rootScope.PrevUploadFiles, function (value, key) {
            if (value.Delete == "Yes")
                PrevDocArr.push(value);
        });

        fileData.append('PrevDocument', JSON.stringify(PrevDocArr));
       
        for (var i = 0; i < $scope.UploadFiles.length; i++) {
            fileData.append($scope.UploadFiles[i].name, $scope.UploadFiles[i].file);
        }
        var objArr = [];
        objArr.push(data);
        fileData.append('ProjectDetails', JSON.stringify(objArr));

        CommonAppUtilityService.DataWithFile("/TIM_ProjectDashboard/SaveProject", fileData).then(function (response) {
            if (response[0] == "OK") {
                alert(msg);
                $scope.ProjectDetails.length = 0;
                $scope.ProjectCreateLoad = false;
                $('#AddProjectPopUp').modal('hide');
                $scope.LoadProjectData();
                $(".overlay").hide();
            }
            else {
                alert("Something went wrong. Please try after some time.");
                $scope.ProjectCreateLoad = false;
                $(".overlay").hide();
            }
                
        });
    }

    $scope.EditMilestonePopUp = function (Project) {
        $rootScope.ProjectPopData = Project;
        var data = {
            'ProjectId': Project.ID
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
            if (response.data[0] == "OK") {
                $rootScope.EditMilestone = response.data[1];
                angular.forEach($rootScope.EditMilestone, function (value, key) {
                    var obj = {};
                    obj.ID = value.ID;
                    obj.Milestone = value.MileStone;
                    obj.Description = value.Description;
                    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.NoOfDays = value.NoOfDays;
                    obj.Project = $scope.ProjectPopData.ID;
                    obj.ProjectManager = $rootScope.ProjectPopData.ProjectManager;
                    obj.MembersText = $rootScope.ProjectPopData.MembersText;
                    obj.Members = $rootScope.ProjectPopData.Members;
                    obj.InternalStatus = value.InternalStatus;
                    obj.Status = value.Status;
                    obj.Client = value.Client;
                    obj.ClientName = value.ClientName;
                    obj.Delete = "No";
                    $rootScope.Milestone.push(obj);
                });

                if (response.data[2].length > 0) {
                    angular.forEach(response.data[2], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                $("#btnMilestoneCreation").text("Update");
                $("#AddMilestonePopUp").modal("show");
                //$scope.$apply();
            }
            else {
            }
        });

    }

    $scope.DeleteMilestone = function (Project) {
        swal({
            title: "Milestone Deletion",
            text: "Are you sure do you really want to delete this project milestone?",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            var data = {
                'ProjectId': Project.ID
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/DeleteAllMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    swal("Milestone deleted successfully.");
                    $rootScope.LoadProjectData();
                }
            });
        });
    }

    $scope.OpenEditTaskPop = function (Milestone, Project, MileTableRow) {
        $rootScope.MiletableRow = MileTableRow;
        $rootScope.ProjectPopData = Project;
        $rootScope.MilestonePopData = Milestone;

        var data = {
            'MilestoneId': Milestone.ID,
            'Members': Project.Members,
            'ProjectManager': Project.ProjectManager
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.MembersArr = response.data[3];
                $rootScope.EditTask = response.data[1];
                angular.forEach($rootScope.EditTask, function (value, key) {
                    var obj = {};
                    obj.ID = value.ID;
                    obj.Task = value.Task;
                    obj.Members = value.Members;
                    obj.Membersvalue = value.Members + ":" + value.MembersEmail;
                    obj.MembersName = value.MembersName;
                    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.NoOfDays = value.NoOfDays;
                    //obj.TaskStatus = value.TaskStatus;
                    //obj.StatusName = value.TaskStatusName;
                    obj.Milestone = value.MileStone;
                    obj.Project = value.Project;
                    obj.MileStoneName = value.MileStoneName;
                    obj.ProjectName = value.ProjectName;
                    obj.InternalStatus = value.InternalStatus;
                    obj.TaskStatusName = value.TaskStatusName;
                    obj.Status = value.Status;
                    obj.Client = value.Client;
                    obj.ClientName = value.ClientName;
                    obj.Delete = "No";
                   $rootScope.Task.push(obj);
                });

                if (response.data[2].length > 0) {
                    angular.forEach(response.data[2], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                $("#btnTaskCreation").text("Update");
                $("#AddTaskPopUp").modal("show");
                //$scope.$apply();
            }
            else {
            }
        });

    }

    $scope.DeleteTask = function (Milestone, Project, MileTableRow) {
        swal({
            title: "Task Deletion",
            text: "Are you sure do you really want to delete this project task?",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            var data = {
                'ProjectId': Project.ID,
                'MilestoneId': Milestone.ID
            }
            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/DeleteAllTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    var data = {
                        'ProjectId': Project.ID
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                        if (response.data[0] == "OK") {
                            $rootScope.BindMilestoneTable(response, Project, MileTableRow);
                            swal("Task deleted successfully.");
                        }

                    });
                }
            });
        });
    }

    $scope.EditSubTaskPop = function (Task, Milestone, Project, TaskTableRow) {
        $rootScope.TaskRow = TaskTableRow;
        $rootScope.ProjectPopData = Project;
        $rootScope.MilestonePopData = Milestone;
        $rootScope.TaskPopData = Task;
        var data = {
            'TaskId': Task.ID,
            'Members': Project.Members,
            'ProjectManager': Project.ProjectManager
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetSubTask", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.MembersSubTaskArr = response.data[3];
                $rootScope.EditSubTask = response.data[1];
                angular.forEach($rootScope.EditSubTask, function (value, key) {

                    var obj = {};
                    obj.ID = value.ID;
                    obj.SubTask = value.SubTask;
                    obj.Members = value.Members;
                    obj.Membersvalue = value.Members + ":" + value.MembersEmail;
                    obj.MembersName = value.MembersName;
                    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
                    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
                    obj.NoOfDays = value.NoOfDays;
                    obj.Task = value.Task;
                    obj.Milestone = value.MileStone;
                    obj.Project = value.Project;
                    obj.MileStoneName = value.MileStoneName;
                    obj.ProjectName = value.ProjectName;
                    obj.TaskName = value.TaskName;
                    obj.InternalStatus = value.InternalStatus;
                    obj.Status = value.Status;
                    obj.SubTaskStatusName = value.SubTaskStatusName;
                    obj.Client = value.Client;
                    obj.ClientName = value.ClientName;
                    obj.Delete = "No";
                    $rootScope.SubTask.push(obj);
                });
                if (response.data[2].length > 0) {
                    angular.forEach(response.data[2], function (value, key) {
                        var temp = {};
                        temp.ID = value.ID;
                        temp.Name = value.Name;
                        temp.LID = value.LID;
                        temp.DocumentPath = value.DocumentPath;
                        $rootScope.PrevUploadFiles.push(temp);
                    });
                }
                $("#btnSubTaskCreation").text("Update");
                $("#AddSubTaskPopUp").modal("show");
                //$scope.$apply();
            }
            else {
            }
        });

    }

    $scope.EditSubTaskPopMain = function (TaskData) {
        var data = {
            'ProjectId': TaskData.Project,
            'MilestoneId': TaskData.MileStone,
            'TaskId': TaskData.ID
        }
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddSubTaskMain", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.EditSubTaskPop(TaskData, response.data[2], response.data[1], '');
            }
        });
    }

    $scope.DeleteSubTask = function (Task, Milestone, Project, TaskTableRow) {
        swal({
            title: "Subtask Deletion",
            text: "Are you sure do you really want to delete this project subtask?",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            var data = {
                'ProjectId': Project.ID,
                'MilestoneId': Milestone.ID,
                'TaskId': Task.ID
            }
                $scope.DeleteAllSubTask(data, Milestone);
        });
    }

    $scope.DeleteSubTaskMain = function (TaskData) {
        swal({
            title: "Subtask Deletion",
            text: "Are you sure do you really want to delete this project subtask?",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            var data = {
                'ProjectId': TaskData.Project,
                'MilestoneId': TaskData.MileStone,
                'TaskId': TaskData.ID
            }
                $scope.DeleteAllSubTask(data, "");

        });
    }

    $scope.DeleteAllSubTask = function (data, Milestone) {
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/DeleteAllSubTask", data).then(function (response) {
            if (response.data[0] == "OK") {
                if (Milestone != "") {
                    var data = {
                        'MilestoneId': Milestone.ID,
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                        if (response.data[0] == "OK") {
                            $rootScope.BindTask(response, Milestone, Project, TaskTableRow);
                            swal("Subtask deleted successfully.");
                        }

                    });

                }
                else {
                    swal("Subtask deleted successfully.");
                    $rootScope.LoadProjectData();
                }
            }
            
        });
    }

});

ProjectDashboardApp.controller('AddMilestoneController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $scope.ResizeDatePicker = function (popup) {
        var popupTop = popup.top - 110;
        $('.datetimepicker').css({
            'top': popupTop
        });
    }


    $scope.OnLoadMilestone = function () {

        // do something
        $('#AddMilestonePopUp').on('hide.bs.modal', function () {
            $rootScope.PrevUploadFiles.length = 0;
            $scope.ngtxtMileDays = "";
            $("#frmProjectDashboard")[0].reset();
            $rootScope.Milestone.length = 0;
            $rootScope.EditMilestone.length = 0;
            $("#btnMilestoneCreation").text("Submit");
        });

        $("#txtMileStartDate").on('click', function () {
            var popup = $(this).offset();
            $scope.ResizeDatePicker(popup);
            if ($("#divAddMile").attr("data-id") > 0)
                $('.datetimepicker').hide();
        });

        $("#txtMileEndDate").on('click', function () {
            var popup = $(this).offset();
            $scope.ResizeDatePicker(popup);
        });

        $('#txtMileStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtMileEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('.Miledate').datetimepicker().on('changeDate', function (e) {
            
            var start = moment($("#txtMileStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtMileEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            
            if ($("#divAddMile").attr("data-enddate") != "") {
                var EditEndDate = moment($("#divAddMile").attr("data-enddate"), 'DD/MM/YYYY');
                var ChkEditDate = moment(EditEndDate).format('YYYY/MM/DD');
                if (!moment(end.format('YYYY/MM/DD')).isSameOrAfter(ChkEditDate)) {
                    alert("End date can't be less");
                    $("#txtMileEndDate").val('');
                    $timeout(function () {
                        $("#txtMileEndDate").val($("#divAddMile").attr("data-enddate"));
                    })
                    return false;
                }
            }

            var ProjectSdate = moment($scope.ProjectPopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectPopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            //check start and end date is between project dates
            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Milestone date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Milestone date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check end date should not be less than start date				
            if ($('#txtMileStartDate').val() != "" && $('#txtMileEndDate').val() != "") {

                if (days <= 0) {
                    alert("End date shuold be less from start date");
                    $("#txtMileEndDate").val('');
                    $("#txtMileDays").val('');
                    $scope.ngtxtMileDays = '';
                    return false;
                }
                $scope.ngtxtMileDays = days;
                $("#txtMileDays").val(days);

                if ($("#txtMileDays").hasClass("parsley-error")) {
                    $("#txtMileDays").removeClass("parsley-error");
                    $("#txtMileDays").addClass("parsley-success");
                    $("#txtMileDays").next().remove();
                }
            }

            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        }).on('hide', function (event) {
            return false;
        });

        $('input,select,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

    }

    $scope.DeleteMilestone = function (index) {
        $rootScope.Milestone.splice(index, 1);
    }

    function clearErrorClass() {
        $('input').removeClass("parsley-success");
        $('input').removeClass("parsley-error");
        $(".parsley-errors-list").remove();
    }

    $scope.ValidateRequest = function () {
        clearErrorClass();
        var rv = true;
        if ($scope.ngtxtMilestone == "" || $scope.ngtxtMilestone == undefined || $scope.ngtxtMilestone == null) {
            $("#txtMilestone").addClass("parsley-error");
            $("#txtMilestone").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileDescription == "" || $scope.ngtxtMileDescription == undefined || $scope.ngtxtMileDescription == null) {
            $("#txtMileDescription").addClass("parsley-error");
            $("#txtMileDescription").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileStartDate == "" || $scope.ngtxtMileStartDate == undefined || $scope.ngtxtMileStartDate == null) {
            $("#txtMileStartDate").addClass("parsley-error");
            $("#txtMileStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileEndDate == "" || $scope.ngtxtMileEndDate == undefined || $scope.ngtxtMileEndDate == null) {
            $("#txtMileEndDate").addClass("parsley-error");
            $("#txtMileEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtMileDays == "" || $scope.ngtxtMileDays == undefined || $scope.ngtxtMileDays == null) {
            $("#txtMileDays").addClass("parsley-error");
            $("#txtMileDays").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }

    $scope.AddMilestone = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {
            var obj = {};
            obj.ID = $("#divAddMile").attr("data-id");
            obj.Milestone = $scope.ngtxtMilestone;
            obj.Description = $scope.ngtxtMileDescription;
            obj.StartDateView = moment($("#txtMileStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtMileEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtMileStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtMileEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtMileDays;
            obj.Project = $rootScope.ProjectPopData.ID;
            obj.ProjectManager = $rootScope.ProjectPopData.ProjectManager;
            obj.MembersText = $rootScope.ProjectPopData.MembersText;
            obj.Members = $rootScope.ProjectPopData.Members;
            obj.Client = $rootScope.ProjectPopData.ClientName;
            if ($("#divAddMile").attr("data-id") > 0) {
                obj.InternalStatus = $("#divAddMile").attr("data-internalstatus");
                obj.Status = $("#divAddMile").attr("data-statusid");
                obj.Delete = "No";

            }
            else {
                obj.InternalStatus = "";
                obj.Status = "0";
            }
            $rootScope.Milestone.push(obj);

            //clear all the fields
            $('#txtMileStartDate').attr('readonly', false);
            $("#divAddMile").attr("data-id", 0);
            $("#divAddMile").attr("data-internalstatus", "");
            $("#divAddMile").attr("data-statusid", "0");
            $("#divAddMile").attr("data-enddate", "");

            $scope.ngtxtMilestone = "";
            $scope.ngtxtMileDescription = "";
            $("#txtMileStartDate").val('');
            $scope.ngtxtMileStartDate = "";
            $("#txtMileEndDate").val('');
            $scope.ngtxtMileEndDate = "";
            $("#txtMileDays").val('');
            $scope.ngtxtMileDays = '';
        }
    }

    $scope.EditMilestone = function (index) {

        $scope.ngtxtMilestone = $rootScope.Milestone[index].Milestone;
        $scope.ngtxtMileDescription = $rootScope.Milestone[index].Description;
        $scope.ngtxtMileStartDate = $rootScope.Milestone[index].StartDateView;
        $("#txtMileStartDate").val($rootScope.Milestone[index].StartDateView);
        $scope.ngtxtMileEndDate = $rootScope.Milestone[index].EndDateView;
        $("#txtMileEndDate").val($rootScope.Milestone[index].EndDateView);
        $scope.ngtxtMileDays = $rootScope.Milestone[index].NoOfDays;

        $("#divAddMile").attr("data-internalstatus", $rootScope.Milestone[index].InternalStatus);
        $("#divAddMile").attr("data-statusid", $rootScope.Milestone[index].Status);
        $("#divAddMile").attr("data-id", $rootScope.Milestone[index].ID);
        if ($rootScope.Milestone[index].ID > 0) {
            $('#txtMileStartDate').attr('readonly', true);
            $("#divAddMile").attr("data-enddate", $rootScope.Milestone[index].EndDateView);
        }

        $rootScope.Milestone.splice(index, 1);
    }

    $scope.FinalAddMilestone = function () {
        if ($rootScope.Milestone.length > 0) {
            $scope.MilestoneCreationLoad = true;
            $(".overlay").show();
            if ($rootScope.EditMilestone.length > 0) {
                var data = {
                    'AddMilestone': $rootScope.Milestone,
                    'Action': "Update"
                }
            }
            else {
                var data = {
                    'AddMilestone': $rootScope.Milestone,
                    'Action': "Insert"
                }
            }

            //var AddMilestone = new Array();
            //AddMilestone = $rootScope.Milestone;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddMilestone", data).then(function (response) {
                if (response.data[0] == "OK") {
                    $scope.MilestoneCreationLoad = false;
                    $("#AddMilestonePopUp").modal("hide");
                    $rootScope.LoadProjectData();
                    $(".overlay").hide();
                }
                else {
                    alert("Something went wrong. Please try after some time.");
                    $(".overlay").hide();
                    $scope.MilestoneCreationLoad = false;
                }
                    
            });
        }
        else {
            $scope.ValidateRequest();
        }
    }

});

ProjectDashboardApp.controller('AddTaskController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $scope.OnLoadTask = function () {

        $scope.ResizeDatePicker = function (popup) {
            var popupTop = popup.top - 110;
            $('.datetimepicker').css({
                'top': popupTop
            });
        }

        $('#AddTaskPopUp').on('hide.bs.modal', function () {
            $rootScope.PrevUploadFiles.length = 0;
            $("#frmProjectDashboard")[0].reset();
            $('#txtTaskStartDate').attr('readonly', false);
            $("#divAddTask").attr("data-id", 0);
            $("#ddlMember").val(null).trigger('change');
           // $("#ddlStatus").val(null).trigger('change.select2');
            $("#btnTaskCreation").text("Submit");
            $('#txtTaskStartDate').attr('readonly', false);
           $rootScope.Task.length = 0;
        });

        $("#txtTaskStartDate").on('click', function () {
            var popup = $(this).offset();
            $scope.ResizeDatePicker(popup);
            
            //$(".datetimepicker").css({ top: '355.594px' });
            if ($("#divAddTask").attr("data-id") > 0)
                $('.datetimepicker').hide();
        });

        $("#txtTaskEndDate").on('click', function () {
            var popup = $(this).offset();
            $scope.ResizeDatePicker(popup);
        });


        $('#txtTaskStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true,
            
        })

        $('#txtTaskEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        //$('.Taskdate').datetimepicker().on('changeMonth', function (e) {
        //    $timeout(function () {
        //        var popup = $(this).offset();
        //        $scope.ResizeDatePicker(popup);
        //    })
            
        //}).on('hide', function (event) {
        //    return false;
        //});

        $('.Taskdate').datetimepicker().on('changeDate', function (e) {
            
            var result = true;
            var start = moment($("#txtTaskStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtTaskEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            if ($("#divAddTask").attr("data-enddate") != "") {
                var EditEndDate = moment($("#divAddTask").attr("data-enddate"), 'DD/MM/YYYY');
                var ChkEditDate = moment(EditEndDate).format('YYYY/MM/DD');
                if (!moment(end.format('YYYY/MM/DD')).isSameOrAfter(ChkEditDate)) {
                    alert("End date can't be less");
                    $("#txtTaskEndDate").val('');
                    $timeout(function () {
                        $("#txtTaskEndDate").val($("#divAddTask").attr("data-enddate"));
                    })
                    return false;
                }
            }


            //check start and end date is between project dates
            var ProjectSdate = moment($scope.ProjectPopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectPopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Task date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                result = false;
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("Task date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                result = false;
                return false;
            }

            //check start and end date is between milestone dates
            var MilestoneSdate = moment($rootScope.MilestonePopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var MilestoneEdate = moment($rootScope.MilestonePopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var msdate = moment(MilestoneSdate).format('YYYY/MM/DD');
            var medate = moment(MilestoneEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("Task date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                result = false;
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("Task date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                result = false;
                return false;
            }

            //check end date should not be less than start date
            if ($('#txtTaskStartDate').val() != "" && $('#txtTaskEndDate').val() != "") {
                if (days <= 0) {
                    alert("End date shuold be less from start date");
                    $("#txtTaskEndDate").val('');
                    $("#txtTaskDays").val('');
                    $scope.ngtxtTaskDays = '';
                    result = false;
                    return false;
                }
                $scope.ngtxtTaskDays = days;
                $("#txtTaskDays").val(days);

                if ($("#txtTaskDays").hasClass("parsley-error")) {
                    $("#txtTaskDays").removeClass("parsley-error");
                    $("#txtTaskDays").addClass("parsley-success");
                    $("#txtTaskDays").next().remove();
                }
            }

            if (result == true) {
                if ($(this).hasClass("parsley-error")) {
                    $(this).removeClass("parsley-error");
                    $(this).addClass("parsley-success");
                    $(this).next().remove();
                }
            }
        }).on('hide', function (event) {
            return false;
        });

        $('input,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

        $('.select2').on('select2:selecting', function (e) {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).next().children().first().children().css("border-color", "#22c03c");
                $(this).siblings("li").remove();
            }
        });
    }

    $scope.DeleteTask = function (index) {
       $rootScope.Task.splice(index, 1);
    }

    function clearErrorClass() {
        $('input').removeClass("parsley-success");
        $('input').removeClass("parsley-error");
        $('#ddlMember').next().children().first().children().css("border-color", "#e1e5ef");
        $("#ddlStatus").next().children().first().children().css("border-color", "#e1e5ef");
        $(".parsley-errors-list").remove();
    }

    $scope.ValidateRequest = function () {
        clearErrorClass();
        var rv = true;
        if ($scope.ngtxtTask == "" || $scope.ngtxtTask == undefined || $scope.ngtxtTask == null) {
            $("#txtTask").addClass("parsley-error");
            $("#txtTask").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtTaskStartDate == "" || $scope.ngtxtTaskStartDate == undefined || $scope.ngtxtTaskStartDate == null) {
            $("#txtTaskStartDate").addClass("parsley-error");
            $("#txtTaskStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtTaskEndDate == "" || $scope.ngtxtTaskEndDate == undefined || $scope.ngtxtTaskEndDate == null) {
            $("#txtTaskEndDate").addClass("parsley-error");
            $("#txtTaskEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngddlMember == "" || $scope.ngddlMember == undefined || $scope.ngddlMember == null) {
            $("#ddlMember").addClass("parsley-error");
            $("#ddlMember").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlMember").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        //if ($scope.ngddlStatus == "" || $scope.ngddlStatus == undefined || $scope.ngddlStatus == null) {
        //    $("#ddlStatus").addClass("parsley-error");
        //    $("#ddlStatus").next().children().first().children().css("border-color", "#ee335e");
        //    $("#ddlStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
        //    rv = false;
        //}

        if ($scope.ngtxtTaskDays == "" || $scope.ngtxtTaskDays == undefined || $scope.ngtxtTaskDays == null) {
            $("#txtTaskDays").addClass("parsley-error");
            $("#txtTaskDays").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }

    $scope.AddTask = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {
            var obj = {};
            obj.ID = $("#divAddTask").attr("data-id");
            obj.Task = $scope.ngtxtTask;
            obj.Membersvalue = $scope.ngddlMember;
            obj.Members = $scope.ngddlMember.split(':')[0];
            obj.MembersName = $("#ddlMember option:selected").text();
            obj.MembersEmail = $scope.ngddlMember.split(':')[1];
            obj.StartDateView = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtTaskDays;

            //obj.TaskStatus = $scope.ngddlStatus;
            //obj.StatusName = $("#ddlStatus option:selected").text();
            obj.Project = $rootScope.ProjectPopData.ID;
            obj.Client = $rootScope.ProjectPopData.ClientName;
            obj.Milestone = $rootScope.MilestonePopData.ID;
            obj.MileStoneName = $rootScope.MilestonePopData.MileStone;
            obj.ProjectName = $rootScope.ProjectPopData.ProjectName;

            if ($("#divAddTask").attr("data-id") > 0) {
                obj.InternalStatus = $("#divAddTask").attr("data-internalstatus");
                obj.Status = $("#divAddTask").attr("data-statusid");
                obj.Delete = "No";

            }
            else {
                obj.InternalStatus = "";
                obj.Status = "0";
            }

            $rootScope.Task.push(obj);

            //clear all the fields
            $('#txtTaskStartDate').attr('readonly', false);
            $("#divAddTask").attr("data-id", 0);
            $("#divAddTask").attr("data-internalstatus", "");
            $("#divAddTask").attr("data-statusid", "0");
            $("#divAddTask").attr("data-enddate", "");

            $scope.ngddlMember = "";
            $scope.ngddlStatus = "";
            $scope.ngtxtTask = "";
            $scope.ngtxtMileDescription = "";
            $("#txtTaskStartDate").val('');
            $("#txtTaskEndDate").val('');
            $scope.ngtxtTaskStartDate = "";
            $scope.ngtxtTaskEndDate = "";
            $scope.ngtxtTaskDays = "";
            $timeout(function () {
                $("#ddlMember").trigger('change');
                //$("#ddlStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditTask = function (index) {
        $scope.ngtxtTask =$rootScope.Task[index].Task;
        $scope.ngtxtTaskStartDate = $rootScope.Task[index].StartDateView;
        $("#txtTaskStartDate").val($scope.Task[index].StartDateView);
        $scope.ngtxtTaskEndDate = $rootScope.Task[index].EndDateView;
        $("#txtTaskEndDate").val($scope.Task[index].EndDateView);
        $scope.ngtxtTaskDays =$rootScope.Task[index].NoOfDays;

        $timeout(function () {
            
            //$scope.ngddlMember = $scope.Task[index].Members;
            $("#ddlMember").val("string:" + $rootScope.Task[index].Membersvalue).trigger('change');
            //$("#ddlStatus").val($scope.Task[index].TaskStatus).trigger('change');
            $rootScope.Task.splice(index, 1);
        }, 10);

        $("#divAddTask").attr("data-internalstatus", $rootScope.Task[index].InternalStatus);
        $("#divAddTask").attr("data-statusid", $rootScope.Task[index].Status);
        $("#divAddTask").attr("data-id", $rootScope.Task[index].ID);
        if ($rootScope.Task[index].ID > 0) {
            $('#txtTaskStartDate').attr('readonly', true);
            $("#divAddTask").attr("data-enddate", $rootScope.Task[index].EndDateView);
        }

    }

    $scope.FinalAddTask = function () {
        if ($scope.Task.length > 0) {
            $rootScope.TaskLoad = true;
            $(".overlay").show();
            if ($rootScope.EditTask.length > 0) {
                var data = {
                    'AddTask': $rootScope.Task,
                    'Action': "Update",
                }
            }
            else {
                var data = {
                    'AddTask': $rootScope.Task,
                    'Action': "Insert",
                }
            }
            //var AddTask = new Array();
            //AddTask =$rootScope.Task;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddTask", data).then(function (response) {
                if (response.data[0] == "OK") {
                    
                    var data = {
                        'ProjectId': $rootScope.ProjectPopData.ID
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                        if (response.data[0] == "OK") {
                            $rootScope.BindMilestoneTable(response, $rootScope.ProjectPopData, $rootScope.MiletableRow);
                            $rootScope.LoadTaskTab();
                            $timeout(function () {
                                $rootScope.TaskLoad = false;
                                $("#AddTaskPopUp").modal("hide");
                                $(".overlay").hide();
                            },100)
                        }

                    });

                }
                else {
                    alert("Something went wrong. Please try after some time.");
                    $(".overlay").show();
                    $rootScope.TaskLoad = false;
                }
                   
            });
        }
        else
            $scope.ValidateRequest();
    }

});

ProjectDashboardApp.controller('AddSubTaskController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $rootScope.SubTask = [];

    $scope.LoadSubTask = function () {

        $('#AddSubTaskPopUp').on('hide.bs.modal', function () {
            $rootScope.PrevUploadFiles.length = 0;
            $("#frmProjectDashboard")[0].reset();
            $("#divAddSubTask").attr("data-id", 0);
            $('#txtSubTaskStartDate').attr('readonly', false);
            $("#ddlSubTaskMember").val(null).trigger('change');
            $("#ddlSubTaskStatus").val(null).trigger('change.select2');
            $("#btnSubTaskCreation").text("Submit");
            $rootScope.SubTask.length = 0;
        });

        $("#txtSubTaskStartDate").on('click', function () {
            var popup = $(this).offset();
            var popupTop = popup.top - 120;
            $('.datetimepicker').css({
                'top': popupTop
            });
            //$(".datetimepicker").css({ top: '570.594px' });
            if ($("#divAddSubTask").attr("data-id") > 0)
                $('.datetimepicker').hide();
        });

        $("#txtSubTaskEndDate").on('click', function () {
            var popup = $(this).offset();
            var popupTop = popup.top - 120;
            $('.datetimepicker').css({
                'top': popupTop
            });
        });

        $('#txtSubTaskStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtSubTaskEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('.SubTaskdate').datetimepicker().on('changeDate', function (e) {

            var start = moment($("#txtSubTaskStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtSubTaskEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

            if ($("#divAddSubTask").attr("data-enddate") != "") {
                var EditEndDate = moment($("#divAddSubTask").attr("data-enddate"), 'DD/MM/YYYY');
                var ChkEditDate = moment(EditEndDate).format('YYYY/MM/DD');
                if (!moment(end.format('YYYY/MM/DD')).isSameOrAfter(ChkEditDate)) {
                    alert("End date can't be less");
                    $("#txtSubTaskEndDate").val('');
                    $timeout(function () {
                        $("#txtSubTaskEndDate").val($("#divAddSubTask").attr("data-enddate"));
                    })
                    return false;
                }
            }

            //check start and end date is between project dates
            var ProjectSdate = moment($scope.ProjectPopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var ProjectEdate = moment($scope.ProjectPopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var sdate = moment(ProjectSdate).format('YYYY/MM/DD');
            var edate = moment(ProjectEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("SubTask date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(sdate, edate, undefined, '[]')) {
                alert("SubTask date range should be within the Project date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check start and end date is between milestone dates
            var MilestoneSdate = moment($rootScope.MilestonePopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var MilestoneEdate = moment($rootScope.MilestonePopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var msdate = moment(MilestoneSdate).format('YYYY/MM/DD');
            var medate = moment(MilestoneEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(msdate, medate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check start and end date is between Task dates
            var TaskSdate = moment($scope.TaskPopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var TaskEdate = moment($scope.TaskPopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

            var tsdate = moment(TaskSdate).format('YYYY/MM/DD');
            var tedate = moment(TaskEdate).format('YYYY/MM/DD');

            if (!moment(start.format('YYYY/MM/DD')).isBetween(tsdate, tedate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileStartDate = "";
                return false;
            }

            if (!moment(end.format('YYYY/MM/DD')).isBetween(tsdate, tedate, undefined, '[]')) {
                alert("SubTask date range should be within the Milestone date range");
                $("#" + this.id).val('');
                $scope.ngtxtMileEndDate = "";
                return false;
            }

            //check end date should not be less than start date
            if ($('#txtSubTaskStartDate').val() != "" && $('#txtSubTaskEndDate').val() != "") {
                if (days <= 0) {
                    alert("End date should be less from start date");
                    $("#txtSubTaskEndDate").val('');
                    $("#txtSubTaskDays").val('');
                    $scope.ngtxtSubTaskDays = '';
                    return false;
                }
                $scope.ngtxtSubTaskDays = days;
                $("#txtSubTaskDays").val(days);
                if ($("#txtSubTaskDays").hasClass("parsley-error")) {
                    $("#txtSubTaskDays").removeClass("parsley-error");
                    $("#txtSubTaskDays").addClass("parsley-success");
                    $("#txtSubTaskDays").next().remove();
                }
            }
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        }).on('hide', function (event) {
            return false;
        });

        $('input,select,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });

        $('.select2').on('select2:selecting', function (e) {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).next().children().first().children().css("border-color", "#22c03c");
                $(this).siblings("li").remove();
            }
        });

    }

    $scope.DeleteSubTask = function (index) {
        $rootScope.SubTask.splice(index, 1);
    }

    function clearErrorClass() {
        $('input').removeClass("parsley-success");
        $('input').removeClass("parsley-error");
        $('#ddlSubTaskMember').next().children().first().children().css("border-color", "#e1e5ef");
        $("#ddlSubTaskStatus").next().children().first().children().css("border-color", "#e1e5ef");
        $(".parsley-errors-list").remove();
    }

    $scope.ValidateRequest = function () {
        clearErrorClass();
        var rv = true;
        if ($scope.ngtxtSubTask == "" || $scope.ngtxtSubTask == undefined || $scope.ngtxtSubTask == null) {
            $("#txtSubTask").addClass("parsley-error");
            $("#txtSubTask").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskStartDate == "" || $scope.ngtxtSubTaskStartDate == undefined || $scope.ngtxtSubTaskStartDate == null) {
            $("#txtSubTaskStartDate").addClass("parsley-error");
            $("#txtSubTaskStartDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskEndDate == "" || $scope.ngtxtSubTaskEndDate == undefined || $scope.ngtxtSubTaskEndDate == null) {
            $("#txtSubTaskEndDate").addClass("parsley-error");
            $("#txtSubTaskEndDate").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        //if ($scope.ngddlSubTaskStatus == "" || $scope.ngddlSubTaskStatus == undefined || $scope.ngddlSubTaskStatus == null) {
        //    $("#ddlSubTaskStatus").addClass("parsley-error");
        //    $("#ddlSubTaskStatus").next().children().first().children().css("border-color", "#ee335e");
        //    $("#ddlSubTaskStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
        //    rv = false;
        //}

        if ($scope.ngddlSubTaskMember == "" || $scope.ngddlSubTaskMember == undefined || $scope.ngddlSubTaskMember == null) {
            $("#ddlSubTaskMember").addClass("parsley-error");
            $("#ddlSubTaskMember").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlSubTaskMember").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        if ($scope.ngtxtSubTaskDays == "" || $scope.ngtxtSubTaskDays == undefined || $scope.ngtxtSubTaskDays == null) {
            $("#txtSubTaskDays").addClass("parsley-error");
            $("#txtSubTaskDays").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

        return rv;
    }

    $scope.AddSubTask = function () {
        var ValidateStatus = $scope.ValidateRequest();
        if (ValidateStatus) {
            var obj = {};
            obj.ID = $("#divAddSubTask").attr("data-id");
            obj.SubTask = $scope.ngtxtSubTask;
            obj.Membersvalue = $scope.ngddlMember;
            obj.Members = $scope.ngddlSubTaskMember.split(':')[0];
            obj.MembersName = $("#ddlSubTaskMember option:selected").text();
            obj.MembersEmail = $scope.ngddlSubTaskMember.split(':')[1];
            obj.StartDateView = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtSubTaskDays;
            //obj.SubTaskStatus = $scope.ngddlSubTaskStatus;
            //obj.StatusName = $("#ddlSubTaskStatus option:selected").text();
            obj.Project = $scope.ProjectPopData.ID;
            obj.Client = $rootScope.ProjectPopData.ClientName;
            obj.Milestone = $rootScope.MilestonePopData.ID;
            obj.Task = $rootScope.TaskPopData.ID;

            obj.MileStoneName = $rootScope.MilestonePopData.MileStone;
            obj.ProjectName = $rootScope.ProjectPopData.ProjectName;
            obj.TaskName = $rootScope.TaskPopData.Task;

            if ($("#divAddSubTask").attr("data-id") > 0) {
                obj.InternalStatus = $("#divAddSubTask").attr("data-internalstatus");
                obj.Status = $("#divAddSubTask").attr("data-statusid");
                obj.Delete = "No";

            }
            else {
                obj.InternalStatus = "";
                obj.Status = "0";
            }

            $rootScope.SubTask.push(obj);

            $('#txtSubTaskStartDate').attr('readonly', false);
            $("#divAddSubTask").attr("data-id", 0);
            $("#divAddSubTask").attr("data-internalstatus", "");
            $("#divAddSubTask").attr("data-statusid", "0");
            $("#divAddSubTask").attr("data-enddate", "");


            //clear all the fields
            $scope.ngddlSubTaskMember = "";
            $scope.ngddlSubTaskStatus = "";
            $scope.ngtxtSubTask = "";
            $scope.ngtxtMileDescription = "";
            $("#txtSubTaskStartDate").val('');
            $("#txtSubTaskEndDate").val('');
            $scope.ngtxtSubTaskDays = "";
            $timeout(function () {
                $("#ddlSubTaskMember").trigger('change');
                //$("#ddlSubTaskStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditSubTask = function (index) {
        $scope.ngtxtSubTask = $rootScope.SubTask[index].SubTask;
        $scope.ngtxtSubTaskStartDate = $rootScope.SubTask[index].StartDateView;
        $("#txtSubTaskStartDate").val($rootScope.SubTask[index].StartDateView);
        $scope.ngtxtSubTaskEndDate = $rootScope.SubTask[index].EndDateView;
        $("#txtSubTaskEndDate").val($rootScope.SubTask[index].EndDateView);
        $scope.ngtxtSubTaskDays = $rootScope.SubTask[index].NoOfDays;

        $timeout(function () {
            $("#ddlSubTaskMember").val("string:" + $rootScope.SubTask[index].Membersvalue).trigger('change');
            //$("#ddlSubTaskStatus").val($rootScope.SubTask[index].SubTaskStatus).trigger('change');
            $rootScope.SubTask.splice(index, 1);
        }, 10);

        $("#divAddSubTask").attr("data-internalstatus", $rootScope.SubTask[index].InternalStatus);
        $("#divAddSubTask").attr("data-statusid", $rootScope.SubTask[index].Status);
        $("#divAddSubTask").attr("data-id", $rootScope.SubTask[index].ID);
        if ($rootScope.SubTask[index].ID > 0) {
            $('#txtSubTaskStartDate').attr('readonly', true);
            $("#divAddSubTask").attr("data-enddate", $rootScope.SubTask[index].EndDateView);
        }
    }

    $scope.FinalAddSubTask = function () {
        if ($rootScope.SubTask.length > 0) {
            $rootScope.SubTaskLoad = true;
            $(".overlay").show();
            if ($rootScope.EditSubTask.length > 0) {
                var data = {
                    'AddSubTask': $rootScope.SubTask,
                    'Action': "Update"
                }
            }
            else {
                var data = {
                    'AddSubTask': $rootScope.SubTask,
                    'Action': "Insert"
                }
            }
            //var AddSubTask = new Array();
            //AddSubTask = $rootScope.SubTask;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddSubTask", data).then(function (response) {
                if (response.data[0] == "OK") {

                    var data = {
                        'MilestoneId': $rootScope.MilestonePopData.ID,
                        'Members': $rootScope.ProjectPopData.Members,
                        'ProjectManager': $rootScope.ProjectPopData.ProjectManager
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                        if (response.data[0] == "OK") {

                            if ($rootScope.TaskRow != undefined) {
                                $rootScope.LoadTaskTab();
                                $rootScope.BindTask(response, $rootScope.MilestonePopData, $rootScope.ProjectPopData, $rootScope.TaskRow);
                            }
                            else
                                $rootScope.LoadProjectData();

                            $timeout(function () {
                                $rootScope.SubTaskLoad = false;
                                $("#AddSubTaskPopUp").modal("hide");
                                $(".overlay").hide();
                            }, 100);
                            

                        }
                    });
                }
                else {
                    alert("Something went wrong. Please try after some time.");
                    $rootScope.SubTaskLoad = false;
                    $(".overlay").show();
                }
                    
            });
        }
        else
            $scope.ValidateRequest();
    }

});
