
console = console || {
    log: function () {
    }
};

var constants = {
    paths: {
        root: "/",
        createNew: "/new",
        edit: "/edit/:inventoryItemId"
    }
};

angular.module('cqrsSample', []).
  value('cqrsUrl', 'http://localhost:34543/api/InventoryItem').
  factory('InventoryItems', function (cqrsUrl) {
      return allInventoryItems(cqrsUrl);
  }).
  config(function ($routeProvider) {
      $routeProvider.
        when(constants.paths.root, { controller: ListCtrl, templateUrl: 'templates/list.html' }).
        when(constants.paths.createNew, { controller: CreateCtrl, templateUrl: 'templates/detail.html' }).
        when(constants.paths.edit, { controller: EditCtrl, templateUrl: 'templates/detail.html' }).
        otherwise({ redirectTo: constants.paths.root });
  });

function loadData(cqrsUrl, inventoryItems) {

    $.ajax({ url: cqrsUrl, async: false }).done(function (data, xhr) {
        for (var i = 0; i < data.length; i++) {
            inventoryItems.push(new InventoryItem(data[i]));
        }
    });

}

function allInventoryItems(cqrsUrl) {

    var inventoryItems = [];
    loadData(cqrsUrl, inventoryItems);


    inventoryItems.add = function ($scope, afterDoneCallback) {

        var command = new CreateInventoryItem($scope.name);
        $.ajax({ url: cqrsUrl, type: "POST", data: command, dataType: "json", async: false })
            .done(function (data, xhr) {
                inventoryItems.splice(0, inventoryItems.length); // clear
                loadData(cqrsUrl, inventoryItems);
            });

        if (afterDoneCallback)
            afterDoneCallback();
    };

    return inventoryItems;
}

function InventoryItem(data) {

    if (typeof data === "string") {
        this.name = data;
        this.count = 0;
    } else {
        $.extend(this, data);
    }

}


function CreateInventoryItem(name) {
    this.name = name;
}

/*
function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
               .toString(16)
               .substring(1);
};

function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
           s4() + '-' + s4() + s4() + s4();
}
*/

function ListCtrl($scope, InventoryItems) {
    $scope.inventoryItems = InventoryItems;
}

function CreateCtrl($scope, $location, $timeout, InventoryItems) {
    $scope.save = function () {
        InventoryItems.add($scope.inventoryItem, function () {
            $timeout(function () { $location.path(constants.paths.root); });
        });
    };
}

function EditCtrl($scope, $location, $routeParams, angularFire, fbURL) {
    angularFire(cqrsUrl + $routeParams.inventoryItemId, $scope, 'remote', {}).
    then(function () {
        $scope.inventoryItem = angular.copy($scope.remote);
        $scope.inventoryItem.$id = $routeParams.inventoryItemId;
        $scope.isClean = function () {
            return angular.equals($scope.remote, $scope.project);
        };

        $scope.destroy = function () {
            $scope.remote = null;
            $location.path(constants.paths.root);
        };

        $scope.save = function () {
            $scope.remote = angular.copy($scope.project);
            $location.path(constants.paths.root);
        };
    });
}