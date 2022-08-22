using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using MelonLoader;
using HarmonyLib;
using Dissonance;
using NAudio.Wave;

[assembly: MelonInfo(typeof(IncreaseDissonanceAudioBuffer.IncreaseDissonanceAudioBufferMod), "Increase Dissonance Audio Buffer", "1.0.0", "happysmash27")]
[assembly: MelonGame(null, null)]

namespace IncreaseDissonanceAudioBuffer
{
    public class IncreaseDissonanceAudioBufferMod : MelonMod
    {
	[HarmonyPatch("Dissonance.Audio.Capture.BasePreprocessingPipeline, DissonanceVoip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "BasePreprocessingPipeline", MethodType.Constructor)]
	[HarmonyPatch( new Type[] { typeof(WaveFormat), typeof(int), typeof(int), typeof(int), typeof(int) })]
	    class Constructor {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
		    var codes = new List<CodeInstruction>(instructions);

		    MelonLogger.Msg("Scanning for instantiations of Dissonance.Audio.Capture.BufferedSampleProvider where the last argument is multiplied by 16...");
		    var BSPConstructor = AccessTools.TypeByName("Dissonance.Audio.Capture.BufferedSampleProvider").GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new [] { typeof(WaveFormat), typeof(int) }, null);

		    //For every instantiation of Dissonance.Audio.Capture.BufferedSampleProvider where the last argument is multiplied by 16 (loaded in per Ldc_I4_S), multiply it by 256 (loaded per Ldc_I4) instead
		    for(int i=0; i<codes.Count; i++){
			if (codes[i].opcode == OpCodes.Newobj && codes[i].operand.Equals(BSPConstructor) && i-2>=0 &&
			    codes[i-1].opcode == OpCodes.Mul &&
			    codes[i-2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i-2].operand == 16)
			{
			    MelonLogger.Msg("Found instantiation!");
			    MelonLogger.Msg("Current sample count: "+(sbyte)codes[i-2].operand);

			    codes[i-2].opcode = OpCodes.Ldc_I4;
			    codes[i-2].operand = 256;

			    MelonLogger.Msg("Patched sample amount to "+(int)codes[i-2].operand);

			    //An older version of this code used `break;` at the end of this loop to stop at the first instance of this line of code
			    //Newer versions, however, try to be flexible by patching ANY code that matches the pattern. So if newer lines are added that also multiply the size of the buffer by 16, they will all be patched too.
			    //Uncomment this line to re-enable the old functionality
			    //break;
			}
		    }

		    return codes.AsEnumerable();
		
		}
	    }
    }
}
