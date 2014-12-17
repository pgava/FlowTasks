flowTasksApp
    .factory('AuthenticationRsrc', ['$resource', function ($resource) {
        return $resource('/api/auth/:action',
		{ action: "@action" }, {
		});
    }])
    .service('AuthenticationSrv', [,
        function () {
            return {
               
            };
        }
    ]);