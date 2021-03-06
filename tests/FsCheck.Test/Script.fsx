﻿open System
open System.Reflection
open Microsoft.FSharp.Reflection

#r "bin\Debug\FsCheck.dll"
open FsCheck

//how to generate float between 0 and 5
//let f = 
//    Arb.generate<NormalFloat>
//    |> Gen.map float
//    |> Gen.resize 5
//    |> Gen.map abs
//    |> Gen.suchThat (fun f -> f < 5.)
//    |> Gen.sample 100 10

type Arbs =
  static member SafeString () =
    Arb.Default.String () |> Arb.filter (fun str -> str <> null && not (str = ""))

  static member NonEmptyStringMaps () =
    Arb.Default.Map () |> Arb.filter (fun m -> m |> Map.toList |> List.forall (fun (k,v) -> k <> "" && k <> null))

  static member DateTime () =
    Arb.Default.DateTime ()
    |> Arb.mapFilter (fun dt -> dt.ToUniversalTime()) (fun dt -> dt.Kind = System.DateTimeKind.Utc)

let fsCheckConfig = { Config.Default with Arbitrary = [ typeof<Arbs> ] }

Check.One(fsCheckConfig,(fun (s:string,m:Map<string,int>) -> not <| String.IsNullOrEmpty s && m |> Map.forall (fun s v -> not <| String.IsNullOrEmpty s)))

Check.One(fsCheckConfig,(fun (s:DateTime) -> s.Kind = DateTimeKind.Utc))

//Gen.sample 10 1 <| Arbs.DateTime().Generator
