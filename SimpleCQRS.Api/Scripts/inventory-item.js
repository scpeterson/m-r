console = console || {
    log: function() {
    }
};


angular.module('cqrsSample', []).
  value('cqrsUrl', 'http://localhost:34543/api/InventoryItem').
  factory('InventoryItems', function (cqrsUrl) {
      return allInventoryItems(cqrsUrl);
  }).
  config(function ($routeProvider) {
      $routeProvider.
        when('/', { controller: ListCtrl, templateUrl: 'templates/list.html' }).
        when('/new', { controller: CreateCtrl, templateUrl: 'templates/detail.html' }).
        when('/edit/:inventoryItemId', { controller: EditCtrl, templateUrl: 'templates/detail.html' }).
        otherwise({ redirectTo: '/' });
  });

function allInventoryItems(cqrsUrl) {

    var inventoryItems = [];

    console.log("started!");
    $.ajax({ url: cqrsUrl, async : false }).done(function(data, xhr) {
        for (var i = 0; i < data.length; i++) {
            inventoryItems.push(new InventoryItem(data[i]));
        }
    });
    console.log("last!");

    inventoryItems.add = function ($scope) {
        
        var command = new CreateInventoryItem($scope.name);

        $.ajax({ url: cqrsUrl, type: "POST", data: command, dataType: "json" });

    };

    return inventoryItems;
}

function InventoryItem(data) {
    
    if (typeof data === "string") {
        this.name = data;
        this.id = guid();
        this.count = 0;
    } else {
        // $.extend(this, data);  
        this.name = data.Name;
        this.id = data.Id;
        this.count = data.Count;
    }  
    
}

function CreateInventoryItem(name) {
    this.name = name;
    this.id = guid();
}


function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
               .toString(16)
               .substring(1);
};

function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
           s4() + '-' + s4() + s4() + s4();
}

function ListCtrl($scope, InventoryItems) {
    $scope.inventoryItems = InventoryItems;
}

function CreateCtrl($scope, $location, $timeout, InventoryItems) {
    $scope.save = function() {
        InventoryItems.add($scope.inventoryItem, function () {
            $timeout(function() { $location.path('/'); });
        });
    };
}

function EditCtrl($scope, $location, $routeParams, angularFire, fbURL) {
    angularFire(cqrsUrl + $routeParams.inventoryItemId, $scope, 'remote', {}).
    then(function () {
        $scope.inventoryItem = angular.copy($scope.remote);
        $scope.inventoryItem.$id = $routeParams.inventoryItemId;
        $scope.isClean = function() {
            return angular.equals($scope.remote, $scope.project);
        };
        
        $scope.destroy = function () {
            $scope.remote = null;
            $location.path('/');
        };
        
        $scope.save = function () {
            $scope.remote = angular.copy($scope.project);
            $location.path('/');
        };
    });
}