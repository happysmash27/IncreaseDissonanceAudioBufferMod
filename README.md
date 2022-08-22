# Increase Dissonance Audio Buffer
This is a Melonloader mod to increase the dissonance audio buffer size from the default and inadequate 16, to a much larger number, by default 1024, in order to fix the issue where you sound like a microwave at low framerates due to the buffer being too small to store your audio in-between frames.

To build, download the project (`git clone https://github.com/happysmash27/IncreaseDissonanceAudioBufferMod`), cd into its directory in your terminal (`cd IncreaseDissonanceAudioBufferMod`), and run `dotnet build` for a Debug build or `dotnet build --configuration Release` for a Release build (I recommend a Release build for normal usage); or download and import it into an IDE that supports .csproj projects and build it there. If building on the command line, be sure to have dotnet installed so that the `dotnet` command exists!

The default search paths for necessary libraries to build this project, are based on having ChilloutVR installed in a standard location. If you have ChilloutVR installed in a non-standard location, or are building for something other than ChilloutVR, you can define where to fine the MelonLoader folder using `-p:MelonLoaderPath=<path>`, and where to find the Managed folder using `-p:ManagedPath=<path>`, or by simply linking them into the directory of the project using `ln -s <path> MelonLoader` and `ln -s <path> Managed` so you don't have to have such a large command line all the time. In all these commands, be sure to replace `<path>` with the full path to the original directory.

To install, move `IncreaseDissonanceAudioBuffer.dll` to the Mods directory in your ChilloutVR directory. When building from source, it can be found in `bin/Debug/net472/IncreaseDissonanceAudioBuffer.dll` for a debug build or `bin/Release/net472/IncreaseDissonanceAudioBuffer.dll` for a release build.