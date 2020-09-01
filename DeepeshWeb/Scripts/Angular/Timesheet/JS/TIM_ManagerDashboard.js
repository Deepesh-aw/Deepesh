var ManagerDashboardApp = angular.module('ManagerDashboardApp', ['CommonAppUtility'])
ManagerDashboardApp.controller('ManagerDashboardController', function ($scope, $http, $timeout, CommonAppUtilityService) {

    $scope.CurrentTimesheet = [];

    //$(function () {
    //    $('#ViewTimesheetPopUp').on('hide.bs.modal', function () {
    //        $scope.Report = true;
    //    });
    //})

    $scope.TimesheetLoad = function () {
        $(".overlay").hide();
        $('input,select,textarea').keypress(function () {
            if ($(this).hasClass("parsley-error")) {
                $(this).removeClass("parsley-error");
                $(this).addClass("parsley-success");
                $(this).next().remove();
            }
        });
        CommonAppUtilityService.CreateItem("/TIM_ManagerDashboard/LoadTimesheetData", "").then(function (response) {
            if (response.data[0] == "OK") {
                $scope.BindTimesheetData = response.data[1];

                $('#Pending').DataTable().clear().destroy();
                $('#Approved').DataTable().clear().destroy();
                $('#Rejected').DataTable().clear().destroy();

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

                    $('#Rejected').DataTable({
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

    $scope.ApproveTimesheet = function (Timesheet, Action, ID, iclass) {
        $("#Load" + ID).removeClass(iclass);
        $("#Load" + ID).addClass('spinner-border spinner-border-sm');
        var data = {
            'TimesheetId': Timesheet.TimesheetID
        }
        CommonAppUtilityService.CreateItem("/TIM_TimesheetDashboard/GetEditTimesheet", data).then(function (response) {
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

                $("#Load" + ID).removeClass('spinner-border spinner-border-sm');
                $("#Load" + ID).addClass(iclass);

                $("#ViewTimesheetPopUp").modal('show');
            }
            else {
                $("#Load" + ID).removeClass('spinner-border spinner-border-sm');
                $("#Load" + ID).addClass(iclass);
            }
        });
    }

    $scope.TimeFormat = function (t) {
        if (t != undefined || t != null) {
            return moment(t, ["dd-MM-yyyy h:mm:ss"]).format("HH:mm");
        }
    }

    function clearErrorClass() {
        $('textarea').removeClass("parsley-success");
        $('textarea').removeClass("parsley-error");
        $(".parsley-errors-list").remove();
    }

    $scope.FinalApproveTimesheet = function (Action) {
        clearErrorClass();
        if ($scope.ngRemark == undefined || $scope.ngRemark == null || $scope.ngRemark == "") {
            $("#txtRemark").addClass("parsley-error");
            $("#txtRemark").parent().append("<li class='parsley-required parsley-errors-list filled erralign'>This value is required.</li>");
            return false;
        }

        $(".overlay").show();
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
                $(".overlay").hide();
                clearErrorClass();
                $scope.ngRemark = "";
            }
            else {
                alert("Something went wrong. Please try after some time.");
                $scope.ApproveLoad = false;
                $scope.RejectLoad = false;
                $(".overlay").hide();

            }

        });

    }

    $scope.GetTotalHours = function () {
        var Hours = 0, Minute = 0;
        angular.forEach($scope.CurrentTimesheet, function (value, key) {
            Hours = Number(Hours) + Number(value.Hours.split(':')[0]);
            Minute = Number(Minute) + Number(value.Hours.split(':')[1]);
            hour_carry = 0;

            if (Minute > 59) {
                hour_carry = Minute / 60 | 0;
                Minute = Minute % 60 | 0;
            }

            Hours = Hours + Number(hour_carry);

            if (Minute.toString().length == 1)
                Minute = "0" + Minute.toString();
        });
        return Hours + ":" + Minute;
    }
});