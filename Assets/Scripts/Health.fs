namespace MegaDark
open UnityEngine
open UnityEngine.UI
open System
open Microsoft.FSharp.Core

[<AbstractClass>]
[<AllowNullLiteral>]
type Health() =
    inherit MonoBehaviour()

    [<SerializeField>]
    let mutable maxHealth = 4
    [<SerializeField>]
    let mutable invincibleTime = 0.5f

    let mutable health = 0

    let mutable flicker :Flicker = null

    member this.CurrentHealth = health
    member this.MaxHealth = maxHealth

    member this.Start() =
        flicker <- this.GetComponent<Flicker>()
        health <- maxHealth

    member this.FixedUpdate() =
        if health <= 0
        then this.Kill()

    abstract member Kill : unit -> unit
    default this.Kill() =
        GameObject.Destroy(this.gameObject)

    abstract member OnCollide : CollisionData -> unit
    interface Collideable with
        member this.OnCollide data = this.OnCollide data

    member this.TakeDamage damage =
        if not this.IsInvincible
        then
            health <- health - damage
            flicker.BeginFlicker(invincibleTime)

    member this.IsInvincible =
        flicker.IsFlickering()


type HealthText() =
    inherit MonoBehaviour()

    [<SerializeField>]
    let mutable health : Health = null

    let mutable text = None
    let mutable formatText = ""

    member this.Start() =
        text <- Some <| this.GetComponent<Text>()
        formatText <- match text with
                      | None -> ""
                      | Some t -> t.text

    member this.Update() =
        let curText =
            String.Format(formatText,
                health.CurrentHealth,
                health.MaxHealth)
        match text with
        | None -> ()
        | Some t -> t.text <- curText


//type Bullet() =
//    inherit MonoBehaviour()
//
//    [<SerializeField>]
//    let mutable speed = 15.0f
//    [<SerializeField>]
//    let mutable maxRange = 50.0f
//    [<SerializeField>]
//    let mutable damage = 1
//
//    let mutable physics = null
//    let mutable flownDist = 0.0f
//    let mutable shooter = null
//
//    member this.Damage = damage
