(function (app) {
    app.controller('SubscriptionController', function ($scope, ajax, notify, panels) {
        $scope.subscriptions = [];
        panels('subscriptions').register('opening', function () {
            notify.waiting('Loading subscriptions...');
            $scope.busy = true;
            ajax.get('/adminPanel/api/subscriptions', function (success, data) {
                $scope.busy = false;
                if (!success) {
                    notify.error(data.error);
                    return;
                }
                $scope.subscriptions = data.result;
                notify.clear();
            });
        });
    });

    app.controller('SubscriptionRegisterController', function ($scope, ajax, modal, notify) {
        var dialog = modal('#subscribe-dialog');
        $scope.dialog = dialog;
        $scope.busy = false;

        function moderatorOfHandler(success, data, status, statusText) {
            if (!success) {
                notify.error(data.error || statusText);
                $scope.busy = false;
                return;
            }

            var items = data.result.map(function (a) { return a.toLowerCase(); });
            items = items.filter(function (a) { return $scope.subscriptions.indexOf(a) == -1; });

            if (items.length === 0) {
                notify.alert('No Sub Reddits detected.');
                return;
            }

            dialog.items = items;
            notify.clear();
            dialog.show();
            $scope.busy = false;
        }

        $scope.execute = function () {
            $scope.busy = true;
            notify.waiting('Detecting Sub Reddits...');
            ajax.get('/adminPanel/api/moderatorOf', moderatorOfHandler);
        };

        $scope.subscribe = function (name) {
            $scope.busy = true;
            notify.waiting('Linking to /r/' + name);
            ajax.post('/adminPanel/api/subscribe', { name: name },
                function (success, data) {
                    if (!success) {
                        notify.error(data.error);
                        $scope.busy = false;
                        return;
                    }
                    $scope.subscriptions.push(name);
                    dialog.items = dialog.items.filter(function (a) { return a != name; });
                    $scope.busy = false;
                    notify.success('Linked Sub Reddit. Flairs will push shortly.');
                });
        };
    });
})(angular.module('Admin'));