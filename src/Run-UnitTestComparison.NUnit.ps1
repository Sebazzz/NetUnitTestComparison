Param([Switch]$Debug)

function Run($subPath) {
	$assemblyFile = "$subPath\bin\$subPath.dll"
    
    Write-Host "Running $subPath on $assemblyFile"

	if ((Test-Path -Path $assemblyFile) -eq $false) {
		Write-Error -Message "Unable to find '$assemblyFile'. Please build the project first"
		Exit -1
	}

	$packageDir = "packages\NUnit.ConsoleRunner.3.2.1\tools"
	$cmd = "$packageDir\nunit3-console.exe"
	$args = @($assemblyFile, "--noheader", "--verbose", "--full", "--process=InProcess", "--domain=None", "--noresult", "--labels=All")

	if ($Debug -eq $true) {
        # In order to support debugging we must start NUnit in the debugger.
        # This is because the --debug option for NUnit does not work on its own:
        # We are past test discovery when NUnit requests the debugger
        $innerCmd = $cmd
        $innerArgs = $args + @("--debug")
        $cmd = "vsjitdebugger.exe"
        $args = @($innerCmd ) + $args
	}

	& $cmd $args
}

function Check-CustomNUnitBuild() {
    Return Test-Path -Path "nunit-custom.user"
}

function Replace-FileLine($SubPath, $Pattern, $Replacement) {
    git checkout $SubPath
    $lines = Get-Content $SubPath | ForEach-Object { $_ -replace $Pattern , $Replacement }
    $lines | Set-Content $SubPath
}

if (!(Check-CustomNUnitBuild)) {
    Write-Host "Custom NUnit build not initialized... building NUnit"

    if (!(Test-Path ../extern/nunit -PathType Container)) {
        Write-Error "Unable to find custom nunit directory. Please check-out NUnit first in ../../nunit"
    }

    Push-Location ..\extern\nunit\

    # Set version
    Replace-FileLine build.cake "var version = .+" "var version = `"3.2.1`";"
    Replace-FileLine src\NUnitFramework\Frameworkversion.cs "\[assembly: AssemblyFileVersion\(.+" "[assembly: AssemblyFileVersion(`"3.2.1.0`")]"
    Replace-FileLine src\NUnitEngine\EngineVersion.cs "\[assembly: AssemblyVersion\(.+" "[assembly: AssemblyVersion(`"3.2.1.0`")]"
    Replace-FileLine src\NUnitEngine\EngineVersion.cs "\[assembly: AssemblyFileVersion\(.+" "[assembly: AssemblyFileVersion(`"3.2.1.0`")]"
    Replace-FileLine src\NUnitEngine\EngineApiVersion.cs "\[assembly: AssemblyFileVersion\(.+" "[assembly: AssemblyFileVersion(`"3.2.1.0`")]"

    try {
        .\build.ps1 -Configuration Debug -Target Build
    } finally {
        Pop-Location
    }

    Copy-Item ..\extern\nunit\bin\Debug\nunit-agent.* .\packages\NUnit.ConsoleRunner.3.2.1\tools -Force
    Copy-Item ..\extern\nunit\bin\Debug\nunit-agent-x86.* .\packages\NUnit.ConsoleRunner.3.2.1\tools -Force
    Copy-Item ..\extern\nunit\bin\Debug\nunit.engine.* .\packages\NUnit.ConsoleRunner.3.2.1\tools -Force
    Copy-Item ..\extern\nunit\bin\Debug\net-2.0\nunit.framework.* .\packages\NUnit.3.2.1\lib\net20 -Force
    Copy-Item ..\extern\nunit\bin\Debug\net-3.5\nunit.framework.* .\packages\NUnit.3.2.1\lib\net35 -Force
    Copy-Item ..\extern\nunit\bin\Debug\net-4.0\nunit.framework.* .\packages\NUnit.3.2.1\lib\net40 -Force
    Copy-Item ..\extern\nunit\bin\Debug\net-4.5\nunit.framework.* .\packages\NUnit.3.2.1\lib\net45 -Force
	Copy-Item ..\extern\nunit\bin\Debug\net-4.5\nunit.framework.* .\packages\NUnit.ConsoleRunner.3.2.1\tools -Force
    Copy-Item ..\extern\nunit\bin\Debug\portable\nunit.framework.* .\packages\NUnit.3.2.1\lib\portable-net45+win8+wp8+wpa81+Xamarin.Mac+MonoAndroid10+MonoTouch10+Xamarin.iOS10 -Force
	
	Write-Output "Check-CustomNUnitBuild" | Out-File nunit-custom.user
}


Write-Host "Building..."
try {
	msbuild @("UnitTestComparison.NUnit.sln", "/p:Configuration=Debug", "/t:Rebuild", "/verbosity:quiet", "/nologo")
	if ($LASTEXITCODE -ne 0) {
		Write-Error "Build failed"
		Exit $LASTEXITCODE
	}
} catch {
	Write-Error "Build failed (check if msbuild is in PATH)"
	Exit -1
}

Write-Host "Running tests..."
#Run "BankAccountApp.NUnitTests.Unit"
Run "BankAccountApp.NUnitTests.Integration"