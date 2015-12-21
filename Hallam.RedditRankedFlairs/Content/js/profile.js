(function (app) {

    app.factory('$summoners', function ($resource) {
        return {};
    });

    app.factory('$messages', function () {
        var listeners = [];
        function addListener(listener) {
            listeners.push(listener);
        }
        addListener.error = function (msg) {
            for (var i = 0; i < listeners.length; i++)
                listeners[i]('error', msg);
        };
        addListener.success = function (msg) {
            for (var i = 0; i < listeners.length; i++)
                listeners[i]('success', msg);
        };
        return addListener;
    });

    app.factory('$registration', function () {
        return {
            code: null
        };
    });

    app.controller('MainController', function ($scope, $resource, $summoners) {
        $scope.summoners = $summoners;
    });

    app.controller('NotificationController', function ($scope, $messages) {
        $messages(function (type, msg) {
            $scope.notification = { type: type, msg: msg };
        });
    });

    app.controller('RegisterController', function ($scope, $http, $registration, $timeout) {
        $scope.ok = function () {
            var data = { region: $scope.region, summonerName: $scope.summonerName };
            var request = $http.post('/profile/api/register', data).then(function (resp) { return resp.data; });
            request.then(
                function success(resp) {
                    $registration.code = resp.result.code;
                    $registration.data = data;
                    $scope.busy = false;
                    modal.close();
                    $timeout(function () { modal('#validation-modal'); }, 300);
                },
                function error(resp) {
                    if (resp.status == 400) {
                        $scope.status = { error: resp.validationErrors[0] };
                    }
                    else {
                        $scope.status = { error: resp.error };
                    }
                    $scope.busy = false;
                });
            $scope.busy = true;
            $scope.status = null;
        };
    });

    app.controller('ValidationController', function ($scope, $http, $registration, $timeout) {
        $scope.registration = $registration;
        $scope.ok = function () {
            $scope.busy = true;
            $scope.status = { msg: 'This may take a minute. Please be patient.' };
            $timeout(function () {
                var request = $http.post('/profile/api/validate', $registration.data);
                request = request.then(function (r) { return r.data; });
                request.then(
                    function success(resp) {
                        window.location.reload(true);
                    },
                    function error(resp) {
                        if (resp.status == 417) {
                            $scope.status = { msg: 'Validation was unsuccessful.' };
                        }
                        else if (resp.status == 400) {
                            $scope.status = { error: resp.validationErrors[0] };
                        }
                        else {
                            $scope.status = { error: resp.error };
                        }
                        $scope.busy = false;
                    });
            }, 10000);
        };
    });
})(angular.module('Profile', ['ngResource']));