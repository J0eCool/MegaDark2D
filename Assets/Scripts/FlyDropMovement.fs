namespace MegaDark
open UnityEngine

type FlyDropMovement() =
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable flySpeed = 2.0f
    [<SerializeField>]
    let mutable fallSpeed = 4.0f

    let mutable isFalling = false

    member this.Update() =
        if isFalling
        then this.FallUpdate()
        else this.FlyUpdate()

    member this.FlyUpdate() =
        let player = PlayerManager.Instance.Player
        let playerPos = player.transform.position
        let delta = playerPos - this.transform.position
        this.MoveBy(flySpeed, 0.0f)

    member this.FallUpdate() =
        this.MoveBy(0.0f, fallSpeed)

    member this.MoveBy(x, y) =
        let t = Time.fixedDeltaTime
        let dx = x * t
        let dy = y * t
        this.transform.Translate(dx, dy, 0.0f)
