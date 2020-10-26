![Manifest](https://github.com/infinite-options/Manifest-Mobile/blob/master/Manifest/Manifest/Manifest.iOS/Assets.xcassets/AppIcon.appiconset/Icon120.png)
# Manifest-Mobile

## Table of Content
1. Introduction
2.Requirements
3. Prepare
4. Optional Packages
5. Note
6. Overview
7. Common Questions

## Introduction
An app to help people with memory imparement people

## Requiremnents
1. Visual Studio: you can doload it from following link: https://visualstudio.microsoft.com/downloads/
2. config folder, Ask for config folder to be shared with you so that you can run the project

## Prepare
1. Once cloned repository to the local system, paste the config folder at location: Manifest-Mobile/tree/master/Manifest/Manifest/Manifest
2. run Manifest-Mobile/Manifest/Manifest.sln upon start you visual studio might ask you to install other packages. Please install those packages

## Optional Packages (Suggested)
1. Windows users are suggested to install "Rapid XAML Analysis" nuget package

## Note
1. Mac users will see an exclaimation mark on ManifestTest (unit-test)
2. Apple login is not available on android

## Overview
This app uses MVVM architecture and built on xamarin Form. For navigation purpose xamarin Shell is used <br />
Basic Flow: App.xaml -> AppShell.xaml -> SplashScreen -> LoginPage.xaml (if not logged in) OR TodaysList.xaml (if logged in)

### Signin
Login Page contains the basic login features. It uses all the files in present in the Login folder <br />
Once login in complete Session (Models.Session Object) should be saved in Repository (Services.Repository)

### Services
Services contains the parts dealing with rest apis or storage <br />
UI should talk to repository to make any calls to web <br />
Repository is a singleton class so to get an instance of it Repository.Instance <br />

### Navigation
Please Register the page App.xaml or App.xaml.cs <br />
e.g. Following code snippet is used to register LoginPage in AppShell.xaml <br />
```
<ShellContent Route="LoginPage" Shell.FlyoutBehavior="Flyout" ContentTemplate="{DataTemplate local:LoginPage}" />
```
e.g. Following code snippet is used to register AubOccuranceListView in AppShell.xaml.cs <br />
```
Routing.RegisterRoute("SubOccuranceListView", typeof(SubOccuranceListView))
```
Use following syntax to navigate within app. And always use await while navigating fron an async function
1. Shell Navigation: Use this for simple navigation : https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/navigation
    ```
    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}")
    ```
2. Use Navigation Object : Use this function while passing any complex objects or functions to a page. 
    ```
    await Navigation.PushAsync(new SubOccuranceListView(tile.Id, informStatus))
    ```
    The above navigation send informSatus function as an argument
    
## Common Questions
### How to add images?
For mananging static pictures we use resizetizerNT https://github.com/Redth/ResizetizerNT <br />
Please use the following steps to add an image
1. Paste image in images folder in common project
2. Right click and go to properties of image you added and select "SharedImage" in Build Action
3. Open Manifest.csproject by double clicking the common project icon
4. Under ItemGroup Tag where other images are written. The entry should be made automatically. But adding an image size is suggest use the image size what you are going to use in the project. If is going to be used multiple times. you don't need to add image size. example of Shared image entry shown below
    ```
    <SharedImage Include="Images\calendarFive.svg" BaseSize="56,56" />
    ```
