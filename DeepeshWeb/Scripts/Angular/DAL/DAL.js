var CommonAppUtility = angular.module('CommonAppUtility', []);

CommonAppUtility.service('CommonAppUtilityService', function ($http, $q) {


    var jsonHeader = 'application/json;odata=verbose';
    var headers = {
        'Content-Type': jsonHeader,
        'Accept': jsonHeader
    };


    this.CreateItem = function (Url, jsonBody) {

        var spsite = getUrlVars()["SPHostUrl"];
        Url = Url + "?SPHostUrl=" + spsite;
        doc = jsonBody.Upload;

        //var fileData = new FormData();
        //for (var i = 0; i < doc.length; i++) {
        //    fileData.append(doc[i].name, doc[i]);
        //} 

        //  this.test(fileData, Url);
        var data = JSON.stringify(jsonBody),

            configinfo = {
                method: 'POST',
                url: Url,
                data: data,
                headers: headers
            };


        return $http(configinfo);
    }


 


});