namespace FLib
open Mono.Cecil
open System

type ParameterPurpose = 
    | Expected = 0
    | Actual = 1
    | ExpectedOrActual = 2
    | Unknown = 3
    | MetaData = 4

/// <summary>
/// Defines the interface that TestNess uses to discover and inspect test cases implemented
/// using a particular test framework.
/// </summary>
type ITestFramework =
    abstract member IsTestMethod: MethodDefinition -> bool
    abstract member HasExpectedException: MethodDefinition -> bool
    abstract member DoesContainAssertion: MethodDefinition -> bool

    /// <summary>
    /// Returns a list of parameter purposes corresponding to the parameters of the given method.
    /// If the method is not recognized, the return value is <c>null</c> (as opposed to the empty
    /// list, which is returned for a recognized method with no parameters).
    /// </summary>
    /// <param name="method">Supposedly an asserting method from this framework.</param>
    /// <returns>A list of parameter purposes.</returns>
    abstract member GetParameterPurposes: MethodReference -> list<ParameterPurpose> option

    /// <summary>
    /// Determines if the given method is a method that accesses data for a data-driven test.
    /// MSTest uses <c>TestContext.DataRow</c> rather than method parameters for publishing
    /// data from a data source. For this reason, we need a way to check if a method call really
    /// is a way to access data in a data-driven test.
    /// </summary>
    /// <param name="method">The method being called.</param>
    /// <returns>A boolean value indicating if the method is an accessor method for data in a
    /// data-driven test.</returns>
    abstract member IsDataAccessorMethod: MethodReference -> bool

/// <summary>
/// This class merges a number of <see cref="ITestFramework" /> implementations while implementing said
/// interface itself. Thus, it acts as a facade for a number of test frameworks.
/// </summary>
type TestFrameworks() as this =

    let mutable frameworks: list<ITestFramework> = []

    let newTestFramework (t: Type) = Activator.CreateInstance(t) :?> ITestFramework
    let isTestFramework (t: Type) = 
        t.IsPublic && typedefof<ITestFramework>.IsAssignableFrom(t) && not t.IsInterface && not t.IsAbstract && t <> this.GetType()

    do
        frameworks <- this.GetType().Assembly.GetTypes() |> Array.filter(isTestFramework) |> Array.map(newTestFramework) |> Array.toList

    static member Instance: TestFrameworks = new TestFrameworks()

    interface ITestFramework with

        member this.IsTestMethod meth = frameworks |> List.exists (fun f -> f.IsTestMethod(meth))
        member this.HasExpectedException meth = frameworks |> List.exists (fun f -> f.HasExpectedException(meth))
        member this.DoesContainAssertion meth = frameworks |> List.exists (fun f -> f.DoesContainAssertion(meth))
        member this.IsDataAccessorMethod meth = frameworks |> List.exists (fun f -> f.IsDataAccessorMethod(meth))
        member this.GetParameterPurposes meth = 
//            let rec getter (flist: list<ITestFramework>) =
//                match flist with
//                | f :: tail ->
//                    let x = f.GetParameterPurposes(meth)
//                    match x with
//                    | Some(_) -> x
//                    | None -> getter(tail)
//                | [] -> None
//            getter(frameworks)
            frameworks |> Seq.ofList |> Seq.map(fun f -> f.GetParameterPurposes(meth)) |> Seq.tryPick id
