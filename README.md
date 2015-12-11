C# Coding Guidelines for VS2015
======================
[![Build status](https://ci.appveyor.com/api/projects/status/xq8m8wcecfm1avmc/branch/master?svg=true)](https://ci.appveyor.com/project/SamirHafez/csharpcodingguidelines/branch/master)
[![NuGet Status](https://img.shields.io/nuget/v/CodingGuidelines.svg)](https://www.nuget.org/packages/CodingGuidelines)

C# Coding guidelines is a Visual Studio 2015 analyzer nuget package which tries to follow the [Coding Guidelines for C#](http://csharpguidelines.codeplex.com) as defined by Dennis Doomens.

If provides features such as:
* Always check the result of an as operation
* Avoid methods that take a bool flag
* Don't allow methods and constructors with more than three parameters
* Don't use if-else statements instead of a simple (conditional) assignment
* Be reluctant with multiple return statements
* Avoid nested loops
* Don't make explicit comparisons to true or false
* Only use var when the type is very obvious
* Avoid conditions with double negatives
* Methods should not exceed 7 statements
