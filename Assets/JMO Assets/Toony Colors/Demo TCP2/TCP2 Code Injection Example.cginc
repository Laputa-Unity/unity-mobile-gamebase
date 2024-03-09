/// Comments beginning with three slashes are specific to the injection file.
/// They won't be included in the resulting .shader file.
/// I have indented these comments below for readability, they will help you understand each available special comment.

///         Define a code block using the '//# BLOCK: ' prefix followed by the block name.
///         The block name can be anything you want, it is only used for the user interface.
//# BLOCK: Define Custom Properties
///         The '//# Inject @ ' prefix will tell the Shader Generator 2 to automatically inject this block at the specified injection point, if found in the template.
///         The easiest way to find the available injection points for your shader is to enable the 'Mark injection points' setting
///         in the CODE INJECTION tab, and then look for '// Injection Point : [name]' comments in the resulting .shader file.
//# Inject @ Properties/Start
///         All the following lines will be inserted in the .shader file at the injection point location, including comments with two slashes '//'
[Toggle(INJECTED_SHADER_FEATURE)] _InjectedShaderFeature ("Enable Injected Color", Float) = 0.0
_InjectedColor ("Injected Color", Color) = (1.0, 1.0, 0.0, 1.0)
///         Some material decorator drawers are included with Toony Colors Pro 2, you can freely use them like this separator
[TCP2Separator]

///         The current BLOCK will end when a new command starts, or the end of the file is reached.
///         Any additional line breaks between the two will be trimmed, so you can freely add more for readability.


//# BLOCK: Declare Custom Properties
//# Inject @ Variables/Inside CBuffer
half4 _InjectedColor;


//# BLOCK: Custom Shader Features
//# Inject @ Main Pass/Pragma
#pragma shader_feature INJECTED_SHADER_FEATURE


//# BLOCK: Add Pulse Color to Albedo
//# Inject @ Main Pass/Surface Function/End
///         We can define custom Shader Properties here, similar to the ones in the SHADER PROPERTIES tab of the Shader Generator 2.
///         Simply add one line per property with:
///           float(N) propertyName defaultValue(optional)
///         You can then simply use them in your code with their name, and they will be replaced automatically in the final .shader file.
//# float pulseSpeed 4.0

#if defined(INJECTED_SHADER_FEATURE)
///         We know that these variables do exist in the .shader file, so we can freely alter them
	half4 pulsingInjectedColor = _InjectedColor * ((sin(_Time.y * pulseSpeed) + 1) / 2.0);
	output.Albedo += pulsingInjectedColor.rgb;
#endif

///         The '//# REPLACE:' line defines a line to replace in the final .shader, so that you can override or append code to about anywhere in your .shader file.
///         Note: searching for the line to replace only works on a single line; however you can replace it with multiple lines.
//# REPLACE: Disable Ambient
            #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
///         The '//# WITH:' defines the lines with which to replace the found line.
///         You can re-add the original search line if you only want to append lines to it.
//# WITH:
            #if false
///         The '//# INFO: ' prefix defines additional information for this replacement that will show in the Shader Generator 2.
//# INFO: Disable applying indirect diffuse (ambient color)