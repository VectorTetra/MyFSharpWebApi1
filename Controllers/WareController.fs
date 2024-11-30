namespace MyWebApi.Controllers

open System.Collections.Generic
open Microsoft.AspNetCore.Mvc

// Тип моделі "Ware" для демонстрації
type Ware =
    { Id: int
      Name: string
      Description: string
      Price: decimal
      InStock: bool }

// Контролер
[<ApiController>]
[<Route("api/[controller]")>]
type WareController() =
    inherit ControllerBase()

    // Зразкові дані для тестування
    let mutable wares =
        [ { Id = 1
            Name = "Товар 1"
            Description = "Опис товару 1"
            Price = 100.50M
            InStock = true }
          { Id = 2
            Name = "Товар 2"
            Description = "Опис товару 2"
            Price = 250.00M
            InStock = false }
          { Id = 3
            Name = "Товар 3"
            Description = "Опис товару 3"
            Price = 300.75M
            InStock = true } ]

    // Отримати всі товари
    [<HttpGet>]
    member this.GetAllWares() = this.Ok(wares) :> IActionResult

    // Отримати товар за ID
    [<HttpGet("{id}")>]
    member this.GetWareById(id: int) =
        match wares |> List.tryFind (fun w -> w.Id = id) with
        | Some ware -> this.Ok(ware) :> IActionResult
        | None -> this.NotFound($"Товар із ID {id} не знайдено") :> IActionResult

    // Додати новий товар
    [<HttpPost>]
    member this.AddWare([<FromBody>] newWare: Ware) =
        if wares |> List.exists (fun w -> w.Id = newWare.Id) then
            this.Conflict($"Товар із ID {newWare.Id} вже існує") :> IActionResult
        else
            wares <- newWare :: wares
            this.CreatedAtAction("GetWareById", { newWare with Id = newWare.Id }, newWare) :> IActionResult

    // Оновити існуючий товар
    [<HttpPut("{id}")>]
    member this.UpdateWare(id: int, [<FromBody>] updatedWare: Ware) =
        match wares |> List.tryFind (fun w -> w.Id = id) with
        | Some _ ->
            wares <- wares |> List.map (fun w -> if w.Id = id then updatedWare else w)
            this.NoContent() :> IActionResult
        | None -> this.NotFound($"Товар із ID {id} не знайдено") :> IActionResult

    // Видалити товар
    [<HttpDelete("{id}")>]
    member this.DeleteWare(id: int) =
        match wares |> List.tryFind (fun w -> w.Id = id) with
        | Some _ ->
            wares <- wares |> List.filter (fun w -> w.Id <> id)
            this.NoContent() :> IActionResult
        | None -> this.NotFound($"Товар із ID {id} не знайдено") :> IActionResult
