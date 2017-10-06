using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SDX.Compiler;

public class LogicGatesPatcher : IPatcherMod
{



    public bool Patch(ModuleDefinition module)
    {
        Console.WriteLine("==LogicGates Patch==");

		// var pt= module.Types.First(d => d.Name == "PowerTrigger");
		//var field = pt.Fields.First(d=> d.Name == "TriggerPowerDurationTypes");


        return true;
    }
	
	// Called after the patching process and after scripts are compiled.
    // Used to link references between both assemblies
    // Return true if successful
    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        return true;
    }
	
}
	