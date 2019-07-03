[logo]: https://raw.githubusercontent.com/Geeksltd/{Plugin.Name}/master/Shared/NuGet/Icon.png "{Plugin.Name}"


## {Plugin.Name}

![logo]

a brief description of the app


[![NuGet](https://img.shields.io/nuget/v/{Plugin.Name}.svg?label=NuGet)](https://www.nuget.org/packages/{Plugin.Name}/)

> The definition or description of the native feature or third party plugin

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/{Plugin.Name}/](https://www.nuget.org/packages/{Plugin.Name}/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage
......

<br>

### Platform Specific Notes
Some platforms require certain permissions or settings before it will display notifications.


#### Android
......

```csharp
......
```

 
#### iOS
......

```csharp
......
```

#### UWP
......

```csharp
......
```

<br>


### Properties
| Property     | Type         | Android | iOS | Windows |
| :----------- | :----------- | :------ | :-- | :------ |
| id           | int          | x       | x   | x       |



<br>


### Events
| Event             | Type                                          | Android | iOS | Windows |
| :-----------      | :-----------                                  | :------ | :-- | :------ |
| Tapped            | AsyncEvent<KeyValuePair<string, string>[]>    | x       | x   | x       |


<br>


### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Show         | Task<bool&gt;| title -> string<br> body -> string| x       | x   | x       |
