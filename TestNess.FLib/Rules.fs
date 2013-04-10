namespace FLib.Rules
open Mono.Cecil
open Mono.Cecil.Cil
open System.Linq
open System
open FLib

module private Helpers =
    [<Literal>]
    let HiddenLine = 0xfeefee

    let tfTransition (x: bool[]) = x.[0] = true && x.[1] = false
    let methodHasName (name: string) (m: MethodDefinition) = m.FullName.Equals(name)

type IRule =
    abstract member Apply : TestCase -> seq<Violation>

and Violation(rule: IRule, tc: TestCase, message: string) =
    new(rule: IRule, tc: TestCase) = Violation(rule, tc, null)


type AssertDistributionRule() =

    let IsSignificantInstruction (ins: Instruction) =
        match ins.OpCode with
        | x when x = OpCodes.Nop -> false
        | x when x = OpCodes.Ret && ins.Next = null -> false
        | _ -> ins.SequencePoint <> null && ins.SequencePoint.StartLine <> Helpers.HiddenLine

    let IsAssertCall (assertingMethods: seq<MethodDefinition>) (ins: Instruction) =
        match ins.OpCode.FlowControl with
        | FlowControl.Call ->
            match ins.Operand with
            | :? MethodReference as mref -> Seq.exists (Helpers.methodHasName mref.FullName) assertingMethods
            | _ -> false
        | _ -> false

    interface IRule with
        member this.Apply testCase =
            match testCase.TestMethod.DeclaringType.Module.HasSymbols with
            | true ->
                let assertingMethods = testCase.GetCalledAssertingMethods()
                let asserts =
                    testCase.TestMethod.Body.Instructions
                        |> Seq.filter(IsSignificantInstruction)
                        |> Seq.map(IsAssertCall assertingMethods)
                // Find the first true->false transition and map it to a Violation
                Seq.windowed 2 asserts |> Seq.filter(Helpers.tfTransition) |> Seq.take 1 |> Seq.map(fun x -> new Violation(this, testCase))
            | false -> Seq.empty

    override this.ToString() =
        "all asserts in a test case should be placed together last in the test method"


type LimitAssertsPerTestCaseRule() =
    let mutable maxAsserts = 1
    let CreateViolationMessage assertMethodCount = 
        String.Format("test case contains {0} asserts (limit is {1})", assertMethodCount, maxAsserts);

    /// <summary>
    /// The maximum number of acceptable asserts that a test case can contain. By default, this value
    /// is 1. It cannot be set to a values less than 1.
    /// </summary>
    member this.MaxNumberOfAsserts
        with get() = maxAsserts
        and set(value) =
            match value with
            | x when x >= 1 -> maxAsserts <- value
            | _ -> raise (System.ArgumentException("Value must be >= 1"))

    interface IRule with

        member this.Apply testCase =
            let assertMethodCount = Seq.length(testCase.GetCalledAssertingMethods())
            match assertMethodCount with
            | x when 0 < x && x <= maxAsserts -> Seq.empty
            | x when x = 0 && TestFrameworks.Instance.HasExpectedException(testCase.TestMethod) -> Seq.empty
            | _ -> seq { yield new Violation(this, testCase, CreateViolationMessage(assertMethodCount)) }

    override this.ToString() =
        let range = if maxAsserts = 1 then "1" else ("1 to " + maxAsserts.ToString())
        let plural = if maxAsserts = 1 then "" else "s"
        String.Format("a test case should have {0} assert{1} or expect an exception", range, plural)
