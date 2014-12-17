flowTasksApp
    .config([
        '$stateProvider', '$urlRouterProvider', function($stateProvider, $urlRouterProvider) {
            $stateProvider.state({
                name: 'default',
                url: '',
                templateUrl: 'scripts/flowTasksApp/views/Home/main.html',
                authorize: true,
                title: 'FlowTasks - Home'
            });

            $stateProvider.state({
                name: 'home',
                url: '/',
                templateUrl: 'scripts/flowTasksApp/views/Home/main.html',
                authorize: true,
                title: 'FlowTasks - Home'
            });

            $stateProvider.state({
                name: 'signin',
                url: '/signin',
                title: 'FlowTasks - Signin',
                templateUrl: 'scripts/flowTasksApp/views/Login/index.html'
            });

            $stateProvider.state({
                name: 'userprofile',
                url: '/userprofile',
                templateUrl: 'scripts/flowTasksApp/views/User/index.html',
                authorize: true,
                title: 'FlowTasks - Profile'
            });

            $stateProvider.state({
                name: 'userinfo',
                url: '/userinfo/{username}',
                templateUrl: 'scripts/flowTasksApp/views/User/info.html',
                authorize: true,
                title: 'FlowTasks - Profile'
            });

            $stateProvider.state({
                name: 'task',
                url: '/tasks?toid',
                templateUrl: 'scripts/flowTasksApp/views/Task/list.html',
                authorize: true,
                title: 'FlowTasks - Task'
            });

            $stateProvider.state({
                name: 'dashboard',
                url: '/dashboard',
                templateUrl: 'scripts/flowTasksApp/views/Dashboard/index.html',
                authorize: true,
                title: 'FlowTasks - Dashboard'
            });

            $stateProvider.state({
                name: 'holidays',
                url: '/holidays',
                templateUrl: 'scripts/flowTasksApp/views/Holiday/index.html',
                authorize: true,
                title: 'FlowTasks - Holiday'
            });

            $stateProvider.state({
                name: 'sketch',
                url: '/sketch?workflow',
                templateUrl: 'scripts/flowTasksApp/views/Sketch/index.html',
                authorize: true,
                title: 'FlowTasks - Sketch'
            });

            $stateProvider.state({
                name: 'report',
                url: '/report',
                templateUrl: 'scripts/flowTasksApp/views/Report/index.html',
                authorize: true,
                title: 'FlowTasks - Report'
            });

            $urlRouterProvider.otherwise("/");
        }
    ]);