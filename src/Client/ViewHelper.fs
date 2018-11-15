module ViewHelper

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.PowerPack
open Elmish.Browser.Navigation
open Thoth.Elmish
open Fulma
open Fulma.FontAwesome

open Shared
open System

let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore
let onEnter msg dispatch =
    function
    | (ev:React.KeyboardEvent) when ev.keyCode = Keyboard.Codes.enter ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown


// Thoth.Elmish.Toastのレンダラ　https://github.com/MangelMaxime/Thoth/blob/master/demos/Thoth.Elmish.Demo/src/Toast.fs
let renderToastWithFulma =
    { new Toast.IRenderer<Fa.I.FontAwesomeIcons> with
        member __.Toast children color =
            Notification.notification [ Notification.CustomClass color ]
                children
        member __.CloseButton onClick =
            Notification.delete [ Props [ OnClick onClick ] ]
                [ ]
        member __.InputArea children =
            Columns.columns [ Columns.IsGapless
                              Columns.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                              Columns.CustomClass "notify-inputs-area" ]
                children
        member __.Input (txt : string) (callback : (unit -> unit)) =
            Column.column [ ]
                [ Button.button [ Button.OnClick (fun _ -> callback ())
                                  Button.Color IsWhite ]
                    [ str txt ] ]
        member __.Title txt =
            // カスタマイズ(txtがブランクの場合は領域を取らないように)
            if System.String.IsNullOrEmpty(txt) then
                ofOption None
            else
                Heading.h5 []
                    [ str txt ]
        member __.Icon (icon : Fa.I.FontAwesomeIcons) =
            Icon.faIcon [ Icon.Size IsMedium ]
                [ Fa.icon icon
                  Fa.fa2x ]
        member __.SingleLayout title message =
            div [ ]
                [ title; message ]
        member __.Message txt =
            span [ ]
                [ str txt ]
        member __.SplittedLayout iconView title message =
            Columns.columns [ Columns.IsGapless
                              Columns.IsVCentered ]
                [ Column.column [ Column.Width (Screen.All, Column.Is2) ]
                    [ iconView ]
                  Column.column [ ]
                    [ title
                      message ] ]
        member __.StatusToColor status =
            match status with
            | Toast.Success -> "is-success"
            | Toast.Warning -> "is-warning"
            | Toast.Error -> "is-danger"
            | Toast.Info -> "is-info" }