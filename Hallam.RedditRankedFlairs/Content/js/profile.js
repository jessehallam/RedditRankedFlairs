(function (app) {
    app.factory('summoners', function (ajax) {
        return {
            items: [],
            update: function () {
                var $this = this;
                $this.loading = true;
                ajax.get('/profile/api/summoners', function (success, data) {
                    $this.loading = null;
                    $this.items = data.result;
                });
            }
        };
    });

    app.controller('MainController', function ($scope, summoners) {
        $scope.summoners = summoners;
        summoners.update();
    });

    app.controller('NotifyController', function ($scope) {

    });

    app.controller('RegisterController', function ($scope, $timeout, ajax, modal) {
        var dialog = modal('#register-modal');
        $scope.busy = false;

        dialog.register('closing', function () {
            $scope.code = undefined;
            $scope.status = undefined;
        });

        function getValidationCode() {
            var data = {
                summonerName: $scope.summonerName,
                region: $scope.region
            };
            ajax.post('/profile/api/register', data,
                function (success, data) {
                    $scope.busy = false;
                    if (!success) {
                        $scope.status = { error: true, message: data.error };
                        return;
                    }
                    $scope.code = data.result.code;
                });
        }

        function checkValidationStatus(attempts) {
            var data = {
                summonerName: $scope.summonerName,
                region: $scope.region
            };
            ajax.post('/profile/api/validate', data,
                function (success, data, status) {
                    if (!success) {
                        if (--attempts >= 0) {
                            $timeout(function () { checkValidationStatus(attempts); }, 10000);
                        }
                        if (status == 417) {
                            $scope.status = { message: 'Unable to validate summoner.' };
                            $timeout(function () { $scope.busy = false; }, 1000);
                        }
                        else {
                            $scope.status = { error: true, message: data.error };
                        }
                    }
                    else {
                        $scope.status = undefined;
                        $scope.code = undefined;
                        $scope.summoner = undefined;
                        $scope.region = undefined;
                        dialog.hide();
                        $scope.summoners.update();
                    }
                });
        }

        function validateSummoner() {
            $scope.status = { message: 'This could take a minute...' };
            checkValidationStatus(1);
        }

        $scope.close = function () { dialog.hide(); };
        $scope.confirm = function () {
            $scope.busy = true;

            if ($scope.code) {
                validateSummoner();
            }
            else {
                getValidationCode();
            }
        };
    });
    app.controller('ValidationController', function () { });
})(angular.module('Profile'));