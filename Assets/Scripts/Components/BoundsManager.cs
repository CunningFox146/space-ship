using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Components
{
    public class BoundsManager : Singleton<BoundsManager>
    {
        private Dictionary<GameObject, IBoundsTrackable> _inBoundsList;

        //[SerializeProperty("Test")]
        public float BoundsWidth { get; private set; }
        public float BoundsHeight { get; private set; }

        public override void Awake()
        {
            base.Awake();

            _inBoundsList = new Dictionary<GameObject, IBoundsTrackable>();

            RecalculateBounds();
        }

        private void Update()
        {
            foreach (GameObject obj in _inBoundsList.Keys)
            {
                if (ShouldTeleport(obj, _inBoundsList[obj].BoundsOffset))
                {
                    _inBoundsList[obj].OnBoundsReached();
                }
            }
        }

        public void TryTeleport(GameObject obj)
        {
            var pos = obj.transform.position;
            var teleportPos = CalculateNewPos(pos, _inBoundsList[obj].BoundsOffset);

            if (teleportPos != pos)
            {
                obj.transform.position = teleportPos;
            }
        }

        private void RecalculateBounds()
        {
            var cam = Camera.main;
            var point = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            BoundsWidth = point.x;
            BoundsHeight = point.z;
        }

        public void Track(GameObject obj)
        {
            var trackable = obj.GetComponent<IBoundsTrackable>();
            if (trackable != null && !_inBoundsList.ContainsKey(obj))
            {
                _inBoundsList.Add(obj, trackable);
            }
        }

        public void StopTracking(GameObject obj) => _inBoundsList.Remove(obj);
        public bool IsInBounds(GameObject obj) => _inBoundsList.ContainsKey(obj);
        public bool ShouldTeleport(GameObject obj, float offset) => CalculateNewPos(obj.transform.position, offset) != obj.transform.position;

        private Vector3 CalculateNewPos(Vector3 pos, float offset)
        {
            if (pos.x < (-BoundsWidth - offset * 0.5f))
            {
                return new Vector3(pos.x + BoundsWidth * 2, pos.y, pos.z);
            }
            if (pos.x > (BoundsWidth + offset * 0.5f))
            {
                return new Vector3(pos.x - BoundsWidth * 2, pos.y, pos.z);
            }
            if (pos.z < (-BoundsHeight - offset * 0.5f))
            {
                return new Vector3(pos.x, pos.y, pos.z + BoundsHeight * 2);
            }
            if (pos.z > (BoundsHeight + offset * 0.5f))
            {
                return new Vector3(pos.x, pos.y, pos.z - BoundsHeight * 2);
            }

            return pos;
        }
    }
}