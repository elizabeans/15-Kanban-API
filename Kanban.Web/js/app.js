// The Angular Module
angular.module('kanban', ['ngResource']);

angular.module('kanban').value('apiUrl', 'http://localhost:64124/api');
// The Controller
angular.module('kanban').controller('IndexController', function ($scope, $resource, apiUrl) {

    var ListResource = $resource(apiUrl + '/lists/:ListId', { ListId: '@ListId' });
    
    var CardsResource = $resource(apiUrl + '/cards/:CardId', { CardId: '@CardId' },
        {
            'cards': {
                method: 'GET',
                url: apiUrl + '/lists/:ListId/cards',
                isArray: true
            }
        });

    $scope.data = {
        newList: {},
        newCard: {}
    }

    function activate() {
        ListResource.query(function (data) {
            $scope.data.lists = data;

            $scope.data.lists.forEach(function (list) {
                list.cards = CardsResource.cards({ ListId: list.ListId });
            });
        });
    }

    $scope.addList = function () {
        ListResource.save($scope.newList, function () {
            alert('New list added!');
            activate();
        });
    };

    $scope.addCard = function (list) {
        list.newCard.ListId = list.ListId;
        CardsResource.save(list.newCard, function (data) {
            activate();
        });
    };

    $scope.removeList = function (list) {
        list.$remove(function (data) {
            activate();
        });
    };

    $scope.removeCard = function (card) {
        card.$remove(function (data) {
            activate();
        });
    };

    activate();
});