Welcome to StudioMobile Common library. 

## Getting started ##

**If you want to use library code and make changes to it at the same time then follow below instructions:**

1. Checkout Studio Mobile Common from git.
1. Go to top level folder and install dependencies (nuget install).
1. Open you app project and create 'Solution Folder' Studio Mobile.
1. Add to this folder Common project and platform specific projects as necessary.
1. Open platform specific projects in Common and 'refresh' it components. You have to be logged in with Xamarin account to do this.
1. Add references to StudioMobile.<platform> assemblies to your project.

You should end up with a project structure like following
```
Your Solution
├── Studio Mobile 
├──── StudioMobile.Android
├──── StudioMobile.Common
├──── StudioMobile.iOS
├──── StudioMobile.NET
├── <Your Project>.iOS
├──── Referencies
├────── StudioMobile.iOS
├── <Your Project>.Android
├──── Referencies
├────── StudioMobile.Android
├── <Your Project>.NET
├──── Referencies
├────── StudioMobile.NET
├── <Your Project>.Common
```
# Project structure #

* Common - shared, cross platform code
* iOS - iOS specific
* Android - Android specific
* NET - .NET specific code

Every project has similar structure. There are several top level folders:
* Model - contains code which is used when you create Models and implement application logic.
* View - contains code which is used when you create Views.
* Controller - contains code which is used when you create Controllers.
* Util - small things which are used everywhere.

Sometimes you can find additional README or RATIONALE files inside project folders. These files contain additional information about code in this folder.

## Platform specific code ##

Platform specific functions are consumed in Commons project by using one of these methods
1. Wrap native functionality in every platform specific project. Ex: Image, RGB
1. Put common in a partial class in Common and add platform specific partials in platform projects. Ex: Font, IconGenerator
1. Define common interface in Common and then implement it in platform code. 

## External libraries ##

Dependencies are managed by NuGet and Xamarin Components, all dependencies are downloaded into Lib folder as configured in top level nuget.config. Each platform project then contains a references to specific assembly downloaded by NuGet. List of libraries are in top level packages.config. 

If some class in the library depends on thirdparty assembly (such as Parse.com SDK, or LibPhoneNumber) then such code is put in StudioMobile.<LibraryName> namespace. For example StudioMobile.Parse, StudioMobile.LibPhoneNumbers, etc. When you use something from StudioMobile.<LibraryName> namespace make sure you also link with corresponding library otherwise FileLoadException will be thrown at the time you call any method from that namespace.