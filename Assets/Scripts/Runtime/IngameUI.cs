using System;
using System.Collections;
using System.Collections.Generic;
using TeamShrimp.GGJ23.Runtime;
using TMPro;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class IngameUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI resourceText;

        private Dictionary<Resource, String> textPerResource;

        // Start is called before the first frame update
        void Start()
        {
            textPerResource = new Dictionary<Resource, string>();
            textPerResource.Add(Resource.SPORE, ResourceTracker.ResourceToString(Resource.SPORE));
            // textPerResource.Add(Resource.OTHER, ResourceTracker.ResourceToString(Resource.OTHER));
            ResourceTracker.OnResourceChangedEvent.AddListener(resource =>
            {
                textPerResource[resource] = ResourceTracker.ResourceToString(resource);
                BuildText();
            });
            BuildText();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void BuildText()
        {
            resourceText.text = "";
            foreach (string line in textPerResource.Values)
            {
                resourceText.text += line + "\n";
            }
        }
    }
}