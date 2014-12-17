
Useful info for testing JS
--------------------------

http://www.benlesh.com/2013/06/angular-js-unit-testing-services.html
http://www.benlesh.com/2013/05/angularjs-unit-testing-controllers.html
http://odetocode.com/blogs/scott/archive/2014/05/15/a-few-thoughts-on-better-unit-tests-for-angularjs-controllers.aspx

http://busypeoples.github.io/post/testing-angularjs-directives/
http://www.bluesphereinc.com/blog/unit-testing-angularjs-directives


Karma
-----

1. to install karma with proxy
npm config set proxy http://sydproxy.acp.net:8080
npm config set https-proxy http://sydproxy.acp.net:8080

2. Set node path to env. Must be done with a bash cmd
PATH=$PATH:"/c/Program Files (x86)/nodejs/"

3. Launch karma. Must be done with a bash cmd
.node_modules/karma/bin/karma

NOTE: karma is case sensitive!!!!

PATH=$PATH:"/c/Program Files (x86)/nodejs/"
cd Dev/FlowTasks/src/Test/Web/FlowTasksWeb.Test/
node_modules/karma/bin/karma start
