(function (app) {
    app.directive('ngTooltip', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var tooltip = attrs.ngTooltip;
                var match;
                var placement;
                if (!tooltip) {
                    return;
                }
                if ((match = /^(top|right|bottom|left),/.exec(tooltip))) {
                    placement = match[1];
                    tooltip = tooltip.substr(placement.length + 1);
                }
                element.tooltip({
                    'placement': placement,
                    'title': tooltip
                });
            }
        };
    });

    app.factory('ajax', function ($http) {
        function wrap(promise, handler) {
            function interceptHandler(response) {
                if (handler) {
                    var success = response.status >= 200 && response.status <= 299;
                    handler(success, response.data, response.status, response.statusText);
                }
                if (response.data && response.data.exception) {
                    console.log(response.data.exception);
                }
            }
            promise.then(interceptHandler, interceptHandler);
        }
        return {
            get: function (uri, data, handler) {
                if (handler === undefined) {
                    handler = data;
                    data = undefined;
                }
                if (data) {
                    data = { config: { params: data } };
                }
                return wrap($http.get(uri, data), handler);
            },
            post: function (uri, data, handler) {
                if (handler === undefined) {
                    handler = data;
                    data = undefined;
                }
                return wrap($http.post(uri, data), handler);
            }
        };
    });

    app.factory('modal', function ($rootScope) {
        function Modal(id) {
            this.element = $(id);
            this.handlers = {};
        }
        Modal.prototype = {
            hide: function () {
                this.element.removeClass('modal-show');
            },
            register: function (eventName, handler) {
                if (this.handlers[eventName] === undefined) {
                    this.handlers[eventName] = [];
                }
                this.handlers[eventName].push(handler);
            },
            show: function () {
                this.element.addClass('modal-show');
            },
            trigger: function (eventName) {
                if (this.handlers[eventName]) {
                    var args = Array.prototype.splice(arguments, 1);
                    this.handlers[eventName].apply(this, args);
                }
            }
        };
        return function (id) {
            return new Modal(id);
        };
    });

    app.factory('notify', function () {
        var handlers = [];
        function invoke() {
            var args = Array.prototype.splice.call(arguments, 0);
            for (var i = 0; i < handlers.length; i += 1) {
                handlers[i].apply(this, args);
            }
        }
        return {
            register: function (handler) { handlers.push(handler); },
            alert: function (m) { invoke.apply(this, ['alert', m]); },
            clear: function (m) { invoke.apply(this, []); },
            error: function (m) { invoke.apply(this, ['error', m]); },
            success: function (m) { invoke.apply(this, ['success', m]); },
            waiting: function (m) { invoke.apply(this, ['waiting', m]); }
        };
    });

    app.factory('panels', function ($rootScope, $location) {
        function Panel(name) {
            this.name = name;
            this.handlers = {};
        }
        Panel.prototype = {
            register: function (eventName, handler) {
                if (this.handlers[eventName] === undefined) {
                    this.handlers[eventName] = [];
                }
                this.handlers[eventName].push(handler);
            },
            trigger: function (eventName) {
                if (this.handlers[eventName]) {
                    var args = Array.prototype.splice.apply(arguments, [1]);
                    for (var i = 0; i < this.handlers[eventName].length; i += 1) {
                        var handler = this.handlers[eventName][i];
                        handler.apply(this, args);
                    }
                }
            }
        };

        var panels = {};
        return function (panelName) {
            if (panels[panelName] === undefined) {
                panels[panelName] = new Panel(panelName);
            }
            return panels[panelName];
        };
    });

    app.controller('MainController', function ($rootScope, $scope, $location, panels) {
        $scope.panels = panels;
        $scope.panel = null;
        $scope.changePanel = function (panelName) {
            if ($scope.panel) {
                $scope.panel.trigger('closing');
            }
            $scope.panel = panels(panelName);
            if ($scope.panel) {
                $scope.panel.trigger('opening');
            }
        };
        $rootScope.$on('$locationChangeSuccess', function () {
            var path = $location.path();
            if (path && typeof path == 'string' && path.charAt(0) === '/') {
                var name = path.substr(1);
                $scope.changePanel(name);
            }
        });
    });

    app.controller('NotifyController', function ($scope, notify) {
        $scope.message = null;
        $scope.type = null;

        notify.register(function (type, msg) {
            $scope.message = msg;
            $scope.type = type;
        });
    });
})(angular.module('Admin', []));