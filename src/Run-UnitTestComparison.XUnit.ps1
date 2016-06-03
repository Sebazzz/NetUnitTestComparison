Param([Switch]$Debug)

function Run($subPath) {
	Write-Host "Running $subPath"

	$assemblyFile = "$subPath\bin\$subPath.dll"
	if ((Test-Path -Path $assemblyFile) -eq $false) {
		Write-Error -Message "Unable to find '$assemblyFile'. Please build the project first"
		Exit -1
	}

	$cmd = "packages\xunit.runner.console.2.1.0\tools\xunit.console.exe"
	$args = @($assemblyFile, "-nologo", "-noshadow", "-verbose")

	if ($Debug -eq $true) {
		$args += @("-debug")
	}

	& $cmd $args
}

Write-Host "Building..."
try {
	msbuild @("UnitTestComparison.XUnit.sln", "/p:Configuration=Debug", "/t:Rebuild", "/verbosity:quiet", "/nologo")
	if ($LASTEXITCODE -ne 0) {
		Write-Error "Build failed"
		Exit $LASTEXITCODE
	}
} catch {
	Write-Error "Build failed (check if msbuild is in PATH)"
	Exit -1
}

Write-Host "Running tests..."
Run "BankAccountApp.XUnitTests.Unit"
Run "BankAccountApp.XUnitTests.Integration"