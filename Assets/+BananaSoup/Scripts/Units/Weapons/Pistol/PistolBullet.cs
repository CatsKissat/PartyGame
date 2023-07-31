using BananaSoup.Units;

namespace BananaSoup.Weapons
{
    public class PistolBullet : ProjectileBase
    {
        /// <summary>
        /// Method called in the inherited OnTriggerEnter.
        /// Used to kill the colliding player.
        /// </summary>
        /// <param name="player"></param>
        protected override void TriggerEnterAction(PlayerBase player)
        {
            player.Kill();
        }
    }
}
