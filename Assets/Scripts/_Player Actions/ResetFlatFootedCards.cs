using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Player Actions/ Reset Flat Foot")]
    public class ResetFlatFootedCards : PlayerAction
    {
        public override void Execute(PlayerHolder player)
        {
            foreach (CardInstance c in player.cardsDown)
            {
                if (c.isFlatfooted)
                {
                    c.SetFlatfooted(false);
                }
            }
        }
    }
}