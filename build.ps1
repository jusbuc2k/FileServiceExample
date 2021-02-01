#!/usr/bin/env powershell
#requires -version 4
#
Param(
	[alias("c")][string]
	$Configuration = "Release",	
	[switch]
	$NoTest
)

$Solution =  "$(Get-Item -Path *.sln | Select-Object -First 1)"
$OutputPackages = @()
$TestProjects = Get-Item -Path tests\**\*UnitTests.csproj | %{ $_.FullName }
$PublishProjects = @(
	".\src\FileServiceAPI\FileServiceAPI.csproj"
)

Write-Host "==============================================================================" -ForegroundColor DarkYellow
Write-Host "The Build Script for File Service Example"
Write-Host "==============================================================================" -ForegroundColor DarkYellow
Write-Host "Build Tools:`t$BuildToolsVersion"
Write-Host "Solution:`t$Solution"
Write-Host "Skip Tests:`t$NoTest"
Write-Host "==============================================================================" -ForegroundColor DarkYellow

try {
	Write-Host "Setting build server attributes..."
	& dotnet tool install --tool-path .\obj\ nbgv
	& .\obj\nbgv cloud

	# RESTORE
	Write-Host "Restoring Packages..." -ForegroundColor Magenta
	& dotnet restore $Solution
	if ($LASTEXITCODE -ne 0) {
		throw "Package restore failed with exit code $LASTEXITCODE."
	}

	# BUILD SOLUTION
	Write-Host "Performing build..." -ForegroundColor Magenta
	& dotnet build $Solution --configuration $Configuration

	# RUN TESTS
	if ( !($NoTest.IsPresent) -and $TestProjects.Length -gt 0 ) {
		Write-Host "Performing tests..." -ForegroundColor Magenta
		foreach ($test_proj in $TestProjects) {
			Write-Host "Testing $test_proj"			
			dotnet test $test_proj --no-build --configuration $Configuration #--filter TestCategory=Unit
			if ($LASTEXITCODE -ne 0) {
				throw "Test failed with code $LASTEXITCODE"
			}
		}
	}

	# CREATE NUGET PACKAGES
	if ( $OutputPackages.Length -gt 0 ) {
		Write-Host "Packaging..."  -ForegroundColor Magenta
		foreach ($pack_proj in $OutputPackages){
			Write-Host "Packing $pack_proj"
			dotnet pack $pack_proj --no-build --configuration $Configuration
			if ($LASTEXITCODE -ne 0) {
				throw "Pack failed with code $result"
			}
		}
	}
	
	# Publish asp.net core projects
	if ( $PublishProjects.Length -gt 0 ) {
		Write-Host "Publishing..."  -ForegroundColor Magenta
		foreach ($pub_proj in $PublishProjects){
			Write-Host "Publishing $pub_proj"
			dotnet publish $pub_proj --no-build --no-restore --configuration $Configuration
			if ($LASTEXITCODE -ne 0) {
				throw "Publish failed with code $result"
			}
		}
	}

	if ($LASTEXITCODE -ne 0) {
		throw "Publish failed with code $result"
	}

	Write-Host "All Done. This build is great! (as far as I can tell)" -ForegroundColor Green
	exit 0
} catch {
	Write-Host "ERROR: An error occurred and the build was aborted." -ForegroundColor White -BackgroundColor Red
	Write-Error $_	
	exit 3
}