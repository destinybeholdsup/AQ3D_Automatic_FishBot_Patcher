// Install Mono.Cecil 
// Put the exe + Mono.Cecil.dll into aq3d managed folder and run.

public static void Fishing(string assemblyPath = "Assembly-CSharp.dll", string outputPath = "Assembly-CSharp FISH.dll")
{
    if (!File.Exists(assemblyPath))
    {
        Console.WriteLine("ERROR: Assembly-CSharp.dll not found!");
        return;
    }

    Console.WriteLine("Loading assembly...");
    var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadWrite = true });
    var module = assembly.MainModule;
    Console.WriteLine("Assembly loaded successfully.");

    Console.WriteLine("Searching for UIFishing class...");
    var uiFishingType = module.Types.FirstOrDefault(t => t.Name == "UIFishing");
    if (uiFishingType == null)
    {
        Console.WriteLine("ERROR: UIFishing class not found!");
        return;
    }
    Console.WriteLine("UIFishing class found.");

    Console.WriteLine("Searching for Update() method in UIFishing...");
    var updateMethod = uiFishingType.Methods.FirstOrDefault(m => m.Name == "Update");
    if (updateMethod == null || !updateMethod.HasBody)
    {
        Console.WriteLine("ERROR: Update method not found or has no body!");
        return;
    }
    Console.WriteLine("Update method found.");

    var processor = updateMethod.Body.GetILProcessor();
    var firstInstruction = updateMethod.Body.Instructions.FirstOrDefault();
    if (firstInstruction == null)
    {
        Console.WriteLine("ERROR: Update method has no instructions!");
        return;
    }
    Console.WriteLine("Located first instruction of Update().");

    Console.WriteLine("Searching for ResourceMachine class...");
    var machineType = module.Types.FirstOrDefault(t => t.Name == "ResourceMachine");
    if (machineType == null)
    {
        Console.WriteLine("ERROR: ResourceMachine class not found!");
        return;
    }
    Console.WriteLine("ResourceMachine class found.");

    Console.WriteLine("Searching for CollectResource() method in ResourceMachine...");
    var collectResourceMethod = machineType.Methods.FirstOrDefault(m => m.Name == "CollectResource");
    if (collectResourceMethod == null)
    {
        Console.WriteLine("ERROR: CollectResource method not found in ResourceMachine!");
        return;
    }
    Console.WriteLine("CollectResource method found.");

    Console.WriteLine("Searching for 'machine' field in UIFishing...");
    var machineField = uiFishingType.Fields.FirstOrDefault(f => f.Name == "machine");

    if (machineField == null)
    {
        Console.WriteLine("ERROR: machine field not found in UIFishing!");
        return;
    }
    Console.WriteLine("machine field found.");

    Console.WriteLine("Searching for IsInGoal() method in UIFishing...");
    var isInGoalMethod = uiFishingType.Methods.FirstOrDefault(m => m.Name == "IsInGoal");

    if (isInGoalMethod == null)
    {
        Console.WriteLine("ERROR: IsInGoal method not found!");
        return;
    }
    Console.WriteLine("IsInGoal method found.");

    Console.WriteLine("Searching for Close() method in UIFishing...");
    var closeMethod = uiFishingType.Methods.FirstOrDefault(m => m.Name == "Close");

    if (closeMethod == null)
    {
        Console.WriteLine("ERROR: Close method not found!");
        return;
    }
    Console.WriteLine("Close method found.");

    Console.WriteLine("Injecting IL code...");

    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldarg_0));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Call, isInGoalMethod));
    var skipInstruction = processor.Create(OpCodes.Nop);
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Brfalse, skipInstruction));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldarg_0));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldfld, machineField));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Callvirt, collectResourceMethod));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Ldarg_0));
    processor.InsertBefore(firstInstruction, processor.Create(OpCodes.Call, closeMethod));
    processor.InsertBefore(firstInstruction, skipInstruction);

    Console.WriteLine("Saving patched assembly...");
    assembly.Write(outputPath);
    Console.WriteLine($"Patched assembly saved as {outputPath}");

    MessageBox(IntPtr.Zero, "Patch completed successfully!", "Success", 0x0);
}
