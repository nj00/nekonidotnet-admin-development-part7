module Home.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome

open Types

let root model dispatch =
  form [] [
    Field.div [ ]
        [ Label.label [ ] [ str "Username" ]
          Control.div [ Control.HasIconLeft
                        Control.HasIconRight ]
            [ Input.text [ Input.Color IsSuccess
                           Input.DefaultValue ""
                           Input.OnChange (fun ev -> !!ev.target?value |> ChangeStr |> dispatch )]
              Icon.faIcon [ Icon.Size IsSmall; Icon.IsLeft ] [ Fa.icon Fa.I.User ]
              Icon.faIcon [ Icon.Size IsSmall; Icon.IsRight ] [ Fa.icon Fa.I.Check ] ]
          Help.help [ Help.Color IsSuccess ]
            [ str (sprintf "Hello %s" model) ] ]
  ]
