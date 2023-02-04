using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class GhostShroom : ShroomBase
    {

        private List<SpriteRenderer> _sprites;
        
        public override void Start()
        {
            Connector = GetComponent<LineRenderer>();
            _sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        }

        public override void Update()
        {
            transform.position = new Vector3(ShroomPosition.x, ShroomPosition.y, 1);
            Connector.SetPosition(0, new Vector3(ParentPosition.x, ParentPosition.y, 1));
            Connector.SetPosition(1, transform.position);

            Color tint = MushroomManager.Instance.PositionsInRange(ShroomPosition, ParentPosition)
                ? Color.blue
                : Color.red;
            tint.a = _sprites[0].color.a;
            _sprites.ForEach(sprite => sprite.color = tint);
            Connector.sharedMaterial.color = tint;
        }
    }
}