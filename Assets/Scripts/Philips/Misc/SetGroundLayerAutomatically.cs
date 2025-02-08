using UnityEngine;

namespace Switchgrass.Misc
{
    public class SetGroundLayerAutomatically : MonoBehaviour
    {
        private void Awake()
        {
            var groundLayer = LayerMask.NameToLayer("Ground");
            gameObject.layer = groundLayer;
            for (var childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                transform.GetChild(childIndex).gameObject.layer = groundLayer;
            }
        }
    }
}
