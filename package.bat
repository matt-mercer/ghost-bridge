msbuild src\GhostBridge.MSpec\GhostBridge.MSpec.csproj /t:ILMerge /p:Configuration=Release
.nuget\nuget.exe pack src\GhostBridge.MSpec\GhostBridge.MSpec.csproj -Symbols -Properties Configuration=Release

msbuild src\GhostBridge.NUnit\GhostBridge.NUnit.csproj /t:ILMerge /p:Configuration=Release
.nuget\nuget.exe pack src\GhostBridge.NUnit\GhostBridge.NUnit.csproj -Symbols -Properties Configuration=Release