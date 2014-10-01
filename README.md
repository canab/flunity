# Idea

Main idea is make it possible to use [Adobe Flash](www.adobe.com/products/flash.html) for creating visual content for unity games.

The framework consists of two parts:

- **FlashExporter**: desktop application written with Adobe Air that converts flash objects from .swf file to set of resources for unity project.
- **RuntimeLibrary**: C# Library that renders graphics and plays animation at unity runtime.

# Exporting Features

- Exporting symbols from Flash as bitmap sprite.
- Exporting animated symbols from Flash as as bitmap sequence.
- Exporting animated symbols from Flash as timeline animation.
- Packing sprites to sprite sheet.
- Support for basic geometric transforms: scale, rotation, position.
- Support for color transforms: brightness and tint for each channel.
- Design in Flash complex compositions of visual objects. Hierarchical structure of visual objects in such compositions is accessible from unity code.
- Support for text rendering. Font is being exported as set of bitmaps (for each used character).
- Code generation: C# code is being generated for all resource items. Compile-time checking prevents from errors like if someone accidentally removes or renames symbols in Flash.

# Runtime Features
- Clean and robust API for manipulating of composition of visual objects
- Flash objects are rendered in a [**FlashStage**](https://github.com/nravo/flunity/wiki/Scene-Setup-In-Unity) component using one draw call. Whole flash scene is rendered as one mesh.
- Touch events for single object
- Tween library for programmatic animation

# Getting Started

The fastest way is to checkout **master** branch of this repository.  
It contains **SampleProject** which is ready to run in unity.

# Next steps

- [Quick Programming Guide](https://github.com/nravo/flunity/wiki/Quick-Programming-Guide)
- [Working with Resources](https://github.com/nravo/flunity/wiki/Working-with-Resources)
- [Scene Setup In Unity](https://github.com/nravo/flunity/wiki/Scene-Setup-In-Unity)
- [API Documentation](http://nravo.github.io/flunity/api-docs/namespace_flunity.html)
