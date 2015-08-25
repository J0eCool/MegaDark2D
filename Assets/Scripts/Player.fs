namespace MegaDark
open UnityEngine

type PlayerHealth() =
    inherit Health()

    override this.Kill() =
        Application.LoadLevel(Application.loadedLevel)

    override this.OnCollide(collision : CollisionData) =
        if EnemyManager.Instance.IsEnemy collision.sender
        then this.TakeDamage(2)
