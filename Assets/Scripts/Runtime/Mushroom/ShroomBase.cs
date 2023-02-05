using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamShrimp.GGJ23
{
    public abstract class ShroomBase : MonoBehaviour
    {

        private long _shroomId;

        // private Vector2Int _shroomPosition;

        [SerializeField] private ShroomType _shroomType;

        [SerializeField] private Resource _costsResource;

        [SerializeField] private float _price;

        private ShroomBase _parent;

        private List<ShroomBase> _children;
        
        private LineRenderer _connector;
        
        public ShroomType ShroomType
        {
            get => _shroomType;
            set => _shroomType = value;
        }
        
        public Vector2Int ShroomPosition => (Vector2Int) MushroomManager.Instance?.GetCellPositionForMush(WorldPosition);

        public Vector3 WorldPosition { get; set; }

        public ShroomBase Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public List<ShroomBase> Children
        {
            get => _children;
            set => _children = value;
        }

        public Vector2Int ParentPosition => Parent.ShroomPosition;

        public long ShroomId => _shroomId;

        public LineRenderer Connector
        {
            get => _connector;
            set => _connector = value;
        }

        public bool IsOfType(ShroomType shroomType) => _shroomType.name == shroomType.name;

        public Team Owner { get; set; }

        public bool Pay()
        {
            if (ResourceTracker.Get(_costsResource) < _price)
                return false;
            return ResourceTracker.Subtract(_costsResource, _price);
        }
        
        public void ConnectChild(ShroomBase shroom)
        {
            this.Children.Add(shroom);
            shroom.Parent = this;
            // TODO connect via roots
        }

        // Feel free to override this method for specific Shrooms
        public void Initialize(Team owner)
        {
            this._shroomId = MushroomManager.Instance.GenerateUniqueId();
            this.Children = new List<ShroomBase>();
            this.transform.position = new Vector3(WorldPosition.x, WorldPosition.y, -3);
            _connector = GetComponent<LineRenderer>();
            Owner = owner;
            // _connector.SetPositions(new Vector3[] {transform.position, transform.position});

            //if (Parent != null)
            //{
            //    _connector.SetPosition(1, MushroomManager.Instance.GetWorldPositionForShroomPosition(ParentPosition));
            //}
        }
        

        public void OnDestroy()
        {
            try
            {
                MushroomManager.Instance.RemoveAllShroomConnectionsInvolving(this);
                MushroomManager.Instance.RemoveMushroom(this);
            }
            catch(Exception e)
            {}
        }

        public abstract void Start();

        public abstract void Update();

        public override string ToString()
        {
            return ShroomType + "{" +
                   "\n ID=" + _shroomId +
                   "\n Position=" + ShroomPosition +
                   "\n Parent=" + _parent + 
                   "\n WorldPosition=" + WorldPosition + "\n}";
            // "\n Children=" + String.Join('\n', _children) + "\n}";
        }
    }
}
