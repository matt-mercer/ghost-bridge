ghost-bridge
============


# What is it ?
Ghost-Bridge is a bridge for continuous testing / build server testing of JavaScript tests, it allows you to run your javascript tests from visual studio test tools like [Resharper](http://www.jetbrains.com/resharper/) and [NCrunch](http://www.ncrunch.net/), and also on your build server with no extra effort.

# How does it work?
Ghost-Bridge uses a custom MS-Build target to ILGen a code stub hook for MSpec or NUnit that encapsulates the [Chutzpah](http://chutzpah.codeplex.com/) Test Runner,  ( Chutzpah uses PhantomJs.exe and supports [QUnit](http://qunitjs.com/) and [Jasmine](http://pivotal.github.io/jasmine/) Javascript tests )


# Versions
At the moment it only supports .Net 4.5, this is because I wanted it to be dependency free, (apart from the test frameworks themselves), to make it portable for NCrunch .. I didn't get round to implementing the zip inflate algorithm for .net 4.0, oh and Chutzpah is only .net 4.0 in recent versions


# Installation

`install-package GhostBridge.Mspec`
or
`install-package GhostBridge.NUnit`

Remember this only works with  projects with target framework >= .net 4.5 !

the nuget install should add the following to the project file  `<Import Project="GhostBridge.msbuild.targets" />`

the msbuild target should look something like this

```
<?xml version="1.0" encoding="utf-8" ?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
	<UsingTask TaskName="GhostBridge.MSpec.MSBuildTask" AssemblyFile="../packages/GhostBridge.MSpec.1.1.0.0/lib/net45/GhostBridge.MSpec.dll" />
	<Target Name="GhostBridge" AfterTargets="AfterResolveReferences">
		<Message Text="Generating tests" Importance="High" />
		<MSBuildTask BaseDirectory="$(ProjectDir)" Pattern="*.*" Language="$(Language)" ProjectDir="$(ProjectDir)">
			<Output TaskParameter="SpecCount" ItemName="SpecCount" />
		</MSBuildTask>
		<Message Text="Found :: @(SpecCount) specs" Importance="High" />
		<ItemGroup Condition="@(SpecCount)!='0'">
			<Compile Include="ghost_bridge_specs.cs" />
		</ItemGroup>
		<Message Text="Complete" Importance="High" />
	</Target>
</Project>
```




things to note here:

`BaseDirectory="$(ProjectDir)"`  change this to restrict where the test generator looks for your specs .. eg `BaseDirectory="$(ProjectDir)\jasmin-specs"`

`Pattern="*.*"` change this to filter the files ... eg `Pattern="*.jasmin.spec.js`


`AssemblyFile="../packages/GhostBridge.MSpec.1.1.0.0/lib/net45/GhostBridge.MSpec.dll" />` 
this path will need to be available at build time to your test runner , this is particularly relevant for NCrunch ( look at 'additional files to include' for your project under ncrunch settings )


