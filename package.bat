msbuild src\GhostBridge.MSpec\GhostBridge.MSpec.csproj /t:ILMerge /p:Configuration=Release
.nuget\nuget.exe pack src\GhostBridge.MSpec\GhostBridge.MSpec.csproj