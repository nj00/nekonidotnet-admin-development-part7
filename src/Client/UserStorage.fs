module App.UserStorage

open Fable.PowerPack
open Elmish
open Shared.Auth
open Thoth.Json

[<Literal>]
let StorageKey = "user"

let load () : UserData option =
    let userDecoder = Decode.Auto.generateDecoder<UserData>()
    match BrowserLocalStorage.load userDecoder StorageKey with
    | Ok user -> Some user
    | Error _ -> None

let save user =
    BrowserLocalStorage.save StorageKey user

let remove () =
    BrowserLocalStorage.delete StorageKey

