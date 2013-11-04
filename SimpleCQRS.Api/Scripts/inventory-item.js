
console = console || {
    log: function () {
    }
};

var constants = {
    paths: {
        root: "/",
        createNew: "/new",
        edit: "/edit/:inventoryItemId"
    },
    concurrencyVersionName: "$concurrencyVersion$"
};

angular.module('cqrsSample', []).
    value('cqrsUrl', 'http://localhost:34543/api/InventoryItem').
    /*
    factory('InventoryItems', function (cqrsUrl) {
        return allInventoryItems(cqrsUrl);
    }).
    */
    config(function($routeProvider) {
        $routeProvider.
            when(constants.paths.root, { controller: ListCtrl, templateUrl: 'templates/list.html' }).
            when(constants.paths.createNew, { controller: CreateCtrl, templateUrl: 'templates/detail.html' }).
            when(constants.paths.edit, { controller: EditCtrl, templateUrl: 'templates/detail.html' }).
            otherwise({ redirectTo: constants.paths.root });
    })
    .config(function ($provide) {
        $provide.decorator('$http', function ($delegate) {

            var customHttp = function (config) {
                
                if (config && (config.method === "PUT" || config.method === "POST" || config.method === "DELETE")
                    && config.data && typeof config.data === "object") {
                    
                    config.headers = config.headers || {};

                    config.headers["Content-Type"] = "application/json;domain-model=" + config.data.constructor.name;
                    if (config.method === "PUT" && config.$scope && config.$scope[constants.concurrencyVersionName]) {
                        config.headers["If-Match"] = config.$scope[constants.concurrencyVersionName];
                    }
                                            
                }

                return $delegate(config);

            };

            angular.extend(customHttp, $delegate);
            return customHttp;
        });
    });

function InventoryItem(data) {

    if (typeof data === "string") {
        this.name = data;
        this.count = 0;
    } else {
        $.extend(this, data);
    }

}


function CreateInventoryItemCommand(name) {
    this.name = name;
}

// does not need any properties
function DeactivateInventoryItemCommand() {

}

function RenameInventoryItemCommnad(newName) {
    this.newName = newName;
}

function CheckInItemsToInventoryCommand(count) {
    this.count = count;
}

function RemoveItemsFromInventoryCommand(count) {
    this.count = count;
}


function ListCtrl($scope, $http, cqrsUrl) {
    $http({
        url: cqrsUrl,
        method: "GET"
    })
    .then(function(data) {
        $scope.inventoryItems = data.data;
    });
    
}

function CreateCtrl($scope, $location, $timeout, $http, cqrsUrl) {
    
    $scope.createRenameLabel = "Create";
    $scope.save = function () {

        $http({
            method: "POST",
            url: cqrsUrl,
            data: new CreateInventoryItemCommand($scope.inventoryItem.name)
        })
        .then(function() {
            $location.path(constants.paths.root);
        });

    };
}

function EditCtrl($scope, $location, $http, $routeParams, cqrsUrl) {

    $http.get(cqrsUrl + "/" + $routeParams.inventoryItemId)   
        .then(function (data) {
           
            $scope.inventoryItem = angular.copy(data.data);
            $scope.changeCount = 0;
            $scope.inventoryItem.$id = $routeParams.inventoryItemId;
            $scope[constants.concurrencyVersionName] = data.headers("etag");
            $scope.createRenameLabel = "Rename";
            $scope.isClean = function () {
                return angular.equals($scope.remote, $scope.inventoryItem);
            };

            $scope.deactivate = function () {
                $scope.remote = null;
                $http({
                    method: "DELETE",
                    data: new DeactivateInventoryItemCommand(),
                    url: cqrsUrl + "/" + $routeParams.inventoryItemId,
                    $scope: $scope
                })
                    .then(function() {
                        $location.path(constants.paths.root);
                    });

            };

            $scope.save = function () {
                
                $http({
                    method: "PUT",
                    data: new RenameInventoryItemCommnad($scope.inventoryItem.name),
                    url: cqrsUrl + "/" + $routeParams.inventoryItemId,
                    $scope: $scope
                })
                   .then(function () {
                       $location.path(constants.paths.root);
                   });

            };

            $scope.checkin = function() {
                $http({
                    method: "POST",
                    data: new CheckInItemsToInventoryCommand($scope.changeCount),
                    url: cqrsUrl + "/" + $routeParams.inventoryItemId
                })
                  .then(function () {
                      $location.path(constants.paths.root);
                  });
            };
            
            $scope.checkout = function () {
                $http({
                    method: "POST",
                    data: new RemoveItemsFromInventoryCommand($scope.changeCount),
                    url: cqrsUrl + "/" + $routeParams.inventoryItemId
                })
                  .then(function () {
                      $location.path(constants.paths.root);
                  });
            };

        },
            function errorHandler() {
            // TODO     
        });
    }