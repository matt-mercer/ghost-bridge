param($installPath, $toolsPath, $package, $project)
# comment the above line an uncomment these to run 'locally'
#$project = Get-Project

$targets = "GhostBridge.msbuild.targets"
$path = [System.IO.Path]
$file = [System.IO.File]
$isvb = $project.CodeModel.Language -eq [EnvDTE.CodeModelLanguageConstants]::vsCMLanguageVB
$specs_file = "ghost_bridge_specs.cs"
if($isvb) {
	$specs_file = "ghost_bridge_specs.vb"
}
$specs_file = $path::Combine($path::GetDirectoryName($project.FullName),$specs_file)


if($file::Exists($specs_file)) {
	$file::Delete($specs_file)
	Write-Host "Removed specs file"
}


if($project.Type -eq 'Web Site') {
    Write-Warning "Skipping '$($project.Name)', Website projects are not supported"
    return
}

Add-Type -AssemblyName ‘Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a’
# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

$msbuild.Xml.Imports | Foreach-Object { 
	if($_.Project -eq $targets) {
		$msbuild.Xml.RemoveImport($targets) | Out-Null
		$project.Save()
		$msbuild.ReevaluateIfNecessary()
		Write-Host "Removed ms build task"
	}
}
 
                 
