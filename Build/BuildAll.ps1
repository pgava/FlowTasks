
function DeployDatabase {
	$curPath = (pwd).path; 		
	$out = $curPath + "\Pack\Publish\SQL\";
	$prj = $Global:baseDir + "\SQL\";	
	
	if (Test-Path $out) {}
	else {
		New-Item $out -itemtype directory
	}
		
	Copy-Item $prj"*.sql" -destination $out
	
}

function CleanOldPack {
	$curPath = (pwd).path;
	$out = $curPath + "\Pack\Publish";
	
	Remove-Item -r -force $out 
}

function DeployWeb {
  param([string[][]]$files)
  
  ForEach($s in $files)
	{	
		$curPath = (pwd).path; 		
		$prj = $Global:baseDir + $s[0];		
		$out = $curPath + "\Pack\Publish\Web\" + $s[1];
		$outBin = $out + "\bin\";
		$configBase = $curPath + "\Pack\Publish\Configs\";
		$configPrj = $configBase + $s[1];
		if (Test-Path $configBase) {}
		else {
			New-Item $configBase -itemtype directory
		}
		if (Test-Path $configPrj) {}
		else {
			New-Item $configPrj -itemtype directory
		}

		MSBuild.exe $prj /t:ResolveReferences /t:_CopyWebApplication /p:VisualStudioVersion=12.0 /p:Configuration=Release /p:WebProjectOutputDir=$out /p:OutDir=$outBin
		
		if ($LastExitCode -ne 0){
			$Global:status = -1
			"NO OK...."
			throw "Error: Failed to compile!"
			break;
		}
		else{
			"OK......"
		}		
		
		Move-Item -path $out"\Web.Release.Config" -destination $configPrj"\Web.Config" -force
		Remove-Item $out"\Web.Config" -force
	}
	
	# Remove configs
	$curPath = (pwd).path; 		
	$out = $curPath + "\Pack\Publish\Web\";		
	Get-ChildItem $out -r -include Web.debug.conf*, packages.conf* | ForEach-Object { Remove-Item $_ -force }
		
}

function DeployConnectors {
  param([string[][]]$files)
 
	ForEach($s in $files)
	{	
		$curPath = (pwd).path; 		
		$prj = $Global:baseDir + $s[0];		
		$out = $curPath + "\Pack\Publish\Connectors\";

		if (Test-Path $out) {}
		else {
			New-Item $out -itemtype directory
		}
		
		$out = $curPath + "\Pack\Publish\Connectors\" + $s[1];		
		if (Test-Path $out) {}
		else {
			New-Item $out -itemtype directory
		}
			
		Get-ChildItem $prj"\Bin\Release" -r -include *.dll, *.exe | % { Copy-Item $_ -destination $out }
		Get-ChildItem $prj *.config | % { Copy-Item $_.FullName -destination $out }
		Get-ChildItem $prj *.bat | % { Copy-Item $_.FullName -destination $out }
		Get-ChildItem $prj README.txt | % { Copy-Item $_.FullName -destination $out }
	}

	# Remove configs
	$curPath = (pwd).path; 		
	$out = $curPath + "\Pack\Publish\Connectors\";		
	Get-ChildItem $out -r -include App.conf*, packages.conf* | % { Remove-Item $_ -force }
		
}

function DeployDocuments {
  param([string[]]$files)
  
	ForEach($s in $files)
	{
		$curPath = (pwd).path; 		
		$prj = $Global:baseDir + $s;		
		$out = $curPath + "\Pack\Publish\Documents\";
		
		if (Test-Path $out) {}
		else {
			New-Item $out -itemtype directory
		}
		
		Get-ChildItem $prj -r -include *.docx | % { Copy-Item $_ -destination $out }
	}
	
}

function DeployPack {
	#WIX 
	$env:Path = $env:Path + ";C:\Program Files (x86)\WiX Toolset v3.8\bin;";
	
	heat.exe dir Pack\Publish -dr FLOWTASKS -ke -srd -cg MyWebWebComponents -var var.publishDir -gg -out Setup\WebSiteContent.wxs
	candle.exe -ext WixSqlExtension -ext WixIISExtension -ext WixUtilExtension -ext WiXNetFxExtension -dpublishDir=Pack\Publish -dMyWebResourceDir=Setup Setup\IisConfiguration.wxs Setup\Product.wxs Setup\WebSiteContent.wxs Setup\UiDialogs.wxs Setup\MyWebUI.wxs
	light.exe -ext WixSqlExtension -ext WixUIExtension -ext WixIISExtension -ext WixUtilExtension -ext WiXNetFxExtension -out Pack\FlowTasks.msi Product.wixobj WebSiteContent.wixobj UiDialogs.wixobj MyWebUI.wixobj IisConfiguration.wixobj
}
  
function BuildPrj {
  param([string[]]$files, [string]$config)

  ForEach($s in $files)
	{
		$prj = $Global:baseDir + $s;		
		MSBuild.exe $prj /t:Clean /t:Build /p:Configuration=$config /p:VisualStudioVersion=12.0
		if ($LastExitCode -ne 0){
			$Global:status = -1
			"NO OK...."
			throw "Error: Failed to compile!"
			break;
		}
		else{
			"OK......"
		}		
	}
}

function RunTest {
  param([string[]]$files)

  $mstest_path = "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe"

  ForEach($s in $files)
	{
		$prj = $Global:baseDir + $s;		
		&$mstest_path /testcontainer:$prj

		if ($LastExitCode -ne 0){
			$Global:status = -1
			"NO OK...."
			throw "Error: Failed to compile!"
			break;
		}
		else{
			"OK......"
		}		
	}
}
 
task default -depends BuildCommon, BuildDal, BuildProxies, BuildServices, BuildWeb, BuildApps, BuildConnectors, BuildDemo, buildTest
task release -depends BuildCommonRel, BuildDalRel, BuildProxiesRel, BuildServicesRel, BuildWebRel, BuildAppsRel, BuildConnectorsRel, BuildDemoRel
task test -depends default, RunAllTest
task pack -depends release, DeployAll

[int]$Global:status = 0
[string]$Global:baseDir = "..\"
[string]$Global:publishDir = $Global:baseDir + "Publish";

[string[]]$Global:Common= @(
"Flow.Library\Flow.Library.csproj",
"Flow.Services\Tasks\Flow.Tasks.Contract\Flow.Tasks.Contract.csproj",
"Flow.Services\Docs\Flow.Docs.Contract\Flow.Docs.Contract.csproj",
"Flow.Services\Users\Flow.Users.Contract\Flow.Users.Contract.csproj"
)
[string[]]$Global:Dal = @(
"Flow.Docs\Flow.Docs.Data\Flow.Docs.Data.csproj",
"Flow.Users\Flow.Users.Data\Flow.Users.Data.csproj",
"Flow.Tasks\Flow.Tasks.Data\Flow.Tasks.Data.csproj"
)
[string[]]$Global:Proxies = @(
"Flow.Services\Tasks\Flow.Tasks.Proxy\Flow.Tasks.Proxy.csproj",
"Flow.Services\Docs\Flow.Docs.Proxy\Flow.Docs.Proxy.csproj",
"Flow.Services\Users\Flow.Users.Proxy\Flow.Users.Proxy.csproj"
)
[string[]]$Global:Services = @(
"Flow.Tasks\Flow.Tasks.Workflow\Flow.Tasks.Workflow.csproj",
"Flow.Docs\Flow.Docs.Process\Flow.Docs.Process.csproj",
"Flow.Services\Tasks\Flow.Tasks.Service\Flow.Tasks.Service.csproj",
"Flow.Services\Docs\Flow.Docs.Service\Flow.Docs.Service.csproj",
"Flow.Services\Users\Flow.Users.Service\Flow.Users.Service.csproj"
)
[string[]]$Global:Web = @(
"Web\Flow.Tasks.View\Flow.Tasks.View.csproj",
"Web\Flow.Tasks.Web\Flow.Tasks.Web.csproj"
)
[string[]]$Global:Apps = @(
"Test\Flow.Workflows.Test\ClientWorkflow\ClientWorkflow.csproj",
"Test\Flow.Workflows.Test\ServiceWorkflowsVB\ServiceWorkflowsVB.vbproj",
"Workflows\ServiceWorkflows\ServiceWorkflows.csproj",
"Sketch\Views\Sketch.Views.csproj",
"Sketch\Workflows\Sketch.Workflows.csproj"
)
[string[]]$Global:Connectors = @(
"Flow.Connectors\Flow.Connectors.DocsOnFolder\DocsOnFolderService.csproj",
"Flow.Connectors\Flow.Connectors.Twitter\Flow.Connectors.Twitter.csproj"
)
[string[]]$Global:Demo = @(
"Demo\DemoFlowTasksView\DemoFlowTasksView.csproj",
"Demo\DemoFlowTasksWorkflow\DemoFlowTasksWorkflow.csproj"
)

[string[]]$Global:Test = @(
"Test\Flow.Docs.Test\Flow.Docs.Test.csproj",
"Test\Connectors\DocsOnFolder\DocsOnFolder.Test.csproj",
"Test\Connectors\Twitter\Twitter.Test.csproj",
"Test\Services\Tasks\TasksService\TasksService.csproj",
"Test\Web\TaskList\TaskList.csproj"
"Test\Flow.Users.Test\Flow.Users.Test.csproj",
"Test\Flow.Tasks.Test\Flow.Tasks.Test.csproj",
"Test\Services\Docs\DocsService\DocsService.csproj"
)

[string[]]$Global:TestDll = @(
"Test\Flow.Docs.Test\bin\Debug\Flow.Docs.Test.dll",
"Test\Connectors\DocsOnFolder\bin\Debug\DocsOnFolder.Test.dll",
"Test\Connectors\Twitter\bin\Debug\Twitter.Test.dll",
"Test\Services\Tasks\TasksService\bin\Debug\TasksService.dll",
"Test\Web\TaskList\bin\Debug\TaskList.dll"
"Test\Flow.Users.Test\bin\Debug\UsersFlowTest.dll",
"Test\Flow.Tasks.Test\bin\Debug\Flow.Tasks.Test.dll",
"Test\Services\Docs\DocsService\bin\Debug\DocsService.dll"
)
 
[string[][]]$Global:DeployWebPrj = @(
@("Web\Flow.Tasks.Web\Flow.Tasks.Web.csproj", "Web"),
@("Flow.Services\Tasks\Flow.Tasks.Service\Flow.Tasks.Service.csproj", "TasksService"),
@("Flow.Services\Docs\Flow.Docs.Service\Flow.Docs.Service.csproj", "DocsService"),
@("Flow.Services\Users\Flow.Users.Service\Flow.Users.Service.csproj", "UsersService"),
@("Workflows\ServiceWorkflows\ServiceWorkflows.csproj", "Workflows")
)

[string[][]]$Global:DeployConnectorsPrj = @(
@("Flow.Connectors\Flow.Connectors.DocsOnFolder", "DocsOnFolder"),
@("Flow.Connectors\Flow.Connectors.Twitter", "Twitter")
)

[string[]]$Global:DeployDocumentation = @(
"Documentation"
)

task BuildCommon {
	BuildPrj $Global:Common 'Debug'
}
task BuildDal {
	BuildPrj $Global:Dal 'Debug'
}
task BuildProxies {
	BuildPrj $Global:Proxies 'Debug'
}
task BuildServices {
	BuildPrj $Global:Services 'Debug'
}
task BuildWeb {
	BuildPrj $Global:Web 'Debug'
}
task BuildApps {
	BuildPrj $Global:Apps 'Debug'
}
task BuildConnectors {
	BuildPrj $Global:Connectors 'Debug'
}
task BuildDemo {
	BuildPrj $Global:Demo 'Debug'
}


task BuildCommonRel {
	BuildPrj $Global:Common 'Release'
}
task BuildDalRel {
	BuildPrj $Global:Dal 'Release'
}
task BuildProxiesRel {
	BuildPrj $Global:Proxies 'Release'
}
task BuildServicesRel {
	BuildPrj $Global:Services 'Release'
}
task BuildWebRel {
	BuildPrj $Global:Web 'Release'
}
task BuildAppsRel {
	BuildPrj $Global:Apps 'Release'
}
task BuildConnectorsRel {
	BuildPrj $Global:Connectors 'Release'
}
task BuildDemoRel {
	BuildPrj $Global:Demo 'Release'
}

task BuildTest {
	BuildPrj $Global:Test 'Debug'
}

task RunAllTest {
	RunTest $Global:TestDll
}

task DeployAll {
	CleanOldPack
	DeployWeb $Global:DeployWebPrj
	DeployConnectors $Global:DeployConnectorsPrj
	DeployDocuments $Global:DeployDocumentation
	DeployDatabase	
	DeployPack
}

task runTest -precondition { return $Global:status -eq 0 } {
	[string]$nunitDir = "`"C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0"

	ForEach($s in $Global:projectsTest)
	{
		$prj = $Global:baseDir + "\" + $s + "\bin\debug\" + $s + ".dll"

		$cmd = "& " + $nunitDir + "\nunit-console.exe`" " + $prj
		Invoke-Expression $cmd
	}

}


