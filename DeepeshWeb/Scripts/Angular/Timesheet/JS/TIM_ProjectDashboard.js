var ProjectDashboardApp = angular.module('ProjectDashboardApp', ['CommonAppUtility'])

ProjectDashboardApp.controller('ProjectDashboardController', function ($scope, $http, $compile, $timeout, $rootScope, CommonAppUtilityService) {
    var table;
    $rootScope.ProjectPopData = [];
    $scope.ProjectDetails = [];
    $rootScope.ProjectData = [];
    var ProjectID;
    var AllDataTableId = {};

    $(function () {
       
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

    $rootScope.LoadProjectData = function () {
        $('#tblProject').DataTable().clear().destroy();
        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetProjectData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $rootScope.ProjectData = response.data[1];
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
                }, 2);
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
        var MileID = response.data[1][0].ID;
        $scope.Project = ProjectData;
        $scope.MileRow = row;
        var Html = '<div><table class="mg-b-0 text-md-nowrap dataTable" style="width: 82%;" id="tblMilestone' + MileID + '" ><thead><tr><th scope="col" style="padding-left: 35px;">Milestone</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th></tr></thead> <tbody>';
        angular.forEach(response.data[1], function (value, key) {
            //this.push(key + ': ' + value);
            $scope["Mile" + value.ID] = value;
            var i = key + 1;
            Html += '<tr><td data-label="Milestone"><span class = "details-td" id = "Task' + value.ID + '">';
            if (value.InternalStatus != "MilestoneCreated") {
                Html += '<span><i class="si si si-plus" ng-click = "ShowTask(' + value.ID + ', Mile' + value.ID + ', Project)"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
            }
            else {
                Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';
            }

            Html += '</span >' + value.MileStone + '</td><td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td>';

            if (value.InternalStatus == "MilestoneCreated")
                Html += '<button class="badge badge-primary" type="button" ng-click="OpenAddTaskPop(Mile' + value.ID + ', Project, MileRow)">Add Task</button>'

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

        }, 2);
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
                'MilestoneId': Milestone.ID
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
        $scope.TaskMilestone = Milestone;
        $scope.TaskProjectData = Project;
        $scope.TaskRow = row;

        var Html = '<div><table class="table key-buttons text-md-nowrap " id="tblTask' + TaskID + '" style="width:82%" ><thead><tr><th scope="col" style="padding-left: 35px;">Task</th><th scope="col">Member</th><th scope="col">Start Date</th><th scope="col">Estimated End Date</th><th scope="col">Days</th><th scope="col">Action</th></tr></thead> <tbody>';
        angular.forEach(response.data[1], function (value, key) {
            var i = key + 1;
            $scope["Task" + value.ID] = value;
            Html += '<tr><td data-label="Task"><span class = "details-td" id = "SubTask' + value.ID + '">';
            if (value.InternalStatus != "TaskCreated")
                Html += '<span><i class="si si si-plus" ng-click = "ShowSubTask(' + value.ID + ', ' + value.ID + ')"  aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;</span>';
            else
                Html += '<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>';

            Html += '</span >' + value.Task + '</td> <td data-label="Member">' + value.MembersName + '</td> <td data-label="Start Date">' + value.StartDate.split(" ")[0] + '</td> <td data-label="Estimated End Date">' + value.EndDate.split(" ")[0] + '</td> <td data-label="Days">' + value.NoOfDays + '</td><td> ';

            if (value.InternalStatus == "TaskCreated")
                Html += '<button class="badge badge-primary" type="button" ng-click="OpenAddSubTaskPop(Task' + value.ID + ', TaskMilestone, TaskProjectData, TaskRow)">Add SubTask</button>';

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

    $scope.AddMilestonePopUp = function (Project) {
        //$.cookie('ProjectId', ProjectId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        //$("#frmProjectDashboard")[0].reset();
        $rootScope.ProjectPopData = Project;
        $("#AddMilestonePopUp").modal("show");


    }

    $scope.OpenAddTaskPop = function (Milestone, Project, MileTableRow) {
        //$.cookie('ProjectId', ProjectId);
        //$.cookie('MilestoneId', MilestoneId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        $rootScope.MiletableRow = MileTableRow;
        $rootScope.ProjectPopData = Project;
        $rootScope.MilestonePopData = Milestone;
        $("#AddTaskPopUp").modal("show");

    }

    $scope.OpenAddSubTaskPop = function (Task, Milestone, Project, TaskTableRow) {
        //$.cookie('TaskId', TaskId);
        //$.cookie('ProjectId', ProjectId);
        //$.cookie('MilestoneId', MilestoneId);
        //var spsite = getUrlVars()["SPHostUrl"];
        //Url = '/TIM_AddSubTask' + "?SPHostUrl=" + spsite;
        //window.location.href = Url;
        $rootScope.TaskRow =TaskTableRow;
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

        if ($scope.ngProjectDelete == true)
            data.InternalStatus = "ProjectDeleted";

        CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/SaveProject", data).then(function (response) {
            if (response.data[0] == "OK") {
                $scope.ProjectCreationLoad = false;
                $('#AddProjectPopUp').modal('hide');
                $scope.LoadProjectData();
            }
            else
                $scope.ProjectCreationLoad = false;
        });
    }


    //$scope.AddProject = function () {
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_ProjectCreation' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}


    //function for Project Deletion Alert Message
    //$scope.ProjectDeletionAlert = function (ProjectId) {
    //    swal({
    //        title: "Project Deletion",
    //        text: "Are you sure do you really want to delete this project?",
    //        type: "info",
    //        showCancelButton: true,
    //        closeOnConfirm: false,
    //        showLoaderOnConfirm: true
    //    }, function () {
    //        $scope.DeleteProject(ProjectId);
    //    });
    //}

    //$scope.DeleteProject = function (ProjectId) {
    //    $.cookie('ProjectId', ProjectId);
    //    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/DeleteProject", "").then(function (response) {
    //        if (response.data[0] == "OK") {
    //            swal("Project deleted successfully.");
    //            $scope.LoadProjectData();
    //        }
    //    });
    //}


    //$scope.GoToMilestone = function (ProjectId) {
    //    $.cookie('ProjectId', ProjectId);
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_AddMilestone' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}

    //$scope.GoToTask = function (ProjectId) {
    //    $.cookie('ProjectId', ProjectId);
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_AddTask' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}

    //$scope.GoToSubTask = function (ProjectId) {
    //    $.cookie('ProjectId', ProjectId);
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_AddSubTask' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}

    //$scope.GoToLanding = function (ProjectId) {
    //    $.cookie('ProjectId', ProjectId);
    //    var spsite = getUrlVars()["SPHostUrl"];
    //    Url = '/TIM_DashboardLanding' + "?SPHostUrl=" + spsite;
    //    window.location.href = Url;
    //}
});

ProjectDashboardApp.controller('AddMilestoneController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $scope.Milestone = [];

    $scope.OnLoadMilestone = function () {

            // do something
        $('#AddMilestonePopUp').on('hide.bs.modal', function () {
             $scope.ngtxtMileDays = "";
            $("#frmProjectDashboard")[0].reset();
            $scope.Milestone.length = 0;
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
    $(function () {
        //$scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        //$scope.MilestoneDetails = $.parseJSON($("#hdnMilestoneData").val());
        //angular.forEach($scope.MilestoneDetails, function (value, key) {
        //    var obj = {};
        //    obj.Milestone = value.MileStone;
        //    obj.Description = value.Description;
        //    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.NoOfDays = value.NoOfDays;
        //    obj.Project = $scope.ProjectData.ID;
        //    obj.ProjectManager = $scope.ProjectData.ProjectManager;
        //    obj.MembersText = $scope.ProjectData.MembersText;
        //    obj.Members = $scope.ProjectData.Members;
        //    $scope.Milestone.push(obj);
        //});
        //$scope.$apply();
        // AmazeUI Datetimepicker



    });

    $scope.DeleteMilestone = function (index) {
        $scope.Milestone.splice(index, 1);
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
            $scope.Milestone.push(obj);

            //clear all the fields
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
        $scope.ngtxtMilestone = $scope.Milestone[index].Milestone;
        $scope.ngtxtMileDescription = $scope.Milestone[index].Description;
        $scope.ngtxtMileStartDate = $scope.Milestone[index].StartDateView;
        $("#txtMileStartDate").val($scope.Milestone[index].StartDateView);
        $scope.ngtxtMileEndDate = $scope.Milestone[index].EndDateView;
        $("#txtMileEndDate").val($scope.Milestone[index].EndDateView);
        $scope.ngtxtMileDays = $scope.Milestone[index].NoOfDays;
        $scope.Milestone.splice(index, 1);
    }

    $scope.FinalAddMilestone = function () {
        if ($scope.Milestone.length > 0) {
            $scope.MilestoneCreationLoad = true;
            var AddMilestone = new Array();
            AddMilestone = $scope.Milestone;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddMilestone", AddMilestone).then(function (response) {
                if (response.data[0] == "OK") {
                    $scope.MilestoneCreationLoad = false;
                    $("#AddMilestonePopUp").modal("hide");
                    $rootScope.LoadProjectData();
                }
                else
                    $scope.MilestoneCreationLoad = false;
            });
        }
        else {
            $scope.ValidateRequest();
        }
    }
});

ProjectDashboardApp.controller('AddTaskController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $scope.Task = [];

    $scope.OnLoadTask = function () {

        $('#AddTaskPopUp').on('hide.bs.modal', function () {
            $("#frmProjectDashboard")[0].reset();
            $("#ddlMember").val(null).trigger('change.select2');
            $("#ddlStatus").val(null).trigger('change.select2');
            $scope.Task.length = 0;
        });

        $('#txtTaskStartDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });

        $('#txtTaskEndDate').datetimepicker({
            minView: 2,
            format: 'dd-mm-yyyy',
            autoclose: true
        });


        $('.Taskdate').datetimepicker().on('changeDate', function (e) {
            var result = true;
            var start = moment($("#txtTaskStartDate").val(), 'DD/MM/YYYY');
            var end = moment($("#txtTaskEndDate").val(), 'DD/MM/YYYY');
            var days = end.diff(start, 'days');

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
            var MilestoneSdate = moment($scope.MilestonePopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var MilestoneEdate = moment($scope.MilestonePopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

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

            if (result==true) {
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

    $(function () {

        //$scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        //$scope.MilestoneData = $.parseJSON($("#hdnMilestoneData").val());
        //$scope.TaskData = $.parseJSON($("#hdnTaskData").val());
        //angular.forEach($scope.TaskData, function (value, key) {
        //    var obj = {};
        //    obj.Task = value.Task;
        //    obj.Members = value.Members;
        //    obj.MemberTitle = value.MembersName;
        //    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.NoOfDays = value.NoOfDays;
        //    obj.TaskStatus = value.TaskStatus;
        //    obj.StatusName = value.TaskStatusName;
        //    obj.Project = $scope.ProjectData.ID;
        //    obj.Milestone = $scope.MilestoneData.ID;
        //    $scope.Task.push(obj);
        //});
        //$scope.$apply();

        // AmazeUI Datetimepicker

    });

    $scope.DeleteTask = function (index) {
        $scope.Task.splice(index, 1);
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

        if ($scope.ngddlStatus == "" || $scope.ngddlStatus == undefined || $scope.ngddlStatus == null) {
            $("#ddlStatus").addClass("parsley-error");
            $("#ddlStatus").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

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
            obj.Task = $scope.ngtxtTask;
            obj.Members = $scope.ngddlMember;
            obj.MemberTitle = $("#ddlMember option:selected").text();
            obj.StartDateView = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtTaskDays;
            obj.TaskStatus = $scope.ngddlStatus;
            obj.StatusName = $("#ddlStatus option:selected").text();
            obj.Project = $scope.ProjectPopData.ID;
            obj.Milestone = $scope.MilestonePopData.ID;
            $scope.Task.push(obj);

            //clear all the fields
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
                $("#ddlStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditTask = function (index) {
        $scope.ngtxtTask = $scope.Task[index].Task;
        $scope.ngtxtTaskStartDate = $scope.Task[index].StartDate;
        $("#txtTaskStartDate").val($scope.Task[index].StartDate);
        $scope.ngtxtTaskEndDate = $scope.Task[index].EndDate;
        $("#txtTaskEndDate").val($scope.Task[index].EndDate);
        $scope.ngtxtTaskDays = $scope.Task[index].NoOfDays;

        $timeout(function () {
            $("#ddlMember").val($scope.Task[index].Members).trigger('change');
            $("#ddlStatus").val($scope.Task[index].TaskStatus).trigger('change');
            $scope.Task.splice(index, 1);
        }, 10);

    }

    $scope.FinalAddTask = function () {
        if ($scope.Task.length > 0) {
            $scope.TaskLoad = true;
            var AddTask = new Array();
            AddTask = $scope.Task;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddTask", AddTask).then(function (response) {
                if (response.data[0] == "OK") {
                    //$('#SuccessModelTask').modal('show');
                    $scope.TaskLoad = false;
                    $("#AddTaskPopUp").modal("hide");
                    var data = {
                        'ProjectId': $rootScope.ProjectPopData.ID
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetMilestone", data).then(function (response) {
                        if (response.data[0] == "OK") {
                            $rootScope.BindMilestoneTable(response, $rootScope.ProjectPopData, $rootScope.MiletableRow);
                        }

                    });

                }
                else
                    $scope.TaskLoad = false;
            });
        }
        else
            $scope.ValidateRequest();
    }

});

ProjectDashboardApp.controller('AddSubTaskController', function ($scope, $http, $rootScope, $timeout, CommonAppUtilityService) {

    $scope.SubTask = [];

    $scope.LoadSubTask = function () {

        $('#AddSubTaskPopUp').on('hide.bs.modal', function () {
            $("#frmProjectDashboard")[0].reset();
            $("#ddlSubTaskMember").val(null).trigger('change.select2');
            $("#ddlSubTaskStatus").val(null).trigger('change.select2');
            $scope.SubTask.length = 0;
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
            var MilestoneSdate = moment($scope.MilestonePopData.StartDate.split(' ')[0], 'DD/MM/YYYY');
            var MilestoneEdate = moment($scope.MilestonePopData.EndDate.split(' ')[0], 'DD/MM/YYYY');

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

    $(function () {

        //$scope.ProjectData = $.parseJSON($("#hdnProjectData").val());
        //$scope.MilestoneData = $.parseJSON($("#hdnMilestoneData").val());
        //$scope.TaskData = $.parseJSON($("#hdnTaskData").val());
        //$scope.SubTaskData = $.parseJSON($("#hdnSubTaskData").val());

        //angular.forEach($scope.SubTaskData, function (value, key) {
        //    var obj = {};
        //    obj.SubTask = value.SubTask;
        //    obj.Members = value.Members;
        //    obj.MemberTitle = value.MembersName;
        //    obj.StartDateView = moment(value.StartDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.EndDateView = moment(value.EndDate, 'DD-MM-YYYY').format("DD-MM-YYYY");
        //    obj.StartDate = moment(value.StartDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.EndDate = moment(value.EndDate, 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
        //    obj.NoOfDays = value.NoOfDays;
        //    obj.SubTaskStatus = value.SubTaskStatus;
        //    obj.StatusName = value.SubTaskStatusName;
        //    obj.Project = $scope.ProjectData.ID;
        //    obj.Milestone = $scope.MilestoneData.ID;
        //    $scope.SubTask.push(obj);
        //});
        //$scope.$apply();

        // AmazeUI Datetimepicker
    });

    $scope.DeleteSubTask = function (index) {
        $scope.SubTask.splice(index, 1);
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

        if ($scope.ngddlSubTaskStatus == "" || $scope.ngddlSubTaskStatus == undefined || $scope.ngddlSubTaskStatus == null) {
            $("#ddlSubTaskStatus").addClass("parsley-error");
            $("#ddlSubTaskStatus").next().children().first().children().css("border-color", "#ee335e");
            $("#ddlSubTaskStatus").parent().append("<li class='parsley - required parsley-errors-list filled erralign'>This value is required.</li>");
            rv = false;
        }

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
            obj.SubTask = $scope.ngtxtSubTask;
            obj.Members = $scope.ngddlSubTaskMember;
            obj.MemberTitle = $("#ddlSubTaskMember option:selected").text();
            obj.StartDateView = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.EndDateView = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("DD-MM-YYYY");
            obj.StartDate = moment($("#txtSubTaskStartDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.EndDate = moment($("#txtSubTaskEndDate").val(), 'DD-MM-YYYY').format("MM-DD-YYYY hh:mm:ss");
            obj.NoOfDays = $scope.ngtxtSubTaskDays;
            obj.SubTaskStatus = $scope.ngddlSubTaskStatus;
            obj.StatusName = $("#ddlSubTaskStatus option:selected").text();
            obj.Project = $scope.ProjectPopData.ID;
            obj.Milestone = $scope.MilestonePopData.ID;
            obj.Task = $scope.TaskPopData.ID;
            $scope.SubTask.push(obj);

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
                $("#ddlSubTaskStatus").trigger('change');
            }, 10);


        }
    }

    $scope.EditSubTask = function (index) {
        $scope.ngtxtSubTask = $scope.SubTask[index].SubTask;
        $scope.ngtxtSubTaskStartDate = $scope.SubTask[index].StartDate;
        $("#txtSubTaskStartDate").val($scope.SubTask[index].StartDate);
        $scope.ngtxtSubTaskEndDate = $scope.SubTask[index].EndDate;
        $("#txtSubTaskEndDate").val($scope.SubTask[index].EndDate);
        $scope.ngtxtSubTaskDays = $scope.SubTask[index].NoOfDays;

        $timeout(function () {
            $("#ddlSubTaskMember").val($scope.SubTask[index].Members).trigger('change');
            $("#ddlSubTaskStatus").val($scope.SubTask[index].SubTaskStatus).trigger('change');
            $scope.SubTask.splice(index, 1);
        }, 10);

    }

    $scope.FinalAddSubTask = function () {
        if ($scope.SubTask.length > 0) {
            $scope.SubTaskLoad = true;
            var AddSubTask = new Array();
            AddSubTask = $scope.SubTask;

            CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/AddSubTask", AddSubTask).then(function (response) {
                if (response.data[0] == "OK") {
                    //$('#SuccessModelSubTask').modal('show');
                    $scope.SubTaskLoad = false;
                    $("#AddSubTaskPopUp").modal("hide");

                    var data = {
                        'MilestoneId': $rootScope.MilestonePopData.ID
                    }
                    CommonAppUtilityService.CreateItem("/TIM_ProjectDashboard/GetTask", data).then(function (response) {
                        if (response.data[0] == "OK") {
                            $rootScope.BindTask(response, $rootScope.MilestonePopData, $rootScope.ProjectPopData, $rootScope.TaskRow);
                        }
                    });
                }
                else
                    $scope.SubTaskLoad = false;
            });
        }
        else
            $scope.ValidateRequest();
    }

});
