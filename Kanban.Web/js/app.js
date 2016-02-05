// The Angular Module
angular.module('kanban', ['ngResource']);

angular.module('kanban').value('apiUrl', 'http://localhost:64124/api');
// The Controller
angular.module('kanban').controller('IndexController', function ($scope, $resource, apiUrl) {

    var ListResource = $resource(apiUrl + '/lists/:listId', { listId: '@id' },
        {
            'cards': {
                url: apiUrl + '/lists/:listId/cards',
                method: 'GET',
                isArray: true
            }
        });
    

    function activate() {
        ListResource.query(function (data) {
            $scope.lists = data;
            $scope.lists.forEach(function (list) {
                list.cards = ListResource.cards({ listId: list.ListId });
            });
        });
    }

    $scope.newList = {};

    $scope.addList = function () {
        ListResource.save($scope.newList, function () {
            alert('New list added!');
            activate();
        });
    };

    $scope.deleteList = function (list) {
        $scope.currentList = list;
        ListResource.remove($scope.currentList, function () {
            alert('List deleted!');
            activate();
        });
    };

    activate();
});