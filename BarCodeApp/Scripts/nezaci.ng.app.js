/// <reference path="nezaci.js" />
/// <reference path="/scripts/ng/angular.js" /> , 'angularNumberPicker', 'enumFlag', 'ngMessages', "ngSanitize", "isteven-multi-select", 'ckeditor'


nezaci.ng = {
    app: {
        services: {}
		, controllers: {}
    }
    , controllerInstances: []
	, exceptions: {}
	, examples: {}
    , defaultDependencies: ["ngAnimate", "ngRoute", "ui.bootstrap", "ngSanitize"]
    , getModuleDependencies: function () {
        if (nezaci.extraNgDependencies) {
            var newItems = nezaci.ng.defaultDependencies.concat(nezaci.extraNgDependencies);
            return newItems;
        }
        return nezaci.ng.defaultDependencies;
    }
};

nezaci.ng.app.module = angular.module('nezaciApp', nezaci.ng.getModuleDependencies());

nezaci.ng.app.module.filter('encodeUri', function ($window) {

    //function newEncode(data) {
    //    return $window.encodeURIComponent(data).replace(/%20/g, '+');
    //}

    //return newEncode;
    return $window.encodeURIComponent;
});

nezaci.ng.app.module.filter('htmlToPlaintext', function ($window) {
    return function (text) {
        return text ? String(text).replace(/<[^>]+>/gm, '') : '';
    };
});



nezaci.ng.app.module.value('$nezaci', nezaci.page);

nezaci.ng.app.module.factory('$daysEnum', [
    function () {
        return Object.freeze({
            SUNDAY: 1,
            MONDAY: 2,
            TUESDAY: 4,
            WEDNESDAY: 8,
            THURSDAY: 16,
            FRIDAY: 32,
            SATURDAY: 64
        });
    }
]);

nezaci.ng.app.module.factory('$eventsEnum', [
    function () {
        return Object.freeze({
            HEADER_DATA: 'header_data'
        });
    }
]);

//#region - base functions and objects -

nezaci.ng.exceptions.argumentException = function (msg) {
    this.message = msg;
    var err = new Error();


    console.error(msg + "\n" + err.stack);
}

nezaci.ng.app.services.baseEventServiceFactory = function ($rootScope) {

    var factory = this;

    factory.$rootScope = $rootScope;

    factory.eventService = function (eventName) {

        var thisEventService = this;

        thisEventService.eventName = eventName;

        thisEventService.broadcast = function () {
            factory.$rootScope.$broadcast(thisEventService.eventName, arguments)
        }

        thisEventService.emit = function () {
            factory.$rootScope.$emit(thisEventService.eventName, arguments)
        }

        thisEventService.listen = function (callback) {
            factory.$rootScope.$on(thisEventService.eventName, callback)
        }

    }

    return factory.eventService;
}


nezaci.ng.app.services.baseService = function ($win, $loc, $util) {
    /*
        when this function is envoked by Angular, Angular wants an instance of the Service object. 
		
    */

    var getChangeNotifier = function ($scope) {
        /*
        will be called when there is an event outside Angular that has modified
        our data and we need to let Angular know about it.
        */
        var self = this;

        self.scope = $scope;

        return function (fx) {
            self.scope.$apply(fx);//this is the magic right here that cause ng to re-evaluate bindings
        }


    }

    var baseService = {
        $window: $win
        , getNotifier: getChangeNotifier
        , $location: $loc
        , $utils: $util
        , merge: $.extend
    };

    return baseService;
}

nezaci.ng.app.controllers.baseController = function ($doc, $logger, $sab, $route, $routeParams, $eventHandlerService, $eventsEnum, $location, $alertService) {
    /*
        this is intended to serve as the base controller
    */

    baseControler = {
        $document: $doc
		, $log: $logger
        , $nezaci: $sab
        , $location: $location
        , merge: $.extend
        , $eventHandlerService: $eventHandlerService
        , $alertService: $alertService
        , $eventsEnum: $eventsEnum
        , setUpCurrentRequest: function (viewModel) {

            viewModel.currentRequest = { originalPath: "/", isTop: true };

            if (viewModel.$route.current) {
                viewModel.currentRequest = viewModel.$route.current;
                viewModel.currentRequest.locals = {};
                viewModel.currentRequest.isTop = false;
            }

            viewModel.$log.log("setUpCurrentRequest firing:");
            viewModel.$log.debug(viewModel.currentRequest);

        }

    };

    return baseControler;
}

//#endregion

//#region  - core ng wrapper functions --

nezaci.ng.getControllerInstance = function (jQueryObj) {///used to grab an instance of a controller bound to an Element
    console.log(jQueryObj);
    return angular.element(jQueryObj[0]).controller();
}

nezaci.ng.addService = function (ngModule, serviceName, dependencies, factory) {
    /*
    nezaci.ng.app.module.service(
    '$baseService', 
    ['$window', '$location', '$utils', nezaci.ng.app.services.baseService]);
    */
    if (!ngModule ||
		!serviceName || !factory ||
		!angular.isFunction(factory)) {
        throw new nezaci.ng.exceptions.argumentException("Invalid Service Call");
    }

    if (dependencies && !angular.isArray(dependencies)) {
        throw new nezaci.ng.exceptions.argumentException("Invalid Service Call [dependencies]");
    }
    else if (!dependencies) {
        dependencies = [];
    }

    dependencies.push(factory);

    ngModule.service(serviceName, dependencies);

}

nezaci.ng.registerService = nezaci.ng.addService;

nezaci.ng.addController = function (ngModule, controllerName, dependencies, factory) {
    if (!ngModule ||
		!controllerName || !factory ||
		!angular.isFunction(factory)) {
        throw new nezaci.ng.exceptions.argumentException("Invalid Service defined");
    }

    if (dependencies && !angular.isArray(dependencies)) {
        throw new nezaci.ng.exceptions.argumentException("Invalid Service Call [dependencies]");
    }
    else if (!dependencies) {
        dependencies = [];
    }

    dependencies.push(factory);
    ngModule.controller(controllerName, dependencies);

}

nezaci.ng.registerController = nezaci.ng.addController;


//#endregions


//#region - adding in baseService and baseController

/*
the basic explaination for these three function arguments

name of thing we are creating:'baseService'
array containing the dependancies of the function we will use to create said thing
the last item in this array is invoked to create the object and passed the preceding dependancies.


*/
nezaci.ng.addService(nezaci.ng.app.module
					, "$baseService"
					, ['$window', '$location']
					, nezaci.ng.app.services.baseService);

nezaci.ng.addService(nezaci.ng.app.module
					, "$eventServiceFactory"
					, ["$rootScope"]
					, nezaci.ng.app.services.baseEventServiceFactory);

nezaci.ng.addService(nezaci.ng.app.module
					, "$baseController"
					, ['$document', '$log', '$nezaci', "$route", "$routeParams", "$eventHandlerService", "$eventsEnum", "$location", "$alertService"]
					, nezaci.ng.app.controllers.baseController);


//#endregion

//#region - Examples on how to use the core functions

//***************************************************************************************
//------------------------ Examples -------------------------------------
/*
 * 
 *              How to call the .addService and .addController
 * 
 */




nezaci.ng.examples.exampleServices = function ($baseService) {
    /*
		when this function is envoked by Angular,
		Angular wants an instance of the Service object. here
		we reuse the same instance of the JS object we have above
	*/

    /*
		we are using this as an example to demonstrate we can use our existing
		code with this new pattern.
	*/

    var anezaciServiceObject = nezaci.services.users;
    var newService = angular.merge(true, {}, anezaciServiceObject, baseService);

    return newService;
}

nezaci.ng.examples.exampleController = function ($scope, $baseController, $exampleSvc) {

    var vm = this;
    vm.items = null;
    vm.receiveItems = _receiveItems;
    vm.testTitle = "Get this to show first";

    //-- this line to simulate inheritance
    $baseController.merge(vm, $baseController);

    //You first pass at creating a new controller end here. 
    //go make this work first
    //-----------------------

    //this is a wrapper for our small dependency on $scope
    vm.notify = $exampleSvc.getNotifier($scope);

    function _receiveItems(data) {
        //this receives the data and calls the special
        //notify method that will trigger ng to refresh UI
        vm.notify(function () {
            vm.items = data.items;
        });
    }
}


nezaci.ng.addService(nezaci.ng.app.module
					, "$exampleSvc"
					, ['$baseService']
					, nezaci.ng.examples.exampleServices);

nezaci.ng.addController(nezaci.ng.app.module
	, 'controllerName'
	, ['$scope', '$baseController', '$exampleSvc']
	, nezaci.ng.examples.exampleController
	);


//------------------------ Examples -------------------------------------
//***************************************************************************************


//#endregion
