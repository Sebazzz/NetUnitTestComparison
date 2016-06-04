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