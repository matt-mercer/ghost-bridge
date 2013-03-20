msbuild src\GhostBridge.MSpec\GhostBridge.MSpec.csproj /t:ILMerge /p:Configuration=Debug
.nuget\nuget.exe pack src\GhostBridge.MSpec\GhostBridge.MSpec.csproj -Symbols -Properties Configuration=Debug