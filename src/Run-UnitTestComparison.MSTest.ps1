function Run($subPath, $item) {
	Write-Host "Running $subPath"

    if ($item -ne $null) {
        $item = "$subPath\$item"
    } else {
        $item = "$subPath\bin\$subPath.dll"
    }

	if ((Test-Path -Path $item) -eq $false) {
		Write-Error -Message "Unable to find '$item'. Please build the project first"
		Exit -1
	}

	$cmd = "vstest.console.exe"
	$args = @($item)

	& $cmd $args
}

function Check-Command($cmd) {
    Return [bool] (Get-Command $cmd -ErrorAction SilentlyContinue)
}


if (!(Check-Command vstest.console.exe)) {
    Write-Error "Unable to find vstest.console.exe in your PATH"
    Exit -2
}


Write-Host "Building..." -ForegroundColor Cyan
try {
	msbuild @("UnitTestComparison.MSTest.sln", "/p:Configuration=Debug", "/t:Rebuild", "/verbosity:quiet", "/nologo")
	if ($LASTEXITCODE -ne 0) {
		Write-Error "Build failed"
		Exit $LASTEXITCODE
	}
} catch {
	Write-Error "Build failed (check if msbuild is in PATH)"
	Exit -1
}

Write-Host "Running tests..." -ForegroundColor Cyan
Run "BankAccountApp.MSTests.Unit"
Run "BankAccountApp.MSTests.Integration" "Integration.orderedtest"