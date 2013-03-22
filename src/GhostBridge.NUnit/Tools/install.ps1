param($installPath, $toolsPath, $package, $project)
# comment the above line an uncomment these to run 'locally'
#$project = Get-Project
#$installPath = "C:\_src\ghost-bridge\packages\GhostBridge.NUnit.0.1.0.0"
#$package = Get-Package "GhostBridge.Mspec"

$targets = "GhostBridge.msbuild.targets"
$path = [System.IO.Path]
$dllPath = $path::Combine($installPath, "lib\net45\" + $package.Id + ".dll")


$targetsFile = $path::Combine($path::GetDirectoryName($project.FullName),$targets)
$targetsUri = New-Object System.Uri($targetsFile)
$dllUri = New-Object System.Uri($dllPath)
$dllPath = $targetsUri.MakeRelativeUri($dllUri)
$isvb = $project.CodeModel.Language -eq [EnvDTE.CodeModelLanguageConstants]::vsCMLanguageVB


(Get-Content $targetsFile) | Foreach-Object {
    if($isvb) {
        $_ -replace "dummypath", $dllPath `
        -replace 'ghost_bridge_specs.cs', 'ghost_bridge_specs.vb'
    }
    else { $_ -replace "dummypath", $dllPath }
} | Set-Content $targetsFile

if($project.Type -eq 'Web Site') {
    Write-Warning "Skipping '$($project.Name)', Website projects are not supported"
    return
}

Add-Type -AssemblyName ‘Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a’
# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

if(!($msbuild.Xml.Imports | ?{ $_.Project -eq $targets } )) {
    $msbuild.Xml.AddImport($targets) | Out-Null
    $project.Save()
    $msbuild.ReevaluateIfNecessary()
    Write-Host "Added ms build task"
}       
                 
