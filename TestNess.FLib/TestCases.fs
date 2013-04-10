namespace FLib
open Mono.Cecil
open Mono.Cecil.Pdb
open System.Linq
open System
open System.IO
open System.Reflection
open System.Collections.Generic

type TestCaseOrigin(assembly: AssemblyDefinition, fileName: string) =
    member this.Assembly = assembly
    member this.AssemblyFileName = fileName

type TestCase(meth: MethodDefinition, origin: TestCaseOrigin) = 
    member this.GetCalledAssertingMethods(): seq<MethodDefinition> =
        Seq.empty<MethodDefinition>
    member this.TestMethod
        with get(): MethodDefinition = null

type TestCases(assembly: AssemblyDefinition, fileName: string) as self =

    let mutable _enumerable = null
    do
        if assembly.Modules.Count > 1 then raise (NotImplementedException("Multi-module assemblies not supported yet!"))
        _enumerable <- self.createEnumerable()

    member private this.createEnumerable() =
        assembly.MainModule.Types
            |> Seq.collect(fun t -> t.Methods)
            |> Seq.filter(fun m -> true)
            |> Seq.map(fun m -> new TestCase(m, new TestCaseOrigin(assembly, fileName)))

    interface Collections.IEnumerable with member x.GetEnumerator () = _enumerable.GetEnumerator() :> _
    interface Collections.Generic.IEnumerable<TestCase> with member x.GetEnumerator () = _enumerable.GetEnumerator()

    static member FromAssembly(assembly: Assembly) =
        let uri = new Uri(assembly.CodeBase)
        TestCases.LoadFromFile(uri.LocalPath)

    static member LoadFromFile(fn: string) =
        let parameters = new ReaderParameters()
        parameters.SymbolReaderProvider <- new PdbReaderProvider()
        parameters.ReadingMode <- ReadingMode.Immediate
        let mutable assemblyDef: AssemblyDefinition = null
        try
            assemblyDef <- AssemblyDefinition.ReadAssembly(fn, parameters)
        with
            | :? FileNotFoundException -> assemblyDef <- AssemblyDefinition.ReadAssembly(fn)
        new TestCases(assemblyDef, fn)


