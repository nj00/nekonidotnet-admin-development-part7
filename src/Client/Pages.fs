module Pages

open Elmish.Browser.UrlParser

[<RequireQualifiedAccess>]
type Page =
  | Home
  | Counter
  | Janken

let toPageUrl page =
  match page with
  | Page.Home -> "#home"
  | Page.Counter -> "#counter"
  | Page.Janken -> "#janken"

let pageParser: Parser<Page->Page,_> =
    oneOf [
        map Page.Home (s "home")
        map Page.Counter (s "counter")
        map Page.Janken (s "janken")
    ]
// let urlParser location = parsePath pageParser location
let urlParser location = (parseHash pageParser) location
