namespace MegaDark
open UnityEngine

[<AbstractClass>]
type Singleton<'T when
        'T :> Singleton<'T> and
        'T : (new : unit -> 'T)>() =
    static let instance :'T = new 'T()
    static member Instance = instance

    abstract member Start : unit -> unit
    default this.Start() = ()

type EnemyManager() =
    inherit Singleton<EnemyManager>()

    let mutable enemyLayer = -1

    override this.Start() =
        enemyLayer <- LayerMask.NameToLayer("Enemy")

    member this.IsEnemy (gameObject :GameObject) =
        gameObject.layer = enemyLayer

type ManagerComponent() =
    inherit MonoBehaviour()

    let managers = [EnemyManager.Instance]

    member this.Start() =
        let callStart (x : Singleton<'T>) = x.Start()
        ignore <| List.map callStart managers

type EnemyHealth() =
    inherit Health()

    override this.OnCollide collision =
        let bullet = collision.sender.GetComponent<Bullet>()
        if bullet <> null
        then
            bullet.DidHit(this.gameObject)
            this.TakeDamage(bullet.Damage)