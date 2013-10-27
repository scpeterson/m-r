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
    console.log("started!");
    $.ajax({ url: cqrsUrl, async : false }).done(function() {
        console.log("done!");
    });
    console.log("last!");
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