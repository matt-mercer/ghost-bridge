ghost-bridge
============


# What is it ?
Ghost-Bridge is a bridge for continuous testing / build server testing of JavaScript tests, it allows you to run your javascript tests from visual studio test tools like [Resharper](http://www.jetbrains.com/resharper/) and [NCrunch](http://www.ncrunch.net/), and also on your build server with no extra effort.

# How does it work?
Ghost-Bridge uses a custom MS-Build target to ILGen a code stub that encapsulates the [Chutzpah](http://chutzpah.codeplex.com/) Test Runner,  ( Chutzpah uses PhantomJs.exe and supports [QUnit](http://qunitjs.com/) and [Jasmine](http://pivotal.github.io/jasmine/) Javascript tests )


# Versions
At the moment it only supports .Net 4.5

Instructions


