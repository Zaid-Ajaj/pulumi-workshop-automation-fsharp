[<AutoOpen>]
module Extensions 

let inline outputs (content: (string * obj) list) = dict content
let (==>) (key: string) (value: obj) = key, value
