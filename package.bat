msbuild src\GhostBridge.MSpec\GhostBridge.MSpec.csproj /t:ILMerge /p:Configuration=Release
.nuget\nuget.exe pack src\GhostBridge.MSpec\GhostBridge.MSpec.csproj -Properties Configuration=Release

msbuild src\GhostBridge.NUnit\GhostBridge.NUnit.csproj /t:ILMerge /p:Configuration=Release
.nuget\nuget.exe pack src\GhostBridge.NUnit\GhostBridge.NUnit.csproj -Properties Configuration=Release