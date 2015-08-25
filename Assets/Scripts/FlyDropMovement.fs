namespace MegaDark
open UnityEngine

type FlyDropMovement() =
    inherit MonoBehaviour()

    [<SerializeField>] 
    let mutable flySpeed = -2.0f
    [<SerializeField>]
    let mutable dropInitialSpeed = 2.0f
    [<SerializeField>]
    let mutable dropAcceleration = -5.0f

    let mutable physics : SpritePhysics = null

    let mutable isFalling = false

    let shouldFall (delta : Vector3) = delta.x * flySpeed < 0.0f

    let SetVel x y =
        physics.vel <- Vector2(x, y)

    let FlyUpdate pos =
        let player = PlayerManager.Instance.Player
        let playerPos = player.transform.position
        let delta = playerPos - pos
        SetVel flySpeed 0.0f

        isFalling <- shouldFall delta

    let FallUpdate() =
        SetVel 0.0f dropInitialSpeed

    member this.Start() =
        physics <- this.GetComponent<SpritePhysics>()

    member this.FixedUpdate() =
        if not isFalling
        then FlyUpdate this.transform.position
        else FallUpdate()


type BottomlessPit() =
    inherit MonoBehaviour()

    interface Collideable with
        member this.OnCollide collision =
            let health = collision.sender.GetComponent<Health>()
            health.Kill()
