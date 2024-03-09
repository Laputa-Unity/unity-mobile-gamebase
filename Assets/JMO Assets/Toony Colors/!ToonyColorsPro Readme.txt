Toony Colors Pro, version 2.8
2021/09/28
© 2021 - Jean Moreno
=============================

QUICK START
-----------
Select one of the following shader in your material:

Built-in or URP:
- Toony Colors Pro 2/Hybrid

Built-in only:
- Toony Colors Pro 2/Standard PBS
- Toony Colors Pro 2/Standard PBS (Specular)

Then set the options in the material inspector.

To go further, you can use the Shader Generator 2 to make your own stylized shaders.
Please read the documentation for more information!


PLEASE LEAVE A REVIEW ON THE ASSET STORE IF YOU FIND THE ASSET USEFUL! It really helps!
Thank you and enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:
jean.moreno.public+unity@gmail.com

I'd be happy to see Toony Colors Pro 2 used in your project, so feel free to drop me a line about that! :)


UPDATE NOTES
------------

See full and formatted changelog here: https://jeanmoreno.com/unity/toonycolorspro/doc/changelog

2.8
#### Added
- [Shader Generator 2] Added ability to generate *Terrain-compatible shaders*, with the flexibility of the Shader Properties system (see documentation and tutorials)
- [Shader Generator 2] Added option to use a "Custom Material Property" as the UV source for a "Material Property/Texture" (e.g. allowing gradient remapping effects)
- [Shader Generator 2] Added ability to reuse samplers for texture properties (Unity 2019.4+)
- [Shader Generator 2] Added "Transparency Dithering" option for the "Alpha Testing" feature with 4 included dithering pattern textures
- [Shader Generator 2] Triplanar: added "Bump Scale" option when Normal Map is enabled
- [Shader Generator 2] Triplanar UV: added "Scale" field to easily change the order of magnitude when needed (e.g. to avoid using very small UV tiling values for Terrain textures)
- [Shader Generator 2] Triplanar: added "Ceiling Smoothness" shader property when using "Min Max Threshold" ceiling mode
- [Shader Generator 2] Vertical Fog: added new options (space, make optional)
#### Modified
- [Shader Generator 2] (URP) Force normals normalization per-pixel for all platforms (was per-vertex on mobile previously). Use Code Injection if you really need to optimize things this much.
- [Shader Generator 2] (Default) Using "Alpha Testing" now also works with "Outline" pass (but is unlikely to work nicely visually due to the outline technique used)
- [Shader Generator 2] Generated shaders now use "shader_feature_local" and their "fragment/vertex" versions when relevant, if supported by the running Unity version (reduces global shader keyword count and compiled shader variants count)
#### Fixed
- [Hybrid Shader] [Shader Generator 2] (URP) Fixed shadow casting for point lights (URP 11+)
- [Shader Generator 2] (Default) Fixed normal map with triplanar UV when using "Sample Normal Map First" option
- [Shader Generator 2] (Default) Fixed bug where dithered shadows wouldn't work
- [Shader Generator 2] (Default) (URP) Rim effects correctly use triplanar normal maps if they exist
- [Shader Generator 2] (Default) Fixed alpha clipping in depth pass when using "Alpha to Coverage" option
- [Shader Generator 2] Fixed channels swizzle not taken into account when using "Triplanar" UV mapping on "Material Property/Texture"
- Restored Water shader and material example in the "Shader Generator 2" demo scene

2.7.4
#### Fixed
- [Hybrid Shader] (URP) Fixed an issue where shadow casting shader variants were stripped from builds when using URP (thus breaking shadows in builds)

2.7.3
#### Added
- [Hybrid Shader] Added automatic stripping of shader variants at build time, based on if the project is using URP or not
#### Fixed
- [Shader Generator 2] Fixed material inspector UI when using Material Layers
- [Shader Generator 2] Fixed UI issue with Material Layers tab
- [Shader Generator 2] Fixed small UI issues

2.7.2
#### Added
- [Shader Generator 2] Added "Progressive Sketch" option for "Sketch" stylization feature, inspired by the Tonal Art Map technique
- [Shader Generator 2] Added "Maximum Pixel Size" option for "Outline" feature when using "Clip Space" mode
- [Shader Generator 2] Added "Global Variable" option for Material Property implementations, so that they can be changed with scripts (using "Shader.SetGlobal*" methods)
- [Hybrid Shader Outline] Added a "Min Max" pixel size option for outline
#### Modified
- Updated the "TCP2 Demo Shader Generator 2" scene shaders with the latest SG2 version
- [Shader Generator 2] Changed "View Dir Ambient Sampling" label to "Single Indirect Color", so that it's the same as in the Hybrid Shader
- [Shader Generator 2] Typing a single letter in a swizzle input will expand it if necessary (e.g. typing "A" in a texture swizzle will expand it to "AAA" for a color property instead of discarding it)
- [Shader Generator 2] Now using a monospace font for "Custom Code" and Material Property "Variable" text fields
- [Shader Generator 2] Added a lot of help buttons in the FEATURES tab
#### Fixed
- [Hybrid Shader Outline] Fixed outline size that was dependent on screen resolution: this is now only the case with pixel sizes options (constant, minimum, maximum)
  *NOTE: this may change the outline size of your existing materials using the Hybrid Shader Outline!*
- [Shader Generator 2] Fixed outline size that was dependent on screen resolution when in clip space: this is now only the case with pixel sizes options (constant, minimum, maximum)
- [Shader Generator 2] Fixed "Reflection Color" shader property only being applied when "Planar Reflections" was enabled
- [Shader Generator 2] Fixed errors with "Code Injection" when the referenced injection file of a shader has been modified and the shader loaded again

2.7.1
#### Added
- [Shader Generator 2] (URP) Added point lights shadow support (URP 11+)
- [Shader Generator 2] "Sine Distortion" UV animation: added "Global" option to make the calculation global to the selected texcoord channel
- [Hybrid Shader] (URP) Added point lights shadow support (URP 11+)
- [Hybrid Shader] (Default) Added cast shadows support for additional point/spot/directional lights (built-in render pipeline)
#### Modified
- [Shader Generator 2] Removed LWRP template
#### Fixed
- [Hybrid Shader] Fixed "Fade" transparent mode
- [Hybrid Shader] Fixed "Shadowmask" lightmap mode
- [Hybrid Shader] Fixed compilation issue in shadow pass when compiling for certain platforms
- [Shader Generator 2] Fixed global UV settings being applied multiple times with Custom Material Properties
- [Shader Generator 2] Fixed compilation error when using a different texcoord channel for layered properties
- [Shader Generator 2] Fixed some UV issues when using Custom Material Properties in layered properties
- [Shader Generator 2] (Default) Fixed compilation error when a Custom Material Property is used for a hook in the Lighting function of a surface shader
- [Shader Generator 2] (Default) Fixed worldPos and other sepcific data not being computed when they are only used in a "#if defined(UNITY_PASS_FORWARDBASE)" block
- [Shader Generator 2] (URP) Fixed spotlight shadows (URP 10+)
- [Shader Generator 2] (URP) Fixed "Shadowmask" lightmap mode (URP 10+)
- [Shader Generator 2] (URP) Fixed depth texture sampling in URP for depth effects, depending on the clipping planes values
- [Standard PBS] Fixed compilation issue on PlayStation platforms
- [Standard PBS] Fixed a different compilation issue on Switch and Xbox platforms

2.7.0
#### Added
- [Shader Generator 2] Added a new "Material Layers" system to the Shader Generator, allowing to add any number of layers in a single shader.
This can be used to make effects from simple texture blending to snow accumulation, procedural moss and more! Please read the documentation about it!
- Added "Material Layers" section in the documentation
- Added "Demo TCP2 Material Layers" to show Material Layers examples
- [Shader Generator 2] (Default) (URP) Added "Minimum Pixel Width" option for the "Outline" feature when "Clip Space" is selected (acts like the existing option in the "Hybrid Shader")
- [Shader Generator 2] (Default) Added "Sample Normal Map first" option to make sure the normal map is sampled before using world normal elsewhere (e.g. for Material Layers)
- Added texture "TCP2_SketchG_Hatches" for sketch effects
#### Modified
- [Shader Generator 2] The "Code Injection" system has been updated:
  - An injection file that is updated externally (i.e. in a text editor) will automatically be reloaded when going back in Unity
  - Replace blocks now need to have a name defined, and will be listed alongside other blocks
  - All blocks can now be easily disabled (e.g. using a single injection file for multiple shaders)
- Planar Reflections script now allows changing the reflection Render Texture format
#### Fixed
- [Hybrid Shader] Fixed shader compilation errors when building for some platforms
- [Shader Generator 2] (URP) Fixed "Dissolve" effect with the "Outline" pass in URP
- [Shader Generator 2] (Default) (URP) "Triplanar" now works nicely with "Texture Blending"
- [Shader Generator 2] (Default) Fixed wrong attenuation value when using Lightmaps with shadow masks in Mixed Lighting setups
- Fixed the shadow color on materials in the "Demo TCP2 Hybrid Shader" scene
- [Shader Generator 2] "Outline": Correctly perform screen aspect ratio correction when outlines are in Clip Space

2.6.4
#### Fixed
- [Shader Generator 2] (URP) Fixed "Vertex Displacement" error when loading the URP template
- [Shader Generator 2] Removed serialization debug log

2.6.3
#### Added
- [Hybrid Shader] (Default) (URP) Added "Main Light affects Shadow Color" option to have the main light color affect the shadow color (enabled by default, this was the old behavior before v2.6.1)
- [Hybrid Shader] (Default) (URP) Added "Golbal Illumination" material flags options in the material inspector UI
- [Hybrid Shader] (Default) (URP) Added "Meta Pass" to allow baking lighting when using the Hybrid Shader
- [Hybrid Shader] (URP) Added support for SSAO and Depth Normals pass
- [Shader Generator 2] (URP) Added support for SSAO ("Enable SSAO" in the "OPTIONS" section)
- [Shader Generator 2] (URP) Added support for the Depth Normals pass ("Enable Depth Normals Pass" in the "OPTIONS" section)
- [Shader Generator 2] (Default) (URP) Added "Vertex Displacement" feature in the "SURFACE" section (should be easier to use than the existing "Vertex Position" Hook)
- [Shader Generator 2] "Sine Distortion" UV animation: added ability to use another texture's parameters for the animation, to synchronize both textures
- [Shader Generator 2] Added "Reflection Color" Shader Property when using "Planar Reflections"
#### Modified
- [Shader Generator 2] (Default) (URP) Exposed "Triplanar Parameters" in Shader Properties, and added "Triplanar Normal" hook
- [Shader Generator 2] (Default) (URP) "Dissolve" effect now works in the "Outline" pass as well (but it will likely have artifacts)
- [Shader Generator 2] Overwrite prompt will always be called when generating a new shader over an existing path, even if the "don't prompt" option is enabled
#### Fixed
- Fixed possible null reference error in TCP2_PlanarReflection script
- Fixed UI issue in the "Hybrid Shader Demo" at 1080p resolution (text was cropped)
- Fixed serialization issue for vectors for environments where decimals separation is a comma
- [Hybrid Shader] (Default) (URP) Fixed cast shadows when using "Alpha Clipping"
- [Shader Generator 2] (URP) Fixed "VertExmotion" support for URP
- [Shader Generator 2] (URP) Fixed "Directional Ambient" on mobile builds for URP

2.6.2
#### Fixed
- Fixed TCP2_PlanarReflection script compilation error in some Unity versions when URP isn't installed

2.6.1
#### Added
- [Shader Generator 2] (Default) (URP) Added "Planar Reflections" option that works with the eponymous script
- [Shader Generator 2] Added vertex "Local Position", "Local Normal" and "World Normal" implementations for the "Shader Properties" system
- Planar Reflection script now has an option to blur the reflection texture
#### Modified
- [Shader Generator 2] Reorganized vertex-based implementations into their own sub-menu ("Shader Properties" system)
- [Shader Generator 2] Disabled "Aura 2" third party support as it's not actually needed for compatibility
- Planar Reflection script now works with URP
- Improved UI readability for Material Inspectors: using orange labels for headers, better spacing after certain headers
#### Fixed
- [Hybrid Shader] Fixed main directional light color/intensity affecting unlit parts too when it shouldn't
- [Hybrid Shader] Fixed Occlusion being applied twice for the built-in rendering pipeline
- [Shader Generator 2] (Default) (URP) Fixed shadow casting when using "Curved World 2020" support
- [Shader Generator 2] (URP) Fixed "Sine Distortion" UV animation in the shadow pass when used with "World Position" UVs
- [Shader Generator 2] Copying a shader now copies "Code Injection" settings
- [Shader Generator 2] Fixed minor issues with the undo/redo system

2.6.0
#### Added
- [Shader Generator 2] (Default) (URP) Added Water Effects in the default templates: vertex-based waves animation & depth-based effect (color, foam, transparency)
- [Shader Generator 2] (Default) (URP) Added "Auto Optional Transparency" feature: it will add a "Rendering Mode" option in the material inspector to select opaque, fade or transparent mode (as in the Hybrid Shader)
- [Shader Generator 2] (Default) (URP) Added "Custom Time" option in the "Special Effects" section: this will override the built-in shader _Time value with a custom vector, controlable with scripts
- [Shader Generator 2] (Default) (URP) Added support for "Curved World 2020" third party asset
- [Shader Generator 2] Added "Sine Distortion" UV animation for "Material Property/Texture" properties: use this water-like animation for almost any texture in your shader!
- "TCP2_PlanarReflection" script: added option to define a custom background color, instead of inheriting the one from the main camera
- [Documentation] Added sections about the aforementioned new features
#### Modified
- [Shader Generator 2] (Default) N.V-based effect like Rim Lighting now looks a bit better (less visible vertices artefacts in some cases)
#### Fixed
- [Shader Generator 2] (Default) (URP) Wind: fixed Wind Texture being sampled twice
- [Shader Generator 2] (Default) (URP) Fixed N.V (e.g. rim effects) calculation for back faces
- [Shader Generator 2] Performance optimizations when parsing template conditions (which happens every time an option is changed)

2.5.4
#### Added
- [Shader Generator 2] Added "Code Injection" tab: this is a new system allowing to insert arbitrary code at various points in the shader file, for even more control over the resulting file. See the documentation to learn more!
#### Modified
- [Hybrid Shader] Optimizations and readability
#### Fixed
- [Shader Generator 2] (URP) Fixed compilation error with 'vertexNormalInput'
- [Shader Generator 2] Fixed "Vertex UV" implementation sampling
- [Hybrid Shader] Shadow and depth passes now respect the culling mode set in the material inspector
- [Hybrid Shader] Fixed fog calculations for mobile platforms
- [Demo] Updated the Cat Demo URP shaders with the latest template version (fixes compatibility with latest URP, notably shadows and VR)

2.5.3
#### Added
- "Animated Dissolve" example shader in the "Demo TCP2 ShaderGenerator 2" scene
- Added a tutorial about the creation of the "Animated Dissolve" shader in the documentation
- [Shader Generator 2] Added "Triplanar" UV option for "Material Property/Texture" properties: use triplanar mapping for almost any texture in your shader!
#### Modified
- [Shader Generator 2] Improved "Shader Properties" UI, notably for Unity 2019.3+
#### Fixed
- "Hologram" demo shader is now in "Toony Colors Pro 2/Examples/SG2/Hologram" instead of the root of the shaders menu
- [Shader Generator 2] (Default) (URP) Fixed compilation error where some features would miss variable declaration (e.g. Triplanar)

2.5.2
#### Added
- Added back the "Cat Demo URP" package that wrongly disappeared, and removed the LWRP one
- [Shader Generator 2] GPU Instancing options are now available in the OPTIONS section
- [Shader Generator 2] (Default) (URP) Added "Apply Shadows before Ramp" feature
#### Modified
- [Shader Generator 2] Shader output is different and simplified: all variables for all passes will always be declared in an include block at the top now
#### Fixed
- [Hybrid Shader] Fixed compilation error for outline version when using UV2 as normals and textured outline
- [Hybrid Shader] Fixed harmless GUI error when assigning Hybrid Shader Outline to a new material
- [Hybrid Shader] Fixed [MainTexture] tag to identify _BaseMap as the main texture in the shader (Unity 2019.1+)
- [Hybrid Shader Demo] Fixed UI issue when resolution isn't 720p
- [Shader Generator 2] Fixed Shader Properties marked as "GPU Instanced"
- [Shader Generator 2] "Silhouette Pass" now works with GPU Instancing
- [Shader Generator 2] Fixed error when selecting an invalid shader in the "Current Shader" field
- [Shader Generator 2] (URP) Fixed compilation errors with strict compilers (e.g. PS4)
- [Shader Generator 2] (URP) Fixed "Affect Ambient" option for "Sketch" stylization feature
- [Shader Generator 2] (URP) Fixed "View Dir Ambient Sampling" option
- [Shader Generator 2] (URP) "SRP Batcher" option is removed and now always enabled, and it also fixes some compilation errors that could happen without it

2.5.1
#### Added
- [Shader Generator 2] Check-boxes can now be toggled by clicking on their label
#### Modified
- [Hybrid Shader] Changed specular calculation for Stylized/Crisp types, so that it's visually more intuitive to change size/smoothness
- [Documentation] Updated layout and added missing "Specular" section for "Hybrid Shader"
#### Fixed
- [Hybrid Shader] Added lightmap support for URP and Built-in
- [Hybrid Shader] Reduced number of variants to compile when building when using the Outline version
- [Shader Generator 2] Fixed placement of modules functions block after variables declaration; fixes "No Tile" option for textures and possibly other modules
- [Shader Generator 2] (Default) (URP) Fixed "Unlit" ramp option that still needed a main directional light in the scene to work properly
- [Shader Generator 2] (URP) Fixed compilation error when using "VertExmotion" option

2.5.0
#### Added
- [TCP2 Hybrid Shader]: this is a new ubershader to replace the old Desktop/Mobile ones.
  It adds a few options that were lacking, e.g. transparency, and is compatible with both the *Built-In* and *Universal render pipelines*.
  Please read the documentation to know more!
- Added new demo scene: "Demo TCP2 Hybrid Shader"
- [Documentation] New updated documentation using the same format as the Shader Generator 2 documentation.
  Some parts have been rewritten, and some have been added (e.g. Hybrid Shader).
- [Smoothed Normals Utility] Updated the tool, now supporting "UV1", "UV3", "UV4" targets to store the normals, as well as storing them as "Full XYZ", "Compressed XY", "Compressed ZW" for UV targets.
- [Shader Generator 2] (Default) (URP) Added "Bands" options for the "Ramp Style" feature in "Lighting"

#### Modified
- `Desktop` and `Mobile` shaders have been moved into the "Shaders/Legacy/" folder, as well as their shader path ("Toony Colors Pro 2/Legacy/")
- `Standard PBS` shaders have been moved into the "Shaders/Standard PBS/" folder

2.4.5
#### Added
- [Shader Generator 2] (Default) (URP) Added "Wrapped Lighting" feature in "Lighting"
- [Shader Generator 2] (Default) (URP) Added "Shadow Line Strength" property for "Shadow Line", to help make larger lines
- [Shader Generator 2] (Default) (URP) Exposed sine parameters as Shader Properties for "Wind" animation
#### Fixed
- [Shader Generator 2] (Default) (URP) Fixed fog and vertical fog when used with "Outline"
- [Shader Generator 2] (Default) (URP) Fixed "Texture Splatting/Blending" error when using non-Linear blend and normal map
- [Shader Generator 2] (URP) Fixed compilation error when "Outline" and "Silhouette Pass" are both enabled without "SRP Batcher Compatibility"
- [Shader Generator 2] (URP) Fixed compilation error with "Hair Anisotropic" specular

2.4.4
#### Added
- [Shader Generator 2] (Default) (URP) Added "Wind Animation" feature in "Special Effects"
- [Shader Generator 2] (Default) (URP) Added "Hair Anisotropic" option for "Specular" feature
- [Shader Generator 2] (Default) (URP) Added "Fresnel Reflections" option for Reflections features
- [Shader Generator 2] (Default) (URP) Added "Unlit" option for "Ramp Style" feature
- [Shader Generator 2] (Default) (URP) Added "Fresnel Reflections" options for "Reflections"
- [Shader Generator 2] (URP) Added "Clamp Light Intensities" option when using "All Lights" shadow color shading: prevent received light from exceeding a defined value
- [Shader Generator 2] (URP) Added "View Dir Ambient Sampling", allowing to have a solid color with no gradations on the whole mesh when sampling ambient color/light probes
- [Shader Generator 2] (URP) Added "Special Implementation/Ambient Color" and "Indirect Diffuse" to sample those in other properties
- [Shader Generator 2] Added "Other Shader Property" UV option for textures: use any other property as the UV source of a texture; this can allow more complicated UV calculations, or use the same source for multiple textures
- [Shader Generator 2] Added "Constant Float" implementation for "Shader Properties"
- [Shader Generator 1] Added "Shadow Color Texture" feature for the "Surface PBS" template
- Added wind animation example in "Demo TCP2 ShaderGenerator 2" scene
- Updated the "Features Reference" part of the documentation
#### Fixed
- [Shader Generator 2] (URP) Fixed main light culling layers
- [Shader Generator 2] (URP) More SRP Batcher fixes
- [Shader Generator 2] (URP) Compilation error when using "World Position" UV
- [Shader Generator 2] (Default) (URP) Fixed Texture Blending with Normal Maps
- [Shader Generator 2] Fixed missing text in some dynamic tooltips
- [Shader Generator 2] Removed the "Fresnel" word from the Rim Lighting feature, as both words actually describe different things
- Added a warning regarding mesh orientation for TCP2_PlanarReflection script
- Fixed console warning because of a Font import settings

2.4.33.2
- [Shader Generator 2] (URP) Fixed possible compilation errors because of new "SRP Batcher Compatibility" option
- [Shader Generator 2] Minor UI fixes

2.4.33.1
- [Shader Generator 2] (URP) Fixed broken additional passes because of new "SRP Batcher Compatibility" option

2.4.33
- [Shader Generator 2] (Default) (URP) Added "Shadow Line" feature in Stylization, to create a crisp line between highlighted and shadowed part. Can be used to create comic-book style shadows for example.
- [Shader Generator 2] Added "World Position" UV option for textures: use the vertex world position as texture UV. This can allow different effects, like using a world-space texture blending map to control multiple objects (ground, grass), or add a scrolling clouds shadow map for example.
- [Shader Generator 2] (URP) Added "SRP Batcher Compatibility" in the Options (experimental, let me know if it doesn't work!)
- [Shader Generator 2] Modified Shader Properties are now retained when changing template; this should make it easier to convert a shader from the built-in pipeline to URP for example.
- Added a few Sketch textures
- [Shader Generator 2] Added ability to use a custom font in the Options
- [Shader Generator 2] Improved UI and readability, especially in Unity 2019.3+
- [Shader Generator 2] (URP) Fixed shadow coordinates calculation changes introduced in URP 7.2.0
- [Shader Generator 2] (URP) Fixed Meta Pass
- [Shader Generator 2] Fixed possible unwanted variable name changes when changing template on an existing shader.

2.4.32
- [Shader Generator 2] (Default) (LWRP/URP) Added "Shading Ramp" hook
- [Shader Generator 2] (URP) Added options for "Silhouette" and "Outline" to make them explicitly use the "URP Render Features" and updated documentation about it
- [Shader Generator 2] (Default) (LWRP/URP) Changed "smoothness" to "roughness" for Specular PBR/GGX
- [Shader Generator 2] (LWRP/URP) Added GPU instancing and stereo rendering support
- [Shader Generator 2] (LWRP/URP) Fixed error when using "Vertical Fog" and "Enable Fog" in the same shader
- [Shader Generator 1] Fixed visual bug with spot light falloff bypass

2.4.31
- Added `Cat Demo URP` scene (extract it from the `Cat Demo URP.unitypackage` file)
- [Shader Generator 2] (LWRP/URP) Fixed compilation error when using Specular GGX along with Reflection Probes
- [Shader Generator 2] Fixed frequent compilation errors due to the new custom code prepend option
- [Shader Generator 2] Reloading shaders from the current output directory now works
- [Shader Generator 2] Fixed bug that was showing all Shader Properties as modified when copying a shader
- [Shader Generator 2] Performance optimizations in the UI
- Hopefully fixed mesh references that get lost when importing unitypackage in Cat demo scene

2.4.3
- [Shader Generator 2] Added 'Prepend Code' option for Custom Code implementation: add arbitrary code before the implementation to allow for more complex code insertion
- [Shader Generator 2] (Default) (LWRP/URP) Added "Alpha to Coverage" option for "Alpha Testing"
- [Shader Generator 2] (Default) Added "Aura 2" third-party support (experimental)
- [Shader Generator 2] Add Unity version on generated shaders, will be useful for support
- [Shader Generator 2] Changed how hash is calculated: old shaders opened in the tool will show a warning that they are modified externally, even though they are not. The warning will disappear once the shader is updated.
- [Shader Generator] (Water) Enabled 'Add Shadow Pass' option to enable shadow receiving for water shaders (note: not compatible with depth-buffer effects)
- [Shader Generator 2] Fixed UI bug that prevented some options from showing if `Show disabled fields` was disabled
- [Shader Generator 2] Fixed possible compilation error with texture coordinates
- [Shader Generator 2] Fixed "Vertical Fog" that was reapplied when using additional lights
- "Demo TCP2 Cat LWRP" scene: water shader now also works with orthographic cameras
- Fixed possible corrupted shaders when unpacking from the menu

2.4.2
- [Shader Generator 2] (Default) (LWRP/URP) Added "Vertical Fog" special effect
- [Shader Generator 2] (LWRP/URP) Fixed shader compilation error when using features with world-space view direction (e.g. "Specular")
- [Shader Generator 2] Fixed shader compilation error when using `Min Max Threshold` ceiling mode with "Triplanar Mapping"
- [Shader Generator 2] `Custom Code` implementation: fixed reference bug and improved UI on referenced implementations
- [Shader Generator 2] Fixed tiling and scrolling calculation order which could result in animation pops for Texture implementations
- [Shader Generator 1] Fixed issues with non-US culture when using C# 4.x (when using constant float values in Shader Generator 1)
- [Shader Generator 2] Fixed issue that produced a warning for generated shaders
- Cat Demo Scene: fixed mesh references in Unity 2019.1+

2.4.1
- As of version 2.4.0, the "Shaders 2.0" folder has been renamed to "Shaders".
  Any shaders and files in "Shaders 2.0" or a sub-folder (e.g. "Variants") has to be moved there for the shaders to compile properly.
- Added HTML formatted changelog
- Fixed packed shaders that were unpacking in the wrong folder, causing an #include error

2.4.0
- Added "Shader Generator 2" beta: more flexible tool, with a fully-featured Lightweight/Universal Pipeline template
See the documentation to learn more!
- Added "Cat Demo LWRP" scene (extract it from the "Cat Demo LWRP.unitypackage" file)
- Shader Generator: Added "VertExmotion" support (under "Special Effects")
- Shader Generator: Enabled Dithered Shadows when using Alpha Testing with Dithered Transparency
- Shader Generator: fixed Outline in Single-Pass Stereo rendering mode (VR)
- Added 26 MatCap textures
- Reorganized the Textures folder
- Renamed the "Shaders 2.0" folder to "Shaders"
- Added namespaces for all TCP2 classes (except material inspectors for compatibility)
- Added .asmdef files for TCP2 scripts (Unity 2019.1+)

2.3.572
- Fixed compilation error on MacOS
- Fixed issues with non-US culture when using C# 4.x
- Regression fixed: Shader Generator: "Constant Size Outline" was broken, will take objects' scale into account again

2.3.571
- Shader Generator: "Default" template: fixed Specular Mask when using PBR Blinn-Phong or GGX models
- Shader Generator: "PBS" templates: fixed compilation issues on builds when using outlines
- Shader Generator: "Surface PBS" template: fixed Emission feature
- Shader Generator: "Water" template: fixed precision issue causing artifacts on some mobile platforms
- Shader Generator: fixed "Constant Size Outline" option so that it ignores objects' scale
- Shader Generator: UI fixes with inline features

2.3.57
- Shader Generator: upgraded "SRP/Lightweight" template to latest version (4.1.0-preview with Unity 2018.3.0b9)
- Shader Generator: "Default" template: fixed "Pixel MatCap" option when using a normal map with skinned meshes
- Shader Generator: always start with a new shader when opening the tool (don't load the last generated/loaded shader anymore)
- Added example MatCap textures
- Removed 'JMOAssets.dll', became obsolete with the Asset Store update notification system

2.3.56
- Shader Generator: added "Texture Blending" feature for "Surface PBS" template
- Shader Generator: fixed non-repeating tiling for "Texture Blending" in relevant templates
- Shader Generator: fixed masks for "Surface PBS" template (Albedo Color Mask, HSV Mask, Subsurface Scattering Mask)
- Added default non-repeating tiling noise textures

2.3.55
- Shader Generator: added "Silhouette Pass" option: simple solid color silhouette for when objects are behind obstacles
- Shader Generator: fixed fog for "Standard PBS" shaders in Unity 2018.2+
- Reorganized some files and folders

2.3.54
- Shader Generator: added more Tessellation options for "Default" template: Fixed, Distance Based and Edge Length Based
- Shader Generator: added Tessellation support for "Standard PBS" template
- Desktop/Mobile shaders: removed Directional Lightmap support for shaders, so that all variants can compile properly (max number of interpolators was reached for some combination in Unity 2018+)
- Mpbile shaders: disabled view direction calculated in the vertex shader, will be calculated in the fragment instead, so that all variants compile properly (slightly slower but it frees up one interpolator)
- Shader Generator: restored 'VertexLit' fallback for Surface PBS template, so that shadow receiving works by default

2.3.53
- Shader Generator: added "Shadow Color Mode" feature with "Replace Color" option to entirely replace the Albedo with the shadow color
- Shader Generator: updated GGX Specular to be more faithful to the Standard shader implementation
- Shader Generator: fixed GGX Specular when using Linear color space
- Shader Generator: updated Lightweight SRP template to work with Unity 2018.2 and lightweight 2.0.5-preview
- Shader Generator: Lightweight SRP template still works with Unity 2017.1 and lightweight 1.1.11-preview

2.3.52
- Shader Generator: added "Vertical Fog" option for "Default" template
- Shader Generator: fixed wrong colors and transparency in "Fade" mode with "Surface Standard PBS" template
- Shader Generator: fixed disabling depth buffer writing mode depending on transparency mode with "Surface Standard PBS" template
- Shader Generator: reorganized templates UI in foldout boxes for clarity
- Shader Generator: updated UI for clarity
- Shader Generator: harmonized UI colors for Unity Editor pro skin

2.3.51
- Shader Generator: fixed issue with "Sketch" option in "Surface Standard PBS" template
- Shader Generator: fixed "Bump Scale" option in "SRP/Lightweight" template

2.3.5
- Shader Generator: added experiment "SRP/Lightweight" template with limited features (Unity 2018.1+)
	- extract "Lightweight SRP Template.unitypackage" for it to work
- Shader Generator: added "LOD Group Blending" feature (dither or cross-fade)
- script warning fix

See full and formatted changelog here: https://jeanmoreno.com/unity/toonycolorspro/doc/changelog
