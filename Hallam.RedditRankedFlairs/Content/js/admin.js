(function (app) {
    app.factory('$ajax', function ($http) {
        return {
            get: function (uri, data, callback) {
                var config;
                if (callback === undefined) {
                    callback = data;
                    data = undefined;
                }
                if (data) {
                    config = {
                        params: data
                    };
                }
                var request = $http.get(uri, config);
                request.then(function (r) {
                    callback(true, r.data, r.status, r.statusText);
                }, function (r) {
                    callback(false, r.data, r.status, r.statusText);
                });
            },
            post: function (uri, data, callback) {
                if (callback === undefined) {
                    callback = data;
                    data = undefined;
                }
                var request = $http.post(uri, data);
                request.then(function (r) {
                    callback(true, r.data, r.status, r.statusText);
                }, function (r) {
                    callback(true, r.data, r.status, r.statusText);
                })
            }
        };
    });

    app.factory('$notify', function () {
        var handlers = [];
        function Notify(handler) {
            handlers.push(handler);
        }
        Notify.alert = function (message) {
            for (var i = 0; i < handlers.length; i += 1) {
                handlers[i]('alert', message);
            }
        };
        Notify.error = function (message) {
            for (var i = 0; i < handlers.length; i += 1) {
                handlers[i]('error', message);
            }
        };
        Notify.success = function (message) {
            for (var i = 0; i < handlers.length; i += 1) {
                handlers[i]('success', message);
            }
        };
        Notify.waiting = function (message) {
            for (var i = 0; i < handlers.length; i += 1) {
                handlers[i]('waiting', message);
            }
        };
        return Notify;
    });

    app.factory('$panels', function () {
        function Panel(name, title) {
            this.name = name;
            this.title = title;
            this.handlers = { open: [], close: [] };
        }
        Panel.prototype = {
            closing: function (handler) {
                this.handlers.close.push(handler);
            },
            opening: function (handler) {
                this.handlers.open.push(handler);
            },
            trigger: function (event) {
                if (this.handlers[event]) {
                    for (var i = 0; i < this.handlers[event].length; i += 1) {
                        this.handlers[event][i].call(this);
                    }
                }
            }
        };
        return function (items) {
            var r = {};
            for (var key in items) {
                if (items.hasOwnProperty(key)) {
                    r[key] = new Panel(key, items[key]);
                }
            }
            return r;
        };
    });

    app.controller('MainController', function ($scope, $panels) {
        $scope.panels = $panels({
            'eventlog': 'Event Log',
            'subreddits': 'Manage Subscriptions'
        });

        $scope.changePanel = function (name) {
            if ($scope.panel) {
                $scope.panel.trigger('close');
            }
            $scope.panel = $scope.panels[name];
            $scope.panel.trigger('open');
        };
    });

    app.controller('NotifyController', function ($scope, $notify) {
        $scope.message = null;
        $scope.error = null;
        $scope.success = null;

        $notify(function (type, msg) {
            $scope.message = msg;
            $scope.alert = type == 'alert';
            $scope.error = type == 'error';
            $scope.success = type == 'success';
            $scope.waiting = type == 'waiting';
        });

        $scope.close = function () {
            $scope.error = null;
            $scope.success = null;
            $scope.message = null;
        };
    });

    app.controller('SubscriptionsController', function ($scope, $notify, $ajax) {
        $scope.panels.subreddits.opening(function () {
            $scope.loading = true;
            $scope.subscriptions = [];
            $ajax.get('/adminPanel/api/subscriptions',
                function (success, data, status, statusText) {
                    $scope.loading = false;
                    if (!success) { $notify.error('Error loading panel: ' + statusText); }
                    else { $scope.subscriptions = data.result; }
                });
        });

        $scope.autoDetect = {
            busy: false,
            go: function () {
                $notify.waiting('Detecting subscriptions...');
                $scope.autoDetect.busy = true;
                $ajax.post('/adminPanel/api/subscriptions?refresh=true',
                    function (success, data, status, statusText) {
                        if (!success) { $notify.error('Error detecting subscriptions:' + data.error); }

                    });
            }
        };
    });
})(angular.module('Admin', []));