

function ajaxcall() {

    $.ajax({
        type: "GET",
        url: "/Navigation?SPHostUrl=https://ascenworktech.sharepoint.com/sites/Wizrr-Development/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $("#navbar").append(response);

        },
        error: function (response) {
            alert(response);
            console.log(response)
        },
        failure: function (response) {
            alert(response);
            console.log(response)
        }
    });

}


ajaxcall();
