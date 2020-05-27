var ProjectCreationServiceModule = angular.module('ProjectCreationServiceModule', ['CommonAppUtility']);

var jsonheaders = { 'headers': { 'accept': 'application/json;odata=verbose' } };

ProjectCreationServiceModule.service('ProjectCreationService', function ($http, CommonAppUtilityService) {

    this.SaveProject = function (option) {
        return CommonAppUtilityService.CreateItem("/ProjectCreation/SaveProject", option);
    }

});