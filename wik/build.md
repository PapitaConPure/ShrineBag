# How to build the damn thing
Hi!

Wanna build it? Just build it!

You've got two ways...

## The two ways
### IDE
Open the project with [VS Community]() or another .NET IDE and press the build button.

### This one little trick
Open the spell book, point your wand towards the project directory, and cast the ever handy:
```
dotnet restore
dotnet tool restore
dotnet build -c Release
```
...as long as you have a .NET SDK leviosa thing installed it should work.

## Risk of Options
There's a compile flag for Risk of Options integration. It's set to `true` by default. Yay!

If for some reason you don't want that, build the thing with `-p:EnableRiskOfOptions=false`.

Bye!
