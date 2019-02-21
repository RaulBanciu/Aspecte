@echo off

IF NOT EXIST ".git" (
	echo No git directory found
	del Versioning.cs
	goto DOTNETBUILD
)

Rem get the version number
Rem and put it into the variable versionNumber
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-parse HEAD`) DO (
SET versionNumber=%%F
)

Rem define in Main.cs the flag VERSIONING
copy Main.cs temp.txt
echo #define VERSIONING > Main.cs
type temp.txt >> Main.cs
del temp.txt

Rem write the version number to a separate file
(
echo static class Versioning {
echo public static string VersionNumber =^> "%versionNumber%";
echo }
) > Versioning.cs

:DOTNETBUILD
Rem done, we can build
dotnet publish --runtime win10-x64

PAUSE