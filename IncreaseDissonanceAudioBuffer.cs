using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using MelonLoader;
using HarmonyLib;
using Dissonance;
using NAudio.Wave;

[assembly: MelonInfo(typeof(IncreaseDissonanceAudioBuffer.IncreaseDissonanceAudioBufferMod), "Increase Dissonance Audio Buffer", "1.1.0", "happysmash27")]
[assembly: MelonGame(null, null)]

namespace IncreaseDissonanceAudioBuffer
{
    public class IncreaseDissonanceAudioBufferMod : MelonMod
    {
	static int targetMinSampleCount = 1024;
	static bool flexible = true;
	
	[HarmonyPatch("Dissonance.Audio.Capture.BasePreprocessingPipeline, DissonanceVoip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "BasePreprocessingPipeline", MethodType.Constructor)]
	[HarmonyPatch( new Type[] { typeof(WaveFormat), typeof(int), typeof(int), typeof(int), typeof(int) })]
	    class Constructor {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
		    var codes = new List<CodeInstruction>(instructions);

		    MelonLogger.Msg("Scanning for instantiations of Dissonance.Audio.Capture.BufferedSampleProvider where the last argument is multiplied by a number smaller than targetMinSampleCount...");
		    var BSPConstructor = AccessTools.TypeByName("Dissonance.Audio.Capture.BufferedSampleProvider").GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new [] { typeof(WaveFormat), typeof(int) }, null);
		    
		    for(int i=0; i<codes.Count; i++){
			if (codes[i].opcode == OpCodes.Newobj && codes[i].operand.Equals(BSPConstructor) && i-2>=0 &&
			    codes[i-1].opcode == OpCodes.Mul)
			{
			    if (codes[i-2].opcode == OpCodes.Ldc_I4_S &&
				(sbyte)codes[i-2].operand < targetMinSampleCount &&
				(flexible == true || (sbyte)codes[i-2].operand == 16))
			    {
				MelonLogger.Msg("Found instantiation multiplying last argument by sbyte from Ldc_I4_S!");
				MelonLogger.Msg("Current sample count: "+(sbyte)codes[i-2].operand);

				if (targetMinSampleCount>sbyte.MaxValue){
				    codes[i-2].opcode = OpCodes.Ldc_I4;
				}
				codes[i-2].operand = targetMinSampleCount;

				MelonLogger.Msg("Patched sample amount to "+(int)codes[i-2].operand);
			    } else if (flexible == true && codes[i-2].opcode == OpCodes.Ldc_I4 &&
				       (sbyte)codes[i-2].operand < targetMinSampleCount)
			    {
				MelonLogger.Msg("Found instantiation multiplying last argument by int from Ldc_I4!");
				MelonLogger.Msg("Current sample count: "+(int)codes[i-2].operand);

				codes[i-2].operand = targetMinSampleCount;

				MelonLogger.Msg("Patched sample amount to "+(int)codes[i-2].operand);
			    }
			}
			
		    }

		    return codes.AsEnumerable();
		
		}
	    }
    }
}
