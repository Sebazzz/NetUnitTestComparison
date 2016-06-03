Param([Switch]$Debug)

function Run($subPath) {
	Write-Host "Running $subPath"

	$assemblyFile = "$subPath\bin\$subPath.dll"
	if ((Test-Path -Path $assemblyFile) -eq $false) {
		Write-Error -Message "Unable to find '$assemblyFile'. Please build the project first"
		Exit -1
	}

	$packageDir = "packages\NUnit.ConsoleRunner.3.2.1\tools"
	$cmd = "$packageDir\nunit3-console.exe"
	$args = @($assemblyFile, "--noheader", "--verbose", "--process=InProcess", "--domain=None")

	if ($Debug -eq $true) {
		$args += @("--debug", "--pause")
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
Run "BankAccountApp.NUnitTests.Unit"
Run "BankAccountApp.NUnitTests.Integration"