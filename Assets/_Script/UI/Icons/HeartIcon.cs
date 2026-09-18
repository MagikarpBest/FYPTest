using UnityEngine;
using UnityEngine.UI;

namespace GameIcons
{
    /// <summary>
    /// A single heart. state: 0 = empty, 1 = half, 2 = full.
    /// </summary>
    public class HeartIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite fullSprite;

        public void SetState(int state)
        {
            image.sprite = state switch
            {
                1 => fullSprite,
                _ => emptySprite,
            };
        }
    }
}